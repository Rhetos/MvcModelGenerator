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
using Rhetos.Utilities;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly RhetosBuildEnvironment _rhetosBuildEnvironment;
        private readonly CacheUtility _cacheUtility;

        public IEnumerable<string> Dependencies => null;

        public CaptionsResourceGenerator(
            IPluginsContainer<ICaptionsResourceGeneratorPlugin> plugins,
            CaptionsInitialCodePlugin initialCodePlugin,
            ICodeGenerator codeGenerator,
            IAssemblyGenerator assemblyGenerator,
            ILogProvider logProvider,
            RhetosBuildEnvironment rhetosBuildEnvironment,
            FilesUtility filesUtility)
        {
            _plugins = plugins;
            _initialCodePlugin = initialCodePlugin;
            _codeGenerator = codeGenerator;
            _assemblyGenerator = assemblyGenerator;
            _logger = logProvider.GetLogger("CaptionsResourceGenerator");
            _performanceLogger = logProvider.GetLogger($"Performance.{nameof(CaptionsResourceGenerator)}");
            _rhetosBuildEnvironment = rhetosBuildEnvironment;
            _cacheUtility = new CacheUtility(typeof(CaptionsResourceGenerator), rhetosBuildEnvironment, filesUtility);
        }

        public static string ResourcesAssemblyName => "Captions";
        public static string ResourcesFileName => "Captions.resx";
        public static string ResourcesNamespaceName => "Rhetos.Mvc";
        public static string ResourcesClassName => "Captions";
        public static string ResourcesClassFullName => ResourcesNamespaceName + "." + ResourcesClassName;
        public string ResourcesFilePath => Path.Combine(_rhetosBuildEnvironment.GeneratedAssetsFolder, ResourcesFileName);
        public string CompiledResourcesFilePath => Path.ChangeExtension(ResourcesFilePath, "resources");
        public string SourceFromCompiledResources => $"{CompiledResourcesFilePath}.cs";
        public string ResourcesAssemblyDllPath => GetResourcesAssemblyDllPath(_rhetosBuildEnvironment.GeneratedAssetsFolder);

        public static string GetResourcesAssemblyDllPath(string assetsFolder) => Path.Combine(assetsFolder, ResourcesAssemblyName + ".dll");

        public void Generate()
        {
            string sourceFromResources;
            if (GenerateNewResourcesResx())
            {
                CompileResourceFile();
                sourceFromResources = GenerateSourceFromCompiledResources();
            }
            else
            {
                _cacheUtility.CopyFromCache(CompiledResourcesFilePath);
                _cacheUtility.CopyFromCache(SourceFromCompiledResources);
                sourceFromResources = File.ReadAllText(SourceFromCompiledResources);
            }

            var assemblySource = new SimpleAssemblySource
            {
                GeneratedCode = sourceFromResources,
                RegisteredReferences = new List<string>
                {
                    typeof(object).Assembly.Location, // Location of the mscorlib.dll
                    typeof(Uri).Assembly.Location // Location of the System.dll
                }
            };
            var resources = new List<ManifestResource> { new ManifestResource { Name = Path.GetFileName(CompiledResourcesFilePath), Path = CompiledResourcesFilePath, IsPublic = true } };
            _assemblyGenerator.Generate(assemblySource, ResourcesAssemblyDllPath, resources);
        }

        private bool GenerateNewResourcesResx()
        {
            var sw = Stopwatch.StartNew();

            IAssemblySource generatedSource = _codeGenerator.ExecutePlugins(_plugins, "<!--", "-->", _initialCodePlugin);
            string resxContext = generatedSource.GeneratedCode;
            resxContext = CleanupXml(resxContext);

            _performanceLogger.Write(sw, "GenerateNewResourcesResx: ExecutePlugins and cleanup.");

            var resxHash = _cacheUtility.ComputeHash(resxContext);
            var cachedHash = _cacheUtility.LoadHash(ResourcesFilePath);

            _performanceLogger.Write(sw, "GenerateNewResourcesResx: Hash.");

            if (resxHash.SequenceEqual(cachedHash))
            {
                _logger.Trace(() => $"'{ResourcesFilePath}' hash not changed, using cache.");
                if (_cacheUtility.FileIsCached(CompiledResourcesFilePath) && _cacheUtility.FileIsCached(SourceFromCompiledResources))
                    return false;

                _logger.Trace(() => $"'{CompiledResourcesFilePath}' and '{SourceFromCompiledResources}' expected in cache, but some are missing.");
            }
            else
            {
                _logger.Trace(() => $"'{ResourcesFilePath}' hash changed, invalidating cache.");
            }

            _cacheUtility.RemoveFromCache(CompiledResourcesFilePath);
            _cacheUtility.RemoveFromCache(SourceFromCompiledResources);

            File.WriteAllText(ResourcesFilePath, resxContext, Encoding.UTF8);
            _cacheUtility.SaveHash(ResourcesFilePath, resxHash);

            _performanceLogger.Write(sw, "GenerateNewResourcesResx: Save.");

            return true;
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
            var sw = Stopwatch.StartNew();
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
            _cacheUtility.CopyToCache(CompiledResourcesFilePath);
            _performanceLogger.Write(sw, nameof(CompileResourceFile));
        }

        /// <summary>
        /// Generates "resources.cs" file. In standard projects, it is generated automatically by Visual Studio.
        /// </summary>
        private string GenerateSourceFromCompiledResources()
        {
            var sw = Stopwatch.StartNew();
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
            var sourceCode = writer.ToString();

            File.WriteAllText(SourceFromCompiledResources, sourceCode);
            _cacheUtility.CopyToCache(SourceFromCompiledResources);
            _performanceLogger.Write(sw, nameof(GenerateSourceFromCompiledResources));
            return sourceCode;
        }
    }
}
