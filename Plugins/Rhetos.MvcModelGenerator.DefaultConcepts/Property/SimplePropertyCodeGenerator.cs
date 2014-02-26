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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(PropertyInfo))]
    public class SimplePropertyCodeGenerator : IMvcModelGeneratorPlugin
    {
        private static IDictionary<Type, string> supportedPropertyTypes = new Dictionary<Type, string>
        {
            { typeof(BinaryPropertyInfo), "byte[]" },
            { typeof(BoolPropertyInfo), "bool?" },
            { typeof(DatePropertyInfo), "DateTime?" },
            { typeof(DateTimePropertyInfo), "DateTime?" },
            { typeof(DecimalPropertyInfo), "decimal?" },
            { typeof(GuidPropertyInfo), "Guid?" },
            { typeof(IntegerPropertyInfo), "int?" },
            { typeof(LongStringPropertyInfo), "string" },
            { typeof(MoneyPropertyInfo), "decimal?" },
            { typeof(ShortStringPropertyInfo), "string" },
        };

        private static string GetPropertyType(PropertyInfo conceptInfo)
        {
            return supportedPropertyTypes
                .Where(prop => prop.Key.IsAssignableFrom(conceptInfo.GetType()))
                .Select(prop => prop.Value)
                .FirstOrDefault();
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            PropertyInfo info = (PropertyInfo)conceptInfo;
            string propertyType = GetPropertyType(info);

            if (!String.IsNullOrEmpty(propertyType) && DataStructureCodeGenerator.IsSupported(info.DataStructure))
                PropertyCodeGeneratorHelper.GenerateCodeForType(info, codeBuilder, propertyType);
        }
    }
}