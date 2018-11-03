/*
    Copyright (C) 2014 Omega software d.o.o.

    This file is part of Rhetos.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Microsoft.CSharp;
using Rhetos.Compiler;
using Rhetos.Extensibility;
using Rhetos.Logging;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Resources.Tools;
using System.Text;
using System.Text.RegularExpressions;

namespace Rhetos.MvcModelGenerator
{
    [Export(typeof(IGenerator))]
    public class CaptionsResourceGenerator : IGenerator
    {
        private readonly IPluginsContainer<ICaptionsResourceGeneratorPlugin> _plugins;
        private readonly CaptionsInitialCodePlugin _initialCodePlugin;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IAssemblyGenerator _assemblyGenerator;
        private readonly ILogger _logger;
        private readonly ILogger _performanceLogger;

        public CaptionsResourceGenerator(
            IPluginsContainer<ICaptionsResourceGeneratorPlugin> plugins,
            CaptionsInitialCodePlugin initialCodePlugin,
            ICodeGenerator codeGenerator,
            IAssemblyGenerator assemblyGenerator,
            ILogProvider logProvider)
        {
            _plugins = plugins;
            _initialCodePlugin = initialCodePlugin;
            _codeGenerator = codeGenerator;
            _assemblyGenerator = assemblyGenerator;
            _logger = logProvider.GetLogger("CaptionsResourceGenerator");
            _performanceLogger = logProvider.GetLogger("Performance");
        }

        public static string ResourcesAssemblyName { get { return "Captions"; } }
        public static string ResourcesFileName { get { return "Captions.resx"; } }
        public static string ResourcesFilePath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", ResourcesFileName); } }
        public static string CompiledResourcesFilePath { get { return Path.ChangeExtension(ResourcesFilePath, ResourceExtension); } }
        public static string ResourcesNamespaceName { get { return "Rhetos.Mvc"; } }
        public static string ResourcesClassName { get { return "Captions"; } }
        public static string ResourcesClassFullName { get { return ResourcesNamespaceName + "." + ResourcesClassName; } }
        private static readonly string ResourceExtension = "resources";
        public static string ResourceFullName => $"{ResourcesClassFullName}.{ResourceExtension}";
        public static string ResourcesAssemblyDllPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", ResourcesAssemblyName + ".dll"); } }

        public void Generate()
        {
            var sw = Stopwatch.StartNew();

            GenerateResourcesResx();
            _performanceLogger.Write(sw, "CaptionsResourceGenerator generated resx");

            CompileResourceFile();
            _performanceLogger.Write(sw, "CaptionsResourceGenerator compiled resx to resources");

            var assemblySource = GenerateResourcesCs();
            _performanceLogger.Write(sw, "CaptionsResourceGenerator generated cs");

            var manifestResources = new List<ManifestResource> { new ManifestResource { Name = ResourceFullName, Path = CompiledResourcesFilePath } };
            _assemblyGenerator.Generate(assemblySource, ResourcesAssemblyDllPath, manifestResources);
            _performanceLogger.Write(sw, "CaptionsResourceGenerator generated dll");
        }

        private void GenerateResourcesResx()
        {
            IAssemblySource generatedSource = _codeGenerator.ExecutePlugins(_plugins, "<!--", "-->", _initialCodePlugin);

            string resxContext = generatedSource.GeneratedCode;
            resxContext = CleanupXml(resxContext);
            File.WriteAllText(ResourcesFilePath, resxContext, Encoding.UTF8);
        }

        const string detectLineTag = @"\n\s*<!--.*?-->\s*\r?\n";
        const string detectTag = @"<!--.*?-->";

        private static string CleanupXml(string resourcesXml)
        {
            resourcesXml = Regex.Replace(resourcesXml, detectLineTag, "\n");
            resourcesXml = Regex.Replace(resourcesXml, detectTag, "");
            resourcesXml = resourcesXml.Trim();
            return resourcesXml;
        }

        private void CompileResourceFile()
        {
            ResXResourceReader resxReader = new ResXResourceReader(ResourcesFilePath);
            IDictionaryEnumerator resxEnumerator = resxReader.GetEnumerator();

            using (IResourceWriter writer = new ResourceWriter(CompiledResourcesFilePath))
            {
                while (resxEnumerator.MoveNext())
                {
                    try
                    {
                        writer.AddResource(resxEnumerator.Key.ToString(), resxEnumerator.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new FrameworkException(string.Format("Error while compiling resource file \"{0}\" on key \"{1}\".", ResourcesFileName, resxEnumerator.Key.ToString()), ex);
                    }
                }

                writer.Generate();
                writer.Close();
            }
            resxReader.Close();
        }

        private IAssemblySource GenerateResourcesCs()
        {
            string[] errors;
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CodeCompileUnit code = StronglyTypedResourceBuilder.Create(ResourcesFilePath, ResourcesClassName, ResourcesNamespaceName, "", provider, false, out errors);

            if (errors.Length > 0)
            {
                foreach (var error in errors)
                    _logger.Error(error);

                throw new Rhetos.FrameworkException(string.Format(
                    "{0} errors in generated resource file '{1}'. First error: {2}",
                    errors.Length, ResourcesFilePath, errors[0]));
            }

            var writer = new StringWriter();
            provider.GenerateCodeFromCompileUnit(code, writer, new System.CodeDom.Compiler.CodeGeneratorOptions());

            return new SimpleAssemblySource
            {
                GeneratedCode = writer.ToString(),
                RegisteredReferences = new List<string>
                {
                    typeof(object).Assembly.Location, // Location of the mscorlib.dll
                    typeof(Uri).Assembly.Location // Location of the System.dll
                } 
            };
        }

        public IEnumerable<string> Dependencies
        {
            get { return null; }
        }
    }
}
