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

using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using Rhetos.MvcModelGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    [Export(typeof(ICaptionsValuePlugin))]
    public class CamelCaseCaption : ICaptionsValuePlugin
    {
        public double Order
        {
            get { return 0; }
        }

        public void UpdateCaption(IDslModel dslModel, IDictionary<string, string> captionsValue)
        {
            var captionsKeys = captionsValue.Keys.ToList();
            foreach (var captionKey in captionsKeys)
            {
                StringBuilder sb = new StringBuilder();

                var field = captionsValue[captionKey].ToCharArray();
                sb.Append(field[0]);
                for (int i = 1; i < field.Length; i++)
                {
                    if (field[i] == '_') { sb.Append(' '); continue; }
                    if (char.IsUpper(field[i]) && char.IsLower(field[i - 1])) sb.Append(' ');
                    sb.Append(char.ToLower(field[i]));
                }

                captionsValue[captionKey] = sb.ToString();
            }
        }
    }
}
