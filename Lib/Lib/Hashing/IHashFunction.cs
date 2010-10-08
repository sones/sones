using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Hashing
{
    public interface IHashFunction
    {
        Int64 Hash(Byte[] myData);

        Int64 Hash(Byte[] myData, Int32 myLength);

        Int64 Hash(Byte[] myData, Int32 myLength, UInt32 mySeed);
    }
}
