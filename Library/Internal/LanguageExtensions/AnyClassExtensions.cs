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
    }

    public static class ByteArrayExtension
    {
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
    }

    public static class StreamExtension
    {
        #region Stream

        public static UInt64 ULength(this Stream myStream)
        {

            var _ReturnValue = myStream.Length;

            return (_ReturnValue >= 0) ? (UInt64)_ReturnValue : 0;

        }

        #endregion
    }
}
