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

    }
}
