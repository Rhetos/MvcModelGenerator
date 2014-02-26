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
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICodeGenerator = Rhetos.Compiler.ICodeGenerator;

namespace Rhetos.MvcModelGenerator
{
    [Export(typeof(IGenerator))]
    public class MvcModelGenerator : IGenerator
    {
        private readonly IPluginsContainer<IMvcModelGeneratorPlugin> _plugins;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ILogger _logger;
        private readonly ILogger _sourceLogger;
        public const string AssemblyName = "Rhetos.Mvc";

        public MvcModelGenerator(
            IPluginsContainer<IMvcModelGeneratorPlugin> plugins,
            ICodeGenerator codeGenerator,
            ILogProvider logProvider,
            IAssemblyGenerator assemblyGenerator
        )
        {
            _plugins = plugins;
            _codeGenerator = codeGenerator;

            _logger = logProvider.GetLogger("MvcModelGenerator");
            _sourceLogger = logProvider.GetLogger("MvcModelGenerator source");
        }

        const string detectLineTag = @"\n\s*/\*.*?\*/\s*\r?\n";
        const string detectTag = @"/\*.*?\*/";

        public void Generate()
        {
            SimpleAssemblySource assemblySource = GenerateSource();
            _logger.Trace("References: " + string.Join(", ", assemblySource.RegisteredReferences));
            _sourceLogger.Trace(assemblySource.GeneratedCode);

            assemblySource.GeneratedCode = Regex.Replace(assemblySource.GeneratedCode, detectLineTag, "\n");
            assemblySource.GeneratedCode = Regex.Replace(assemblySource.GeneratedCode, detectTag, "");

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", AssemblyName + ".cs"), assemblySource.GeneratedCode);
        }

        private SimpleAssemblySource GenerateSource()
        {
            IAssemblySource generatedSource = _codeGenerator.ExecutePlugins(_plugins, "/*", "*/", new MvcModelInitialCodeGenerator());
            SimpleAssemblySource assemblySource = new SimpleAssemblySource
            {
                GeneratedCode = generatedSource.GeneratedCode,
                RegisteredReferences = generatedSource.RegisteredReferences
            };
            return assemblySource;
        }

        public IEnumerable<string> Dependencies
        {
            get { return null; }
        }
    }
}
