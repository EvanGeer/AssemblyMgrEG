using AssemblyMgrCore.Extensions;
using AssemblyMgrCore.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    public class StringTests
    {
        private const string simpleString = "This is a Testing String";

        [SetUp]
        public void Setup()
        {

        }

        [TestCase(simpleString, "IS", StringComparison.InvariantCultureIgnoreCase, ExpectedResult = true)]
        [TestCase(simpleString, "IS", StringComparison.Ordinal, ExpectedResult = false)]
        [TestCase(simpleString, "this is a testing string", StringComparison.Ordinal, ExpectedResult = false)]
        [TestCase(simpleString, "this is a testing string", StringComparison.InvariantCultureIgnoreCase, ExpectedResult = true)]
        // ToDo: add more cases to capture everything... this is a pretty basic method, but being comprehensive would be good here
        public bool ContainsComparison(string stringToSearch, string searchText, StringComparison stringComparison)
        {
            return stringToSearch.Contains(searchText, stringComparison);
        }
    }
}