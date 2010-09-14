/*
 * IntegrityCheckTypes
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

namespace sones.Lib.Cryptography.IntegrityCheck
{

    
    public enum IntegrityCheckTypes : ushort
    {
        NULLAlgorithm   = 0,        // This means no crypto hash algorithm is used
        MD5             = 1,
        SHA1            = 2,
    }

}
