using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyMgrShared.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string stringToSearch, string searchText, StringComparison stringComparison)
            => stringToSearch.IndexOf(searchText, stringComparison) != -1;
    }
}
