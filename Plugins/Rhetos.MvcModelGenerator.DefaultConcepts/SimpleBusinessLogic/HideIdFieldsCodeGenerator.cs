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
using System.ComponentModel.Composition;

namespace Rhetos.MvcModelGenerator.DefaultConcepts.SimpleBusinessLogic
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(PropertyInfo))]
    [ExportMetadata(MefProvider.DependsOn, typeof(SimplePropertyCodeGenerator))]
    public class HideIdFieldsCodeGenerator : IMvcModelGeneratorPlugin
    {
        static SimpleOverridableAttribute _renderModeAttribute = new SimpleOverridableAttribute("Rhetos.Mvc.RenderMode", false);

        public static bool IsSupported(PropertyInfo info)
        {
            return DataStructureCodeGenerator.IsSupported(info.DataStructure)
                && (info is GuidPropertyInfo || info is IntegerPropertyInfo);
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (PropertyInfo)conceptInfo;

            if (IsSupported(info))
                if (info.Name.EndsWith("ID"))
                    _renderModeAttribute.InsertOrOverrideAttribute(codeBuilder, info, @"Rhetos.Mvc.RenderMode.EditModeOnly");
        }
    }
}
