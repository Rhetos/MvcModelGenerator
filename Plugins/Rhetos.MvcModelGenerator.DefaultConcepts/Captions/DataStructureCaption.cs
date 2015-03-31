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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    /// <summary>
    /// Set the default caption value for supported data structures. <see cref="DataStructureCodeGenerator.IsSupported"/>
    /// The caption may be overridden by implementing the ICaptionsValuePlugin using larger Order value.
    /// </summary>
    [Export(typeof(ICaptionsValuePlugin))]
    public class DataStructureCaption : ICaptionsValuePlugin
    {
        public static double Order = -100;
        double ICaptionsValuePlugin.Order { get { return Order; } }

        public static string GetCaptionResourceKey(DataStructureInfo info)
        {
            return info.Module.Name + "_" + info.Name;
        }

        public void UpdateCaption(IDslModel dslModel, IDictionary<string, string> captionsValue)
        {
            foreach (var dataStructure in dslModel.FindByType<DataStructureInfo>())
                captionsValue[GetCaptionResourceKey(dataStructure)] = dataStructure.Name;
        }
    }
}
