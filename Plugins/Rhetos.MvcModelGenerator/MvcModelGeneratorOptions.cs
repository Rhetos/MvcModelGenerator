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
    /// <summary>
    /// Build options for <see cref="CaptionsResourceGenerator"/> and <see cref="MvcModelGenerator"/>.
    /// </summary>
    [Options("MvcModelGenerator")]
    public class MvcModelGeneratorOptions
    {
        /// <summary>
        /// Generates MVC model C# source file "Rhetos.Mvc.cs".
        /// The file is not compiled within the Rhetos app. It is intended to be copied to the ASP.NET MVC "frontend" app.
        /// See Readme.md for more info.
        /// </summary>
        public bool GenerateMvcModel { get; set; } = true;

        /// <summary>
        /// Generated captions resource file with the corresponding source file "Captions.*"
        /// The files are not compiled within the Rhetos app. They are intended to be copied to the ASP.NET MVC "frontend" app.
        /// See Readme.md for more info.
        /// </summary>
        public bool GenerateCaptionsResource { get; set; } = true;

        /// <summary>
        /// Name of the automatically generated class for accessing resources (Captions class).
        /// </summary>
        /// <remarks>
        /// Configure this option to match the namespace of the ASP.NET MVC application
        /// where the captions will be used.
        /// If the captions class is generated in a separate project (for example, 
        /// in Captions.Designer.cs file in a ASP.NET MVC applications),
        /// configure this option to match the namespace from that file.
        /// </remarks>
        public string CaptionsClassNamespace { get; set; } = "Rhetos.Mvc";

        /// <summary>
        /// Name of the automatically generated class for accessing resources.
        /// </summary>
        public string CaptionsClassName { get; set; } = "Captions";

        /// <summary>
        /// Name of the resource file without its extension (.resx).
        /// It may include namespace to match the MVC application namespace
        /// with relative path of the resource file within the project
        /// (for example WebApplication1.SubfolderName.Captions).
        /// The namespaces does not need to be specified if it is same as <see cref="CaptionsClassNamespace"/>
        /// </summary>
        public string ResourceFullName { get; set; } = "Captions";
    }
}
