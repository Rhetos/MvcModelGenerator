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

namespace Rhetos.MvcModelGenerator
{
    public class MvcModelInitialCodeGenerator : IMvcModelGeneratorPlugin
    {
        public const string UsingTag = "/*using*/";
        public const string ModuleMembersTag = "/*implementation*/";
        public const string NamespaceMembersTag = "/*body*/";
        
        public const string RhetosMvcNamespace = "Rhetos.Mvc";

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            codeBuilder.InsertCode(CodeSnippet);

            codeBuilder.AddReferencesFromDependency(typeof(Guid));
            codeBuilder.AddReferencesFromDependency(typeof(System.Linq.Enumerable));
        }

        private string CodeSnippet =
@"
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
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

namespace " + RhetosMvcNamespace + @"
{

    public partial class BaseMvcModel
    {
         public Guid ID { get; set; }
    }

    " + NamespaceMembersTag + @"

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed partial class MinValueIntegerAttribute : ValidationAttribute
    {
        public string MinValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) >= Convert.ToInt32(MinValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed  partial class MinValueDecimalAttribute : ValidationAttribute
    {
        public string MinValue { get; set; }

        public override bool IsValid(object value)
        {
            return  Convert.ToDecimal(value) >= Convert.ToDecimal(MinValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed partial class MinValueDateTimeAttribute : ValidationAttribute
    {
        public string MinValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToDateTime(value) >= Convert.ToDateTime(MinValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed partial class MaxValueIntegerAttribute : ValidationAttribute
    {
        public string MaxValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) <= Convert.ToInt32(MaxValue);
        }
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed partial class MaxValueDecimalAttribute : ValidationAttribute
    {
        public string MaxValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToDecimal(value) <= Convert.ToDecimal(MaxValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed partial class MaxValueDateTimeAttribute : ValidationAttribute
    {
        public string MaxValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToDateTime(value) <= Convert.ToDateTime(MaxValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed partial class RenderModeAttribute : Attribute
    {
        private RenderMode _renderMode = RenderMode.Any;
        public RenderMode RenderMode { 
            get{
                return _renderMode;   
            }
            set {
                _renderMode = value;
            }
        }

        public RenderModeAttribute(RenderMode renderMode)
        {
            _renderMode = renderMode;
        }
    }

    public enum RenderMode
    {
        Any,
        EditModeOnly,
        DisplayModeOnly,
        None
    }

    public enum LookupType
    {
        DropDown,
        AutoComplete,
        ComboBox
    }

    public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly PropertyInfo resourceProperty;

        public LocalizedDisplayNameAttribute(string displayNameKey, Type resourceType = null)
            : base(displayNameKey)
        {
            if (resourceType != null)
                resourceProperty = resourceType.GetProperty(displayNameKey, BindingFlags.Static | BindingFlags.Public);
        }

        public override string DisplayName
        {
            get
            {
                if (resourceProperty == null)
                    return base.DisplayName;

                try
                {
                    return (string)resourceProperty.GetValue(null);
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    if (ex.InnerException is System.Resources.MissingManifestResourceException)
                        throw ex.InnerException;
                    throw;
                }
            }
        }
    }
}

" + ModuleMembersTag + @"

";
    }
}