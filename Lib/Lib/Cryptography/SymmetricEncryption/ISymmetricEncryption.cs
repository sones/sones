/*
 * ISymmetricEncryption - The Interface for all symmetric encryption algorithms
 * (c) Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
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

namespace sones.Lib.Cryptography.SymmetricEncryption
{

    /// <summary>
    /// The Interface for all symmetric encryption algorithms
    /// </summary>

    public interface ISymmetricEncryption
    {

        byte[] encrypt(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV);
        String encryptToBase64(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV);

        byte[] decrypt(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV);
        String decryptToBase64(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV);

    }

}