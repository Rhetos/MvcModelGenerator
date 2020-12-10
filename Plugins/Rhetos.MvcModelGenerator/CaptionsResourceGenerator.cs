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

using Rhetos.Compiler;
using Rhetos.Extensibility;
using Rhetos.Logging;
using Rhetos.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Rhetos.MvcModelGenerator
{
    [Export(typeof(IGenerator))]
    public class CaptionsResourceGenerator : IGenerator
    {
        private readonly IPluginsContainer<ICaptionsResourceGeneratorPlugin> _plugins;
        private readonly CaptionsInitialCodePlugin _initialCodePlugin;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ILogger _logger;
        private readonly ILogger _performanceLogger;
        private readonly RhetosBuildEnvironment _rhetosBuildEnvironment;
        private readonly CacheUtility _cacheUtility;

        public IEnumerable<string> Dependencies => null;

        public CaptionsResourceGenerator(
            IPluginsContainer<ICaptionsResourceGeneratorPlugin> plugins,
            CaptionsInitialCodePlugin initialCodePlugin,
            ICodeGenerator codeGenerator,
            ILogProvider logProvider,
            RhetosBuildEnvironment rhetosBuildEnvironment,
            FilesUtility filesUtility)
        {
            _plugins = plugins;
            _initialCodePlugin = initialCodePlugin;
            _codeGenerator = codeGenerator;
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

        public void Generate()
        {
            string sourceFromResources;
            if (GenerateNewResourcesResx())
            {
                var resxKeyValuePairs = GetKeyValuePairsFromResxFile(ResourcesFilePath);
                CompileResourceFile(resxKeyValuePairs);
                sourceFromResources = GenerateSourceFromCompiledResources(resxKeyValuePairs);
            }
            else
            {
                _cacheUtility.CopyFromCache(ResourcesFilePath);
                _cacheUtility.CopyFromCache(CompiledResourcesFilePath);
                _cacheUtility.CopyFromCache(SourceFromCompiledResources);
                sourceFromResources = File.ReadAllText(SourceFromCompiledResources);
            }

            File.WriteAllText(Path.Combine(_rhetosBuildEnvironment.GeneratedAssetsFolder, ResourcesAssemblyName + ".cs"), sourceFromResources, Encoding.UTF8);

        }

        private bool GenerateNewResourcesResx()
        {
            var sw = Stopwatch.StartNew();

            // NOTE: There is no need for ICaptionsResourceGeneratorPlugin plugins, they are not used.
            // CaptionsInitialCodePlugin _initialCodePlugin is this only implementation and all the work
            // is done there by calling ICaptionsProvider and its ICaptionsValuePlugin plugins.
            // The ICaptionsResourceGeneratorPlugin interface can be deleted, and the code from CaptionsInitialCodePlugin called directly.
            // We could leave ICaptionsResourceGeneratorPlugin just in case we need to add
            // to the resx file something other then simple "data" elements for captions.
            var generatedSource = _codeGenerator.ExecutePlugins(_plugins, "<!--", "-->", _initialCodePlugin);
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
            _cacheUtility.CopyToCache(ResourcesFilePath);

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


        private IEnumerable<KeyValuePair<string, string>> GetKeyValuePairsFromResxFile(string path)
        {
            return XDocument.Load(path).Root.Descendants("data").Select(x => KeyValuePair.Create(x.Attribute("name").Value, x.Element("value").Value));
        }

        private void CompileResourceFile(IEnumerable<KeyValuePair<string, string>> resxKeyValuePairs)
        {
            var sw = Stopwatch.StartNew();

            using (IResourceWriter writer = new ResourceWriter(CompiledResourcesFilePath))
            {
                foreach (var resxKeyValue in resxKeyValuePairs)
                {
                    try
                    {
                        writer.AddResource(resxKeyValue.Key.ToString(), resxKeyValue.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new FrameworkException(string.Format("Error while compiling resource file \"{0}\" on key \"{1}\".", ResourcesFileName, resxKeyValue.Key.ToString()), ex);
                    }
                }

                writer.Generate();
                writer.Close();
            }
            _cacheUtility.CopyToCache(CompiledResourcesFilePath);
            _performanceLogger.Write(sw, nameof(CompileResourceFile));
        }

        //The StronglyTypedResourceBuilder was generating the Captions.cs file with the value splitted in this way
        //so we are keeping the same functionality
        private IEnumerable<string> SplitStringForCaptionsValue(string str)
        {
            var firstChunkSize = 81;
            var otherChunkSizes = 80;
            yield return str.Substring(0, Math.Min(firstChunkSize, str.Length));
            for (int i = 81; i < str.Length; i += otherChunkSizes)
                yield return str.Substring(i, Math.Min(otherChunkSizes, str.Length - i));
        }

        /// <summary>
        /// Generates "resources.cs" file. In standard projects, it is generated automatically by Visual Studio.
        /// </summary>
        private string GenerateSourceFromCompiledResources(IEnumerable<KeyValuePair<string, string>> resxKeyValuePairs)
        {
            var sw = Stopwatch.StartNew();

            var sb = new StringBuilder();
            sb.Append(@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rhetos.Mvc {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""System.Resources.Tools.StronglyTypedResourceBuilder"", ""4.0.0.0"")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Captions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute(""Microsoft.Performance"", ""CA1811:AvoidUncalledPrivateCode"")]
        internal Captions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager(""Captions"", typeof(Captions).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
");
            foreach (var resxKeyValue in resxKeyValuePairs)
            {
                var formatedKey = string.Join($" +{Environment.NewLine}                        " , SplitStringForCaptionsValue(resxKeyValue.Key).Select(k => $"\"{k}\""));
                sb.Append(@$"        
        /// <summary>
        ///   Looks up a localized string similar to {resxKeyValue.Value}.
        /// </summary>
        public static string {resxKeyValue.Key} {{
            get {{
                return ResourceManager.GetString({formatedKey}, resourceCulture);
            }}
        }}
");
            }

            sb.Append(@"    }
}
");

            var sourceCode = sb.ToString();

            File.WriteAllText(SourceFromCompiledResources, sourceCode);
            _cacheUtility.CopyToCache(SourceFromCompiledResources);
            _performanceLogger.Write(sw, nameof(GenerateSourceFromCompiledResources));
            return sourceCode;
        }
    }
}
