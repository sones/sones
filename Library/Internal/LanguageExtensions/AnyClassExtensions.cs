#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

#endregion


namespace sones.Library.LanguageExtensions
{
    public static class AnyClassExtensions
    {
        public static IEnumerable<T> SingleEnumerable<T>(this T mySingleInstance)
        {
            return Enumerable.Repeat(mySingleInstance, 1);
        }

        public static void CheckNull(this Object myObject, String myArgumentName = "")
        {
            if (null == myObject)
                throw new ArgumentNullException(myArgumentName);
        }        
    }
}
