/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/*
 * Extension Class for several things
 * (c) Stefan Licht, 2009-2010
 * Achim Friedland, 2009-2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using sones.Lib.DataStructures.UUID;
using System.Collections;


#endregion

namespace sones.Lib
{
    public static class Extensions
    {

        #region String

        #region SpacingLeft(SpacingWidth)

        public static String SpacingLeft(this String TargetString, Int32 SpacingWidth)
        {
            return TargetString.PadLeft(TargetString.Length + SpacingWidth);
        }

        #endregion

        #region Firststring(length)

        public static String Firststring(this String currentString, Int32 length)
        {
            if (currentString.Length > length)
                return currentString.Substring(0, length);
            else
                return currentString;
        }

        #endregion

        #region ToBase64(myString)

        public static String ToBase64(this String myString)
        {

            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(myString));
            }

            catch(Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }

        }

        #endregion

        #region FromBase64(myBase64String)

        public static String FromBase64(this String myBase64String)
        {

            try
            {

                var _UTF8Decoder    = new UTF8Encoding().GetDecoder();
                var _Bytes          = Convert.FromBase64String(myBase64String);
                var _DecodedChars   = new Char[_UTF8Decoder.GetCharCount(_Bytes, 0, _Bytes.Length)];
                _UTF8Decoder.GetChars(_Bytes, 0, _Bytes.Length, _DecodedChars, 0);

                return new String(_DecodedChars);

            }

            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }

        }

        #endregion

        #region IsNullOrEmpty

        public static Boolean IsNullOrEmpty(this String myString)
        {
            return String.IsNullOrEmpty(myString);
        }

        #endregion

        #region EscapeForXMLandHTML(myString)

        public static String EscapeForXMLandHTML(this String myString)
        {

            myString = myString.Replace("<", "&lt;");
            myString = myString.Replace(">", "&gt;");
            myString = myString.Replace("&", "&amp;");

            return myString;

        }

        #endregion

        #endregion

        #region IEnumerable<T>

        public static UInt64 ULongCount<T>(this IEnumerable<T> myIEnumerable)
        {

            if (myIEnumerable == null) return 0;

            var _ReturnValue = myIEnumerable.LongCount();

            return (_ReturnValue >= 0) ? (UInt64) _ReturnValue : 0;

        }

        public static UInt64 ULongCount<T>(this IEnumerable<T> myIEnumerable, Func<T, Boolean> myFunc)
        {

            var _ReturnValue = myIEnumerable.LongCount(myFunc);

            return (_ReturnValue >= 0) ? (UInt64) _ReturnValue : 0;

        }

        public static Boolean Exists<T>(this IEnumerable<T> myIEnumerable, Func<T, Boolean> match)
        {
            return myIEnumerable.Any(match);
        }

        public static Boolean CountIs<T>(this IEnumerable<T> myIEnumerable, Int32 myNumberOfElements)
        {

            if (myIEnumerable == null)
                return false;

            return (myIEnumerable.Count<T>() == myNumberOfElements);
               
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return true if there are more than <paramref name="myNumberOfElements"/> elements in the collection without cycle through all.</summary>
        ///
        /// <remarks>   Stefan, 16.04.2010. </remarks>
        ///
        /// <typeparam name="T">    . </typeparam>
        /// <param name="myIEnumerable">    my i enumerable. </param>
        /// <param name="myNumberOfElements"> Number of elements. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Boolean CountIsGreater<T>(this IEnumerable<T> myIEnumerable, Int32 myNumberOfElements)
        {

            if (myIEnumerable == null)
                return false;

            var rator = myIEnumerable.GetEnumerator();

            while(myNumberOfElements > 0 && rator.MoveNext())
                myNumberOfElements--;

            return (myNumberOfElements == 0 && rator.MoveNext());

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return true if there are equals or more than <paramref name="myNumberOfElements"/> elements in the collection without cycle through all.</summary>
        ///
        /// <remarks>   Stefan, 16.04.2010. </remarks>
        ///
        /// <typeparam name="T">    . </typeparam>
        /// <param name="myIEnumerable">    my i enumerable. </param>
        /// <param name="myNumberOfElements"> Number of elements. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Boolean CountIsGreaterOrEquals<T>(this IEnumerable<T> myIEnumerable, Int32 myNumberOfElements)
        {

            if (myIEnumerable == null)
                return false;
            
            var rator = myIEnumerable.GetEnumerator();

            while (myNumberOfElements > 0 && rator.MoveNext())
                myNumberOfElements--;

            return (myNumberOfElements == 0);

        }

        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> myEnumerable)
        {
            if (myEnumerable == null || !myEnumerable.CountIsGreater(0))
                return true;

            return false;
        }

        public static Boolean IsNotNullOrEmpty<T>(this IEnumerable<T> myEnumerable)
        {

            if (myEnumerable == null || !myEnumerable.CountIsGreater(0))
                return false;

            return true;

        }


        public static String ToAggregatedString<T>(this IEnumerable<T> myEnumerable)
        {

            if (myEnumerable == null || myEnumerable.Count() == 0)
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _Item in myEnumerable)
                _StringBuilder.AppendLine(_Item.ToString());

            return _StringBuilder.ToString();

        } 

        #endregion

        #region Extensions for LINQ aggregates on UInt64

        public static UInt64 Sum(this IEnumerable<UInt64> myIEnumerable)
        {

            var _ReturnValue = 0UL;

            foreach (var _ActualValue in myIEnumerable)
                _ReturnValue += _ActualValue;

            return _ReturnValue;

        }

        #endregion

        #region Array

        public static UInt64 ULongLength(this Array myArray)
        {

            var _ReturnValue = myArray.LongLength;

            return (_ReturnValue >= 0) ? (UInt64) _ReturnValue : 0;

        }

        #endregion

        #region Byte/Byte[]

        #region ULongLength()

        public static UInt64 ULongLength(this Byte myByte)
        {

            var _ReturnValue = myByte.ULongLength();

            return (_ReturnValue >= 0) ? _ReturnValue : 0;

        }

        #endregion


        #region ToUTF8String()

        /// <summary>
        /// converts an UTF8 String into an Byte Array
        /// </summary>
        /// <param name="InputString">UTF8 encoded String</param>
        /// <returns>an byte array containing the byte representation of the UTF8 string</returns>
        public static String ToUTF8String(this Byte[] myByteArray)
        {

            if (myByteArray.Length == 0)
                return "0";

            return Encoding.UTF8.GetString(myByteArray);

        }

        #endregion

        #region ToHexString()

        public static String ToHexString(this Byte[] myByteArray)
        {
            return ToHexString(myByteArray, SeperatorTypes.NONE);
        }

        #endregion

        #region ToHexString(myRemoveLeadingZero)

        /// <summary>
        /// converts an byte Array into the hexadecimal representation concatenated into one string (useful for hashes as string)
        /// </summary>
        /// <param name="myRemoveLeadingZero">Removes an leading 0 if exist [03] -> [3], [00] -> [0]</param>
        /// <returns>the array as a hex-string</returns>
        public static String ToHexString(this Byte[] myByteArray, Boolean myRemoveLeadingZero)
        {
            return ToHexString(myByteArray, SeperatorTypes.NONE, myRemoveLeadingZero);
        }

        #endregion

        #region ToHexString(mySeperators, myRemoveLeadingZero)

        /// <summary>
        /// converts an byte Array into the hexadecimal representation concatenated into one string (useful for hashes as string)
        /// </summary>
        /// <param name="mySeperators"></param>
        /// <param name="myRemoveLeadingZero">Removes an leading 0 if exist [03] -> [3], [00] -> [0]</param>
        /// <returns>the array as a hex-string</returns>
        public static String ToHexString(this Byte[] myByteArray, SeperatorTypes mySeperators, Boolean myRemoveLeadingZero)
        {

            if (myByteArray.Length == 0)
                return "0";

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < myByteArray.Length - 1; i++)
            {
                var singleHex = myByteArray[i].ToString("x2");
                if (myRemoveLeadingZero && singleHex.StartsWith("0"))
                {
                    singleHex = singleHex.Remove(0, 1);
                    //myRemoveLeadingZero = false;
                }
                //else
                //{
                //    myRemoveLeadingZero = false;
                //}

                switch (mySeperators)
                {

                    case SeperatorTypes.NONE:   sBuilder.Append(singleHex); break;
                    case SeperatorTypes.COLON:  sBuilder.Append(singleHex); sBuilder.Append(":"); break;
                    case SeperatorTypes.HYPHEN: sBuilder.Append(singleHex); sBuilder.Append("-"); break;

                }

            }

            var hex = myByteArray[myByteArray.Length - 1].ToString("x2");
            if (myRemoveLeadingZero && hex.StartsWith("0"))
                hex = hex.Remove(0, 1);
            sBuilder.Append(hex);

            // Return the hexadecimal string.
            return sBuilder.ToString();

        }

        #endregion
        
        #region ToHexString(mySeperators)

        /// <summary>
        /// converts an byte Array into the hexadecimal representation concatenated into one string (useful for hashes as string)
        /// </summary>
        /// <param name="mySeperators"></param>
        /// <returns>the array as a hex-string</returns>
        public static String ToHexString(this Byte[] myByteArray, SeperatorTypes mySeperators)
        {
            return myByteArray.ToHexString(mySeperators, false);
        }

        #endregion

        #region CompareByteArray(myByteArray2)

        /// <summary>
        /// Compares two byte arrays bytewise
        /// </summary>
        /// <param name="myArray1">Array 1</param>
        /// <param name="myArray2">Array 2</param>
        /// <returns></returns>
        public static Int32 CompareByteArray(this Byte[] myByteArray, Byte[] myByteArray2)
        {

            if (myByteArray.Length < myByteArray2.Length)
                return -1;

            if (myByteArray.Length > myByteArray2.Length)
                return 1;

            for (int i = 0; i <= myByteArray.Length - 1; i++)
            {

                if (myByteArray[i] < myByteArray2[i])
                    return -1;

                if (myByteArray[i] > myByteArray2[i])
                    return 1;

            }

            return 0;

        }

        #endregion

        #endregion

        #region Stream

        public static UInt64 ULength(this Stream myStream)
        {

            var _ReturnValue = myStream.Length;

            return (_ReturnValue >= 0) ? (UInt64) _ReturnValue : 0;

        }

        #endregion

        #region Type

        /// <summary>
        /// Generate a name until the BasType of type T is reached
        /// </summary>
        /// <typeparam name="T">The basetype</typeparam>
        /// <param name="myType"></param>
        /// <returns></returns>
        public static String FullBaseName<T>(this Type myType)
        {

            if (myType == null)
                return "";

            var _ReturnValue = myType.Name;

            while (myType != null && myType.BaseType != null && myType.BaseType != typeof(T))
            {
                _ReturnValue = String.Concat(myType.BaseType.Name, ".", _ReturnValue);
                myType = myType.BaseType;
            }

            if (myType.BaseType != typeof(T))
                return "";
            
            return _ReturnValue;

        }

        #endregion

        #region HashSet<T>

        public static void AddRange<T>(this HashSet<T> myList, IEnumerable<T> mySetToAdd)
        {
            foreach (var setItem in mySetToAdd)
                myList.Add(setItem);
        }

        #endregion

        #region List<T>

        public static List<T> Clone<T>(this List<T> myList)
        {
            T[] _Array = new T[myList.Count];
            myList.CopyTo(_Array);
            return _Array.ToList<T>();
        }

        public static String ToString<T>(this List<T> myList)
        {

            var _ReturnValue = new StringBuilder("List<T>[" + myList.Count + "] ");

            foreach (var _Items in myList)
                _ReturnValue.Append(_Items + ", ");

            // Cut the last ", " off
            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        //public static void Insert<T>(this List<T> theList, IComparable value)
        //{
        //    int toInserAfter = -1;
        //    toInserAfter = theList.PandoraBinarySearch(value);
        //    theList.Insert(toInserAfter+1, (T)value);
        //}


        //public static int FastIndexOf<T>(this List<T> theList, IComparable value)
        //{
        //    if (theList.Count == 0) return -1;

        //    int medianIdx = (theList.Count - 1) / 2;
        //    IComparable medianValue = (IComparable)theList.ElementAt(medianIdx);

        //    //also very trivial
        //    if (theList.Count == 1)
        //    {
        //        if (medianValue.CompareTo(value) == 0) return 0;
        //        else return -1;
        //    }

        //    //yeah, we got it
        //    if (medianValue.CompareTo(value)==0)
        //        return medianIdx;

        //    //recursion here
        //    else if (medianValue.CompareTo(value) > 0) //median is greater => search in the lower Part
        //    {
        //        return FastIndexOf(theList.GetRange(0, medianIdx), value);

        //    }
        //    //and here
        //    else
        //    {
        //        int partialRecursiveResult = FastIndexOf(theList.GetRange(medianIdx + 1, theList.Count / 2), value);
        //        if (partialRecursiveResult < 0)
        //            return -1;
        //        else
        //            return medianIdx + 1 + partialRecursiveResult;
        //    }
        //}

        //public static int FastLastIndexOf<T>(this List<T> theList, IComparable value)
        //{
        //    int result = theList.FastIndexOf(value);
        //    if (result.CompareTo(-1) == 0) return result;
        //    while (result >= 0 && ((IComparable)theList[result]).CompareTo(value) == 0)
        //        result--;
            
        //    return result + 1;
        //}

        //public static bool FastContains<T>(this List<T> theList, IComparable value)
        //{
        //    //if (typeof(T) is IComparable)
        //    //{
        //        //most trivial case
        //        if (theList.Count == 0) return false;

        //        int medianIdx = (theList.Count - 1) / 2;
        //        IComparable medianValue = (IComparable)theList.ElementAt(medianIdx);

        //        //also very trivial
        //        if (theList.Count == 1)
        //        {
        //            if (theList.Contains((T)value)) return true;
        //            return false;
        //        }

        //        //yeah, we got it
        //        if (medianValue.CompareTo(value)==0)
        //            return true;

        //        //recursion here
        //        else if (medianValue.CompareTo(value) > 0) //median is greater => search in the lower Part
        //        {
        //            return FastContains(theList.GetRange(0, medianIdx), value);
        //        }
        //        //and here
        //        else
        //        {
        //            return FastContains(theList.GetRange(medianIdx + 1, theList.Count / 2), value);
        //        }
        //    //}
        //    //else
        //    //    return false;
        //}

        //public static int PandoraBinarySearch<T>(this List<T> theList, IComparable value)
        //{
        //    //if (typeof(T) is IComparable)
        //    //{
        //        //most trivial case
        //        if (theList.Count == 0) return -1;

        //        int medianIdx = (theList.Count - 1) / 2;
        //        IComparable medianValue = (IComparable)theList.ElementAt(medianIdx);

        //        //also very trivial
        //        if (theList.Count == 1)
        //        {
        //            if (medianValue.CompareTo(value) <= 0) return 0;
        //            if (medianValue.CompareTo(value) > 0) return -1;
        //        }

        //        //yeah, we got it
        //        if (medianValue == value)
        //            return medianIdx;

        //        //recursion here
        //        else if (medianValue.CompareTo(value) > 0) //median is greater => search in the lower Part
        //        {
        //            return PandoraBinarySearch(theList.GetRange(0, medianIdx), value);

        //        }
        //        //and here
        //        else
        //        {
        //            return medianIdx + 1 + PandoraBinarySearch(theList.GetRange(medianIdx + 1, theList.Count / 2), value);

        //        }
        //    //}
        //    //else
        //    //    return -1;
        //}

        #endregion

        #region ConsoleKeyInfo

        public static Boolean IsSpecialKey(this ConsoleKeyInfo myConsoleKeyInfo)
        {
            if (((Byte)myConsoleKeyInfo.KeyChar) >= 32 && ((Byte)myConsoleKeyInfo.KeyChar) <= 126)
                return false;

            return true;
        }

        #endregion

        #region DateTime

        public static String ToStringWithMilliseconds(this DateTime myDateTime)
        {
            return myDateTime.ToString("yyyy-dd-MM HH:mm:ss.fffffff");
        }

        #endregion

        #region Int64

        public static String ToByteFormattedString(this Int64 myNumber, Int32 myDigits)
        {
            return ToByteFormattedString((UInt64)myNumber, myDigits);
        }

        #endregion

        #region UInt64

        public static String ToByteFormattedString(this UInt64 myNumber, Int32 myDigits)
        {

            if (myNumber > 1024UL * 1024 * 1024 * 1024)
                return String.Format("{0} TB", Math.Round((Double)(myNumber / (1024UL * 1024 * 1024 * 1024)), myDigits));

            else if (myNumber > 1024 * 1024 * 1024)
                return String.Format("{0} GB", Math.Round((Double)myNumber / (1024 * 1024 * 1024), myDigits));

            else if (myNumber > 1024 * 1024)
                return String.Format("{0} MB", Math.Round((Double)myNumber / (1024 * 1024), myDigits));

            else if (myNumber > 1024)
                return String.Format("{0} KB", Math.Round((Double)myNumber / (1024), myDigits));

            else
                return String.Format("{0} B", myNumber);

        }

        public static String ToFormatedNumber(this UInt64 myNumber)
        {
            return String.Format("{0:0,0}", myNumber);
        }

        #endregion

        #region UUID

        public static UInt64 ToUInt64(this UUID myUUID)
        {
            return BitConverter.ToUInt64(myUUID.GetByteArray(), 0);
        }

        #endregion

        #region Raise<TEventArgs>(handler, sender, e)

        public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            if (null != handler)
            {
                handler(sender, e);
            }
        }
        
        #endregion

        #region Dictionary

        public static Boolean IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> myDict)
        {
            return (myDict == null) || (myDict.Count == 0);
        }

        #endregion

        #region StringBuilder

        public static void Indent(this StringBuilder stringbuilder, int width, char character = ' ')
        {
            stringbuilder.Append("".PadLeft(width, character));
        }

        /// <summary>
        /// Removes the last characters of the length of <paramref name="delimiter"/> without checking them.
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="delimiter"></param>
        public static void RemoveEnding(this StringBuilder stringbuilder, String delimiter)
        {
            if (stringbuilder.Length > delimiter.Length)
            {
                stringbuilder.Remove(stringbuilder.Length - delimiter.Length, 2);
            }
        }
        #endregion

    }

}
