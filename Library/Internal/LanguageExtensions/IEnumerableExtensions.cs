using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.LanguageExtensions
{
    public static class IEnumerableExtensions
    {
        #region CountIsGreater

        /// <summary>
        /// Checks whether a Enumerable has more than a certain number of elements   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myIEnumerable">The given Enumerable</param>
        /// <param name="myNumberOfElements">The given number of elements</param>
        /// <returns></returns>
        public static Boolean CountIsGreater<T>(this IEnumerable<T> myIEnumerable, Int32 myNumberOfElements)
        {
            if (myIEnumerable == null)
                return false;

            var rator = myIEnumerable.GetEnumerator();

            while (myNumberOfElements > 0 && rator.MoveNext())
                myNumberOfElements--;

            return (myNumberOfElements == 0 && rator.MoveNext());
        }

        #endregion


        #region Exists        

        public static Boolean Exists<T>(this IEnumerable<T> myIEnumerable, Func<T, Boolean> match)
        {
            return myIEnumerable.Any(match);
        }

        #endregion


        #region  IsNullOrEmpty

        /// <summary>
        /// Checks whether a Enumerable is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myEnumerable">The given Enumerable</param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> myEnumerable)
        {
            if (myEnumerable == null || !myEnumerable.CountIsGreater(0))
                return true;

            return false;
        }

        #endregion


        #region IsNotNullOrEmpty

        /// <summary>
        /// Checks whether a Enumerable is not null or not empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myEnumerable">The given Enumerable</param>
        /// <returns></returns>
        public static Boolean IsNotNullOrEmpty<T>(this IEnumerable<T> myEnumerable)
        {

            if (myEnumerable == null || !myEnumerable.CountIsGreater(0))
                return false;

            return true;

        }

        #endregion


        /// <summary>
        /// Generated from the values ​​of an enumerable a string with a separator character.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="myEnumerable">The given enumerable</param>
        /// <param name="mySeperator">The separator character</param>
        /// <returns></returns>
        public static String ToAggregatedString<T>(this IEnumerable<T> myEnumerable, Char mySeperator = ' ')
        {

            if (myEnumerable == null || myEnumerable.Count() == 0)
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _Item in myEnumerable)
            {
                _StringBuilder.Append(_Item);
                _StringBuilder.Append(mySeperator);
            }
            _StringBuilder.Length = _StringBuilder.Length - 1;

            return _StringBuilder.ToString();

        }

        /// <summary>
        /// Generated from the values ​​of an enumerable a string with a separator character.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="myEnumerable">The given enumerable</param>
        /// <param name="myStringRepresentation"></param>
        /// <param name="mySeperator">The separator character</param>
        /// <returns></returns>
        public static String ToAggregatedString<T>(this IEnumerable<T> myEnumerable, Func<T, String> myStringRepresentation = null, String mySeperator = ", ")
        {

            if (myEnumerable == null || myEnumerable.Count() == 0)
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _Item in myEnumerable)
            {
                if (myStringRepresentation != null)
                {
                    _StringBuilder.Append(myStringRepresentation(_Item) + mySeperator);
                }
                else
                {
                    _StringBuilder.Append(_Item + mySeperator);
                }
            }
            _StringBuilder.Length = _StringBuilder.Length - mySeperator.Length;

            return _StringBuilder.ToString();

        }

    }
}
