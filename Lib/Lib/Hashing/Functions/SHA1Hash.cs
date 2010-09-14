using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace sones.Lib.Hashing.Functions
{
    public class SHA1Hash : IHashFunction
    {
        private static SHA1 _sha1 = SHA1.Create();

        public long Hash(byte[] myData)
        {
            return Hash(myData, myData.Length);
        }

        public long Hash(byte[] myData, int myLength)
        {
            return Hash(myData, myLength, 0);
        }

        public long Hash(byte[] myData, int myLength, uint mySeed)
        {
            byte[] result = _sha1.ComputeHash(myData);

            return BitConverter.ToInt64(result, 0);
        }
    }
}
