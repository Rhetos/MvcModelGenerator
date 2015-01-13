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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhetos.Dsl;
using Rhetos.MvcModelGenerator.DefaultConcepts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhetos.MvcModelGenerator.Test
{
    class MockDslModel : IDslModel
    {
        public IEnumerable<IConceptInfo> Concepts
        {
            get { throw new NotImplementedException(); }
        }

        public IConceptInfo FindByKey(string conceptKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IConceptInfo> FindByType(Type conceptType)
        {
            throw new NotImplementedException();
        }

        public T GetIndex<T>() where T : IDslModelIndex
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class CamelCaseCaptionTest
    {
        [TestMethod]
        public void CamelCase()
        {
            Test(new Dictionary<string, string>
            {
                { "CamelCase", "Camel case" },
                { "Word", "Word" },
                { "SingleLetterX", "Single letter x" },
                { "SingleXLetter", "Single x letter" },
                { "XSingleLetter", "X single letter" },
                { "X", "X" },
            });
        }

        [TestMethod]
        public void Acronym()
        {
            Test(new Dictionary<string, string>
            {
                { "ACRONYM", "ACRONYM" },
            });
        }

        [TestMethod]
        public void Other()
        {
            Test(new Dictionary<string, string>
            {
                { "small", "small" },
                { "sp ace", "sp ace" },
                { "under_score", "under score" },
                { "_underscore", "underscore" },
            });
        }

        private static void Test(Dictionary<string, string> tests)
        {
            var numeratedTest = tests.Select((test, i) => new { Num = i.ToString(), In = test.Key, Out = test.Value });
            var captionsValue = numeratedTest.ToDictionary(test => test.Num, test => test.In);
            new CamelCaseCaption().UpdateCaption(new MockDslModel(), captionsValue);

            foreach (var test in numeratedTest)
            {
                Console.WriteLine("{0}: '{1}' -> '{2}'", test.Num, test.In, captionsValue[test.Num]);
                Assert.AreEqual(test.Out, captionsValue[test.Num]);
            }
        }
    }
}
