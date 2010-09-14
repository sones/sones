using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace sones.Lib.Hashing.Functions
{
    public class MD5Hash : IHashFunction
    {
        private static MD5 _md5 = MD5.Create();

        public Int64 Hash(byte[] myData)
        {
            return Hash(myData, myData.Length);
        }

        public Int64 Hash(byte[] myData, int myLength)
        {
            return Hash(myData, myLength, 0);
        }

        public Int64 Hash(byte[] myData, int myLength, uint mySeed)
        {
            byte[] result = _md5.ComputeHash(myData,0 ,myLength);

            return BitConverter.ToInt64(result, 0);
        }
    }
}
