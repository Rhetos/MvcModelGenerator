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

using System;
using System.Configuration;
using Rhetos.Compiler;
using Rhetos.Dsl;
using System.ComponentModel.Composition;
using Rhetos.Extensibility;
using Rhetos.Utilities;
using System.Reflection;

namespace Rhetos.MvcModelGenerator
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(InitializationConcept))]
    public class MvcModelInitialCodeGenerator : IMvcModelGeneratorPlugin
    {
        public const string UsingTag = "/*using*/";
        public const string OverrideCaptionsResourceClassTag = "/*OverrideCaptionsResourceClass*/";

        private readonly RhetosBuildEnvironment _rhetosBuildEnvironment;

        public MvcModelInitialCodeGenerator(RhetosBuildEnvironment rhetosBuildEnvironment)
        {
            _rhetosBuildEnvironment = rhetosBuildEnvironment;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            codeBuilder.InsertCode(CodeSnippet);

            codeBuilder.AddReference(CaptionsResourceGenerator.GetResourcesAssemblyDllPath(_rhetosBuildEnvironment.GeneratedAssetsFolder));
            codeBuilder.AddReferencesFromDependency(typeof(Guid));
            codeBuilder.AddReferencesFromDependency(typeof(System.Linq.Enumerable));
            codeBuilder.AddReference(Assembly.Load("System.Runtime, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
            codeBuilder.AddReference(Assembly.Load("System.ComponentModel.Primitives, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
            codeBuilder.AddReference(Assembly.Load("System.ComponentModel.Annotations, Version=4.2.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
        }

        private string CodeSnippet =
@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CaptionsResourceClass = " + OverrideCaptionsResourceClassTag + " " + Rhetos.MvcModelGenerator.CaptionsResourceGenerator.ResourcesClassFullName + @";

" + UsingTag + @"

/*
    If you need to use additional DataAnnotation attributes, or override existing attributes in the generated MvcModel,
    create the following class in your project:

    [MetadataTypeAttribute(typeof(MyModel.AdditionalAttributes))]
    public partial class MyModel
    {
        internal sealed class AdditionalAttributes
        {
            private AdditionalAttributes() { }

            [Display(Name = ""Last Name"", Order = 1, Prompt = ""Enter Last Name"")]
            public string LastName { get; set; }

            // Add other properties ...
        }
    }
*/

";
    }
}