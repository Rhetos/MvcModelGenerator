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
using System;
using System.Collections.Generic;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    public interface IOverridableAttribute
    {
        void InsertOrOverrideAttribute(ICodeBuilder codeBuilder, IConceptInfo conceptInfo, string value, string attributeParameters);
    }

    public class OverridableAttributeDictionary
    {
        protected static readonly Dictionary<string, object> OldAttributeValue = new Dictionary<string, object>();
    }

    /// <summary>
    /// This class helps generating the source code for attributes that have AllowMultiple = false.
    /// The overrideOldValue constructor parameter can be used to define if a repeated attibute instance should be ignored,
    /// or should be used to override previously inserted attribute.
    /// </summary>
    public class OverridableAttribute<TAttributeValue> : OverridableAttribute<PropertyInfo, TAttributeValue>
    {
        public OverridableAttribute(string attributeName, Func<TAttributeValue, TAttributeValue, bool> overrideOldValue, Func<string, TAttributeValue> valueParser = null)
            : base(PropertyCodeGeneratorHelper.AttributesTag, attributeName, overrideOldValue, valueParser)
        {
        }
    }

    public class OverridableDataStructureAttribute<TAttributeValue> : OverridableAttribute<DataStructureInfo, TAttributeValue>
    {
        public OverridableDataStructureAttribute(string attributeName, Func<TAttributeValue, TAttributeValue, bool> overrideOldValue, Func<string, TAttributeValue> valueParser = null)
            : base(DataStructureCodeGenerator.AttributesTag, attributeName, overrideOldValue, valueParser)
        {
        }
    }

    public class OverridableAttribute<TConceptType, TAttributeValue> : OverridableAttributeDictionary, IOverridableAttribute
        where TConceptType : IConceptInfo
    {
        private CsTag<TConceptType> _attributeTag;
        private string _attributeName;
        private Func<TAttributeValue, TAttributeValue, bool> _overrideOldValue;
        private CsTag<TConceptType> _overrideOldAttributeTag;
        private Func<string, TAttributeValue> _valueParser;

        /// <param name="overrideOldValue">(oldValue, newValue) => bool whether newValue should override existing attribute with old value.</param>
        /// <param name="valueParser">valueParser needs to be set only if this class is used through IOverridableAttribute interface, since it does not use generics.</param>
        public OverridableAttribute(CsTag<TConceptType> attributeTag, string attributeName, Func<TAttributeValue, TAttributeValue, bool> overrideOldValue, Func<string, TAttributeValue> valueParser = null)
        {
            _attributeTag = attributeTag;
            _attributeName = attributeName;
            _overrideOldValue = overrideOldValue;
            _overrideOldAttributeTag = "Override" + attributeName;
            _valueParser = valueParser;
        }

        private string AttributeKey(TConceptType conceptInfo)
        {
            return _attributeName + " : " + conceptInfo.GetKeyProperties();
        }

        public void InsertOrOverrideAttribute(ICodeBuilder codeBuilder, TConceptType conceptInfo, TAttributeValue value, string attributeParameters)
        {
            string attributeKey = AttributeKey(conceptInfo);

            object oldValue;
            if (OldAttributeValue.TryGetValue(attributeKey, out oldValue))
            {
                if (!(oldValue is TAttributeValue))
                    throw new FrameworkException("The same attibute is already defined in some other class with a different type " + oldValue.GetType().Name + ". (" + ToString() + ")");

                if (_overrideOldValue((TAttributeValue)oldValue, value))
                    codeBuilder.ReplaceCode("//", _overrideOldAttributeTag.Evaluate(conceptInfo));
                else
                {
                    codeBuilder.InsertCode(AttributeSnippet(conceptInfo, attributeParameters, commented: true), _attributeTag, conceptInfo);
                    return;
                }
            }

            codeBuilder.InsertCode(AttributeSnippet(conceptInfo, attributeParameters), _attributeTag, conceptInfo);
            OldAttributeValue[attributeKey] = value;
        }

        void IOverridableAttribute.InsertOrOverrideAttribute(ICodeBuilder codeBuilder, IConceptInfo conceptInfo, string value, string attributeParameters)
        {
            if (_valueParser == null)
                throw new FrameworkException("Cannot use IOverridableAttribute interface if valueParser constructor property is not set. (" + ToString() + ")");
            InsertOrOverrideAttribute(codeBuilder, (TConceptType)conceptInfo, _valueParser(value), attributeParameters);
        }

        public override string ToString()
        {
            return "OverridableAttribute<" + typeof(TAttributeValue).Name + ">(" + _attributeName +")";
        }

        private string AttributeSnippet(TConceptType conceptInfo, string attributeParameters, bool commented = false)
        {
            int sourceAlignment = conceptInfo is PropertyInfo ? 8 : 4;

            return string.Format("{0}[{1}({2})]\r\n{3}",
                commented ? "//" : _overrideOldAttributeTag.Evaluate(conceptInfo),
                _attributeName,
                attributeParameters,
                new string(' ', sourceAlignment));
        }
    }
}
