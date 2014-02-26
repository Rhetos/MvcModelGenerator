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
using Rhetos.Dsl.DefaultConcepts;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    public static class PropertyCodeGeneratorHelper
    {
        public static readonly CsTag<PropertyInfo> AttributesTag = "Attributes";

        public static void GenerateCodeForType(PropertyInfo info, ICodeBuilder codeBuilder, string type, string nameSuffix = "")
        {
            codeBuilder.InsertCode(PropertyCodeSnippet(info, type, nameSuffix), DataStructureCodeGenerator.PropertiesTag, info.DataStructure);
            _displayAttribute.InsertOrOverrideAttribute(codeBuilder, info, DisplayAttributeParameters(info));
        }

        private static string PropertyCodeSnippet(PropertyInfo info, string type, string nameSuffix)
        {
            return string.Format(
                AttributesTag.Evaluate(info) + @"
        public virtual {1} {0}{2} {{ get; set; }}
        public const string Property{0}{2} = ""{0}{2}"";
        
        ",
                info.Name,
                type,
                nameSuffix);
        }

        static SimpleOverridableAttribute _displayAttribute = new SimpleOverridableAttribute("Display", false);

        static string DisplayAttributeParameters(PropertyInfo info)
        {
            return string.Format(@"Name = ""{0}"", ResourceType = typeof({1}), AutoGenerateFilter = true",
                PropertyCaption.GetCaptionResourceKey(info),
                Rhetos.MvcModelGenerator.CaptionsResourceGenerator.ResourcesClassFullName);
        }
    }
}