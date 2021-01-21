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
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(DataStructureInfo))]
    public class DataStructureCodeGenerator : IMvcModelGeneratorPlugin
    {
        public static readonly CsTag<DataStructureInfo> PropertiesTag = "Properties";
        public static readonly CsTag<DataStructureInfo> AttributesTag = "Attributes";

		[Obsolete]
        public static bool IsSupported(DataStructureInfo conceptInfo)
        {
            return true;
        }

        public static bool IsEntityType(DataStructureInfo conceptInfo)
        {
            return conceptInfo is IOrmDataStructure
                || conceptInfo is BrowseDataStructureInfo
                || conceptInfo is QueryableExtensionInfo
                || conceptInfo is ComputedInfo
                || conceptInfo is ActionInfo; // TODO: Remove ActionInfo. It is here only for backward compatibility.
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            DataStructureInfo info = (DataStructureInfo)conceptInfo;

            codeBuilder.InsertCode(ImplementationCodeSnippet(info));

			if (IsEntityType(info))
				Dom.DefaultConcepts.DataStructureCodeGenerator.AddInterfaceAndReference(codeBuilder, typeof(Mvc.BaseMvcModel), info);

            LocalizedDisplayAttribute.InsertOrOverrideAttribute(codeBuilder, info, LocalizedDisplayAttributeProperties(info));
        }

        private static string ImplementationCodeSnippet(DataStructureInfo info)
        {
            return string.Format(@"
namespace Rhetos.Mvc.{0}
{{
    {3}
    public partial class {1} {4}
    {{
        public const string Entity{1} = ""{1}"";

        {2}
    }}
}}
",
                info.Module.Name,
                info.Name,
                PropertiesTag.Evaluate(info),
                AttributesTag.Evaluate(info),
	            Dom.DefaultConcepts.DataStructureCodeGenerator.InterfaceTag.Evaluate(info));
        }

        static readonly SimpleOverridableDataStructureAttribute LocalizedDisplayAttribute = new SimpleOverridableDataStructureAttribute("Rhetos.Mvc.LocalizedDisplayName", false);

        static string LocalizedDisplayAttributeProperties(DataStructureInfo info)
        {
            return $@"""{DataStructureCaption.GetCaptionResourceKey(info)}"", typeof(CaptionsResourceClass)";
        }
    }
}