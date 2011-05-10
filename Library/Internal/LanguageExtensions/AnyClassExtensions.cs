#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IComparable ConvertToIComparable(this Object myObject, Type myConvertType)
        {
            return (IComparable) Convert.ChangeType(myObject, myConvertType);
        }

    }
}
