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

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    /// <summary>
    /// Use instead of OverridableAttribute, when a new attribute should allways (or never) override existing attribute,
    /// regardless of the attribute's properties (value).
    /// </summary>
    /// <typeparam name="TAttributeValue"></typeparam>
    public class SimpleOverridableAttribute : SimpleOverridableAttribute<PropertyInfo>
    {
        public SimpleOverridableAttribute(string attributeName, bool overrideOldValue)
            : base(PropertyCodeGeneratorHelper.AttributesTag, attributeName, overrideOldValue)
        {
        }
    }

    public class SimpleOverridableDataStructureAttribute : SimpleOverridableAttribute<DataStructureInfo>
    {
        public SimpleOverridableDataStructureAttribute(string attributeName, bool overrideOldValue)
            : base(DataStructureCodeGenerator.AttributesTag, attributeName, overrideOldValue)
        {
        }
    }

    public class SimpleOverridableAttribute<TConceptType> : IOverridableAttribute
        where TConceptType : IConceptInfo
    {
        OverridableAttribute<TConceptType, object> _overridableAttribute;

        public SimpleOverridableAttribute(CsTag<TConceptType> attributeTag, string attributeName, bool overrideOldValue)
        {
            _overridableAttribute = new OverridableAttribute<TConceptType, object>(attributeTag, attributeName, (oldValue, newValue) => overrideOldValue, null);
        }

        public void InsertOrOverrideAttribute(ICodeBuilder codeBuilder, TConceptType conceptInfo, string attributeParameters)
        {
            _overridableAttribute.InsertOrOverrideAttribute(codeBuilder, conceptInfo, "", attributeParameters);
        }

        void IOverridableAttribute.InsertOrOverrideAttribute(ICodeBuilder codeBuilder, IConceptInfo conceptInfo, string value, string attributeParameters)
        {
            InsertOrOverrideAttribute(codeBuilder, (TConceptType)conceptInfo, attributeParameters);
        }
    }
}
