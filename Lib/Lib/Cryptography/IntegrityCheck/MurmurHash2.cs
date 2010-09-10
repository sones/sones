/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/*
 * IIntegrityCheck - MurmurHash2
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using sones.Lib;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// MurmurHash 2.0 - C# Implementation
    /// Original C++ Implementation by Austin Appleby (aappleby (AT) gmail)
    /// http://sites.google.com/site/murmurhash/
    /// MurmurHash is a non-cryptographic hash function suitable for general hash-based lookup.
    /// </summary>
    
    public class MurmurHash2 : IIntegrityCheck
    {

        #region Data

        /// <summary>
        /// mixed constant that "just happen to work well"
        /// </summary>
        private const UInt32 _m = 0x5bd1e995;

        private const Int32 _r = 24;

        /// <summary>
        /// the hash
        /// </summary>
        private UInt32 _HashValue;

        #endregion

        #region Properties

        #region Seed

        public UInt32 Seed { get; set; }

        #endregion

        #region HashSize

        public Int32 HashSize { get { return 0; } }

        #endregion

        #region HashStringLength

        public Int32 HashStringLength { get { return 0; } }

        #endregion

        #endregion

        #region Constructor

        public MurmurHash2()
        {
            Seed = 0xc58f1a7b;
        }

        #endregion
                

        #region GetHashValue(myClearText)

        public String GetHashValue(String myClearText)
        {

            if (myClearText == null || myClearText.Length == 0)
                return String.Empty;

            var _ClearText = Encoding.UTF8.GetBytes(myClearText);

            return Hash(_ClearText, _ClearText.Length).ToString();

        }

        public String GetHashValue(Byte[] myClearText)
        {

            if (!myClearText.Any())
                return String.Empty;

            return Hash(myClearText, myClearText.Length).ToString();

        }

        #endregion

        #region GetHashValueAsByteArray(myClearText)

        public Byte[] GetHashValueAsByteArray(String myClearText)
        {

            if (myClearText == null || myClearText.Length == 0)
                return new Byte[0];

            var _ClearText = Encoding.UTF8.GetBytes(myClearText);

            return BitConverter.GetBytes(Hash(_ClearText, _ClearText.Length));

        }

        public Byte[] GetHashValueAsByteArray(Byte[] myClearText)
        {

            if (!myClearText.Any())
                return new Byte[0];

            return BitConverter.GetBytes(Hash(myClearText, myClearText.Length));

        }

        #endregion

        #region CheckHashValue(...)

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(String myClearText, String myHashValue)
        {
            return GetHashValue(myClearText) == myHashValue;
        }

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(Byte[] myClearText, String myHashValue)
        {
            return GetHashValue(myClearText) == myHashValue;
        }

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(Byte[] myClearText, Byte[] myHashValue)
        {

            if (myHashValue.CompareByteArray(GetHashValueAsByteArray(myClearText)) == 0)
                return true;

            return false;

        }

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(String myClearText, Byte[] myHashValue)
        {

            if (myHashValue.CompareByteArray(GetHashValueAsByteArray(myClearText)) == 0)
                return true;

            return false;

        }

        #endregion


        #region (private) Hash(myData, myLength)

        private Int64 Hash(Byte[] myData, Int32 myLength)
        {

            #region Data

            // Initialize hash to random value
            _HashValue = (UInt32) (myLength ^ Seed);

            // Holds 4 bytes in each hashing loop
            UInt32 currentBytes;

            #endregion

            #region Index is used to step through the byte field

            Int32 fromIndex = 0;
            //start mixing 4 bytes at a time into the hash

            while (myLength >= 4)
            {

                currentBytes = BitConverter.ToUInt32(myData, fromIndex);

                currentBytes *= _m;
                currentBytes ^= currentBytes >> _r;
                currentBytes *= _m;

                _HashValue *= _m;
                _HashValue ^= currentBytes;

                //set fromIndex +4 bytes
                fromIndex += 4;
                //and length -4 bytes
                myLength -= 4;

            }

            #endregion

            #region Some bytes are left.. must be hashed to

            switch (myLength)
            {

                case 3: //3 bytes left
                    //take 2
                    _HashValue ^= BitConverter.ToUInt16(myData, fromIndex);
                    //and 1
                    _HashValue ^= (UInt32)myData[fromIndex + 2] << 16;
                    _HashValue *= _m;
                    break;

                case 2: //2 bytes left
                    _HashValue ^= BitConverter.ToUInt16(myData, fromIndex);
                    _HashValue *= _m;
                    break;

                case 1: // 1 byte left
                    _HashValue ^= myData[fromIndex];
                    _HashValue *= _m;
                    break;

                default:
                    break;

            }

            #endregion

            #region Some final mixes

            _HashValue ^= _HashValue >> 13;
            _HashValue *= _m;
            _HashValue ^= _HashValue >> 15;

            #endregion

            return (Int64) _HashValue;

        }

        #endregion

    }

}
