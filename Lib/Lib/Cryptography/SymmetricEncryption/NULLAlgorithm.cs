/*
 * NULLAlgorithm - Encrypts and decrypts a bunch of bytes using the NULLAlgorithm algorithm
 *        This means this class does actually nothing at all ;)
 *        
 * (c) Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using sones.Lib.Cryptography.SymmetricEncryption;

#endregion

namespace sones.Lib.Cryptography.SymmetricEncryption
{

    /// <summary>
    /// Encrypts and decrypts a bunch of bytes using the NULLAlgorithm algorithm
    /// </summary>

    

    public class NULLAlgorithm : ISymmetricEncryption
    {

        #region encrypt(plaintext, cryptoKey, cryptoIV)

        public byte[] encrypt(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV)
        {
            return plaintext;
        }

        #endregion

        #region encryptToBase64(plaintext, cryptoKey, cryptoIV)

        public String encryptToBase64(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV)
        {
            return Convert.ToBase64String(plaintext);
        }

        #endregion


        #region decrypt(ciphertext, cryptoKey, cryptoIV)

        public byte[] decrypt(byte[] ciphertext, byte[] cryptoKey, byte[] cryptoIV)
        {
            return ciphertext;
        }

        #endregion

        #region decrypt(ciphertext, cryptoKey, cryptoIV)

        public String decryptToBase64(byte[] ciphertext, byte[] cryptoKey, byte[] cryptoIV)
        {
            return Convert.ToBase64String(ciphertext);
        }

        #endregion

    }

}
