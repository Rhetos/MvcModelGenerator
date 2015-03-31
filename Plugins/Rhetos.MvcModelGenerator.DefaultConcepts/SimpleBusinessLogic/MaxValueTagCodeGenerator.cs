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
    [ExportMetadata(MefProvider.Implements, typeof(MaxValueInfo))]
    public class MaxValueTagCodeGenerator : IMvcModelGeneratorPlugin
    {
        static OverridableAttribute<int> _overridableAttributeInteger = new OverridableAttribute<int>(
          "Rhetos.Mvc.MaxValueInteger", (oldValue, newValue) => newValue < oldValue, str => int.Parse(str));

        static OverridableAttribute<decimal> _overridableAttributeDecimal = new OverridableAttribute<decimal>(
            "Rhetos.Mvc.MaxValueDecimal", (oldValue, newValue) => newValue < oldValue, str => decimal.Parse(str));

        static OverridableAttribute<DateTime> _overridableAttributeDateTime = new OverridableAttribute<DateTime>(
            "Rhetos.Mvc.MaxValueDateTime", (oldValue, newValue) => newValue < oldValue, str => DateTime.Parse(str));

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (MaxValueInfo)conceptInfo;

            var attribute = (info.Property is IntegerPropertyInfo) ? (IOverridableAttribute)_overridableAttributeInteger :
                (info.Property is MoneyPropertyInfo || info.Property is DecimalPropertyInfo) ? (IOverridableAttribute)_overridableAttributeDecimal :
                (info.Property is DatePropertyInfo || info.Property is DateTimePropertyInfo) ? (IOverridableAttribute)_overridableAttributeDateTime : null;

            attribute.InsertOrOverrideAttribute(codeBuilder, info.Property, info.Value, string.Format(
                @"MaxValue = ""{0}"", ErrorMessage = ""Value for {1} must be less than or equal to {0}.""",
                info.Value, info.Property.Name));
        }
    }
}