using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.LanguageExtensions
{
    public static class ListExtensions
    {
        #region List<T>

        public static List<T> Clone<T>(this List<T> myList)
        {
            T[] _Array = new T[myList.Count];
            myList.CopyTo(_Array);
            return _Array.ToList<T>();
        }

        public static String ToContentString<T>(this List<T> myList)
        {

            var _ReturnValue = new StringBuilder();

            foreach (var _Item in myList)
                _ReturnValue.Append(_Item + ", ");

            // Cut the last ", " off
            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        #endregion
    }
}
