/*
 * SymmetricEncryptionTypes
 * (c) Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Using

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.Lib.Cryptography.SymmetricEncryption
{

    
    public enum SymmetricEncryptionTypes : ushort
    {
        NULLAlgorithm   = 0,        // This means no crypto hash algorithm is used
        AES             = 1,
    }

}
