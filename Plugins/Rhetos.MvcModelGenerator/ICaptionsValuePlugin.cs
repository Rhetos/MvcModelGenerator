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
using System.Collections.Generic;

namespace Rhetos.MvcModelGenerator
{
    public interface ICaptionsValuePlugin
    {
        /// <summary>
        /// Defines plugin priority.
        /// The plugins that implement the same concept will be executed in specified order.
        /// </summary>
        double Order { get; }

        void UpdateCaption(IDslModel dslModel, IDictionary<string, string> captionsValue);
    }
}
