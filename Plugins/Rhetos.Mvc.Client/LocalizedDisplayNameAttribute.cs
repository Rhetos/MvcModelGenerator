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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rhetos.Mvc
{
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
                    return (string)resourceProperty.GetValue(null, null);
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
