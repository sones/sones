using System.Linq;
using System.Collections.Generic;

namespace sones.Library.LanguageExtensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SingleEnumerable<T>(this T mySingleInstance)
        {
            return Enumerable.Repeat(mySingleInstance, 1);
        }

    }
}
