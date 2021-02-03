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
using System.ComponentModel.Composition;

namespace Rhetos.MvcModelGenerator
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(InitializationConcept))]
    public class MvcModelInitialCodeGenerator : IMvcModelGeneratorPlugin
    {
        public static readonly string UsingTag = "/*using*/";
        public static readonly string OverrideCaptionsResourceClassTag = "/*OverrideCaptionsResourceClass*/";

        private readonly MvcModelGeneratorOptions _options;

        public MvcModelInitialCodeGenerator(MvcModelGeneratorOptions options)
        {
            _options = options;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            string codeSnippet =
$@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CaptionsResourceClass = {OverrideCaptionsResourceClassTag} {_options.CaptionsClassNamespace}.{_options.CaptionsClassName};

{UsingTag}

/*
    If you need to use additional DataAnnotation attributes, or override existing attributes in the generated MvcModel,
    create the following class in your project:

    [MetadataTypeAttribute(typeof(MyModel.AdditionalAttributes))]
    public partial class MyModel
    {{
        internal sealed class AdditionalAttributes
        {{
            private AdditionalAttributes() {{ }}

            [Display(Name = ""Last Name"", Order = 1, Prompt = ""Enter Last Name"")]
            public string LastName {{ get; set; }}

            // Add other properties ...
        }}
    }}
*/

";
            codeBuilder.InsertCode(codeSnippet);
        }
    }
}