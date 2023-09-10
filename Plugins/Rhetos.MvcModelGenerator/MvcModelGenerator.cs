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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Rhetos.MvcModelGenerator
{
    [Export(typeof(IGenerator))]
    public class MvcModelGenerator : IGenerator
    {
        private readonly IPluginsContainer<IMvcModelGeneratorPlugin> _plugins;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ILogger _logger;
        private readonly ILogger _performanceLogger;
        private readonly RhetosBuildEnvironment _rhetosBuildEnvironment;
        private readonly MvcModelGeneratorOptions _options;
        public const string AssemblyName = "Rhetos.Mvc";

        public MvcModelGenerator(
            IPluginsContainer<IMvcModelGeneratorPlugin> plugins,
            ICodeGenerator codeGenerator,
            ILogProvider logProvider,
            RhetosBuildEnvironment rhetosBuildEnvironment,
            MvcModelGeneratorOptions options
        )
        {
            _plugins = plugins;
            _codeGenerator = codeGenerator;
            _logger = logProvider.GetLogger(GetType().Name);
            _performanceLogger = logProvider.GetLogger("Performance");
            _rhetosBuildEnvironment = rhetosBuildEnvironment;
            _options = options;
        }

        const string detectLineTag = @"\n\s*/\*.*?\*/\s*\r?\n";
        const string detectTag = @"/\*.*?\*/";

        public void Generate()
        {
            if (!_options.GenerateMvcModel)
            {
                _logger.Trace(() => $"Skipped generating MVC model file because the option {nameof(MvcModelGeneratorOptions.GenerateMvcModel)} is disabled.");
                return;
            }

            var sw = Stopwatch.StartNew();
            var generatedSourceCode = _codeGenerator.ExecutePlugins(_plugins, "/*", "*/", null);

            generatedSourceCode = Regex.Replace(generatedSourceCode, detectLineTag, "\n");
            generatedSourceCode = Regex.Replace(generatedSourceCode, detectTag, "");

            File.WriteAllText(Path.Combine(_rhetosBuildEnvironment.GeneratedAssetsFolder, AssemblyName + ".cs"), generatedSourceCode, Encoding.UTF8);

            _performanceLogger.Write(sw, "MvcModelGenerator.Generate");
        }

        public IEnumerable<string> Dependencies
        {
            get { return new[] { typeof(CaptionsResourceGenerator).FullName }; }
        }
    }
}
