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
using Rhetos.Dsl;
using Rhetos.Extensibility;
using Rhetos.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Rhetos.MvcModelGenerator
{
    public class CaptionsValueGenerator : ICaptionsProvider
    {
        private readonly IDslModel _dslModel;
        private readonly IPluginsContainer<ICaptionsValuePlugin> _plugins;
        private readonly ILogger _logger;
        private readonly ILogger _performanceLogger;

        public CaptionsValueGenerator(
            IDslModel dslModel,
            IPluginsContainer<ICaptionsValuePlugin> plugins,
            ILogProvider logProvider)
        {
            _dslModel = dslModel;
            _plugins = plugins;
            _logger = logProvider.GetLogger("CaptionsValueGenerator");
            _performanceLogger = logProvider.GetLogger("Performance");
        }

        public IDictionary<string, string> Captions
        {
            get
            {
                if (_captionsValue == null)
                    GenerateCaptionValues();
                return _captionsValue;
            }
        }

        private Dictionary<string, string> _captionsValue;

        private void GenerateCaptionValues()
        {
            var sw = Stopwatch.StartNew();
            _captionsValue = new Dictionary<string, string>();

            var plugins = _plugins.GetPlugins().OrderBy(p => p.Order).ToList();
            foreach (var plugin in plugins)
                plugin.UpdateCaption(_dslModel, _captionsValue);

            _performanceLogger.Write(sw, "CaptionsValueGenerator.GenerateCaptionValues");
        }
    }
}
