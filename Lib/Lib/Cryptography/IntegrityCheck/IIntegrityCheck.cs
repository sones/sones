/*
 * IIntegrityCheck - The Interface for all cryptographical secure hashes
 * (c) Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 *      Daniel Kirstenpfad
 * 
 * 
 * ToDo:
 *  - Check http://www.bouncycastle.org/csharp/index.html
 * 
 * */

#region Usings

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// The Interface for all cryptographical secure hashes
    /// </summary>

    public interface IIntegrityCheck
    {

        int     HashSize { get; }
        int     HashStringLength { get; }

        String  GetHashValue(String ClearText);
        String  GetHashValue(byte[] ClearText);

        Byte[]  GetHashValueAsByteArray(String ClearText);
        Byte[]  GetHashValueAsByteArray(byte[] ClearText);

        Boolean CheckHashValue(String ClearText, String HashValue);
        Boolean CheckHashValue(byte[] ClearText, byte[] HashValue);
        Boolean CheckHashValue(byte[] ClearText, String HashValue);
        Boolean CheckHashValue(String ClearText, byte[] HashValue);

    }

}