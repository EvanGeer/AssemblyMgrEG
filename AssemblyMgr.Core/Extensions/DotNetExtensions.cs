using System;
using System.Linq;

namespace AssemblyMgr.Core.Extensions
{
    public static class DotNetExtensions
    {
        public static bool Contains(this string stringToSearch, string searchText, StringComparison stringComparison)
            => stringToSearch.IndexOf(searchText, stringComparison) != -1;

        public static string GetAttribute<TEnum, TAttribute>(this TEnum value, Func<TAttribute, string> getter)
            where TEnum : Enum
            where TAttribute : Attribute
        {
            var descriptionAttribute = typeof(TEnum)
              .GetField(value.ToString())
              ?.GetCustomAttributes(typeof(TAttribute), false)
              ?.FirstOrDefault() as TAttribute;


            return descriptionAttribute != null && getter != null
              ? getter.Invoke(descriptionAttribute)
              : value.ToString();

        }

    }
}
