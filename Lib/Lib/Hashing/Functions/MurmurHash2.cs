/*
 * MurmurHash 2.0 - C# Implementation
 * 
 * Original C++ Implementation by Austin Appleby (aappleby (AT) gmail)
 * http://sites.google.com/site/murmurhash/
 * 
 * MurmurHash is a non-cryptographic hash function suitable for general hash-based lookup.
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Lib.Hashing
{

    public class MurmurHash2 : IHashFunction
    {

        /// <summary>
        /// mixed constant that "just happen to work well"
        /// </summary>
        private const UInt32 _m = 0x5bd1e995;

        private const Int32 _r = 24;

        /// <summary>
        /// the hash
        /// </summary>
        private UInt32 _h;

        public Int64 Hash(Byte[] myData)
        {
            return Hash(myData, myData.Length);
        }

        public Int64 Hash(Byte[] myData, Int32 myLength)
        {
            return Hash(myData, myLength, 0xc58f1a7b);
        }

        public Int64 Hash(byte[] myData, Int32 myLength, UInt32 mySeed)
        {

            #region data

            //initialize hash to random value
            _h = (UInt32)(myLength ^ mySeed);

            //holds 4 bytes in each hashing loop
            UInt32 currentBytes;

            #endregion

            //index is used to step through the byte field
            int fromIndex = 0;
            //start mixing 4 bytes at a time into the hash
            while (myLength >= 4)
            {
                currentBytes = BitConverter.ToUInt32(myData, fromIndex);

                currentBytes *= _m;
                currentBytes ^= currentBytes >> _r;
                currentBytes *= _m;

                _h *= _m;
                _h ^= currentBytes;

                //set fromIndex +4 bytes
                fromIndex += 4;
                //and length -4 bytes
                myLength -= 4;
            }

            //some bytes are left.. must be hashed to
            switch (myLength)
            {
                case 3: //3 bytes left
                    //take 2
                    _h ^= BitConverter.ToUInt16(myData, fromIndex);
                    //and 1
                    _h ^= (UInt32)myData[fromIndex + 2] << 16;
                    _h *= _m;
                    break;
                case 2: //2 bytes left
                    _h ^= BitConverter.ToUInt16(myData, fromIndex);
                    _h *= _m;
                    break;
                case 1: // 1 byte left
                    _h ^= myData[fromIndex];
                    _h *= _m;
                    break;
                default:
                    break;
             
            }

            //some final mixes

            _h ^= _h >> 13;
            _h *= _m;
            _h ^= _h >> 15;

            return (Int64)_h;
        }
    }

}
