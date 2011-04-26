using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.LanguageExtensions
{
    public static class ArrayExtensions
    {
        public static UInt64 ULongLength(this Array myArray)
        {
            if (myArray == null) return 0;
            return (UInt64)myArray.LongLength;
        }
    }
}
