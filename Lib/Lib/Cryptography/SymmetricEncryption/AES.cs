/*
 * AES - Encrypts and decrypts a bunch of bytes using the AES algorithm
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
using System.IO;
using System.Security.Cryptography;

using sones.Lib.Cryptography.SymmetricEncryption;

#endregion

namespace sones.Lib.Cryptography.SymmetricEncryption
{

    /// <summary>
    /// Encrypts and decrypts a bunch of bytes using the AES algorithm
    /// </summary>

    
    class AES : ISymmetricEncryption
    {


        #region encrypt(plaintext, cryptoKey, cryptoIV)

        /// <summary>
        /// Encrypts the given plaintext using the given key and IV
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="cryptoKey"></param>
        /// <param name="cryptoIV"></param>
        /// <returns></returns>
        public byte[] encrypt(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV)
        {

            // Check arguments.
            if (plaintext == null || plaintext.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (cryptoKey == null || cryptoKey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (cryptoIV  == null || cryptoIV.Length  <= 0)
                throw new ArgumentNullException("Key");

            // Declare the streams used
            // to encrypt to an in memory
            // array of bytes.
            MemoryStream    msEncrypt     = null;
            CryptoStream    csEncrypt     = null;
            StreamWriter    swEncrypt     = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged AESAlgorithm  = null;

            // Declare the bytes used to hold the
            // encrypted data.

            try
            {

                // CreateStorage a RijndaelManaged object using the specified key and IV
                AESAlgorithm      = new RijndaelManaged();
                AESAlgorithm.Key  = cryptoKey;
                AESAlgorithm.IV   = cryptoIV;

                // It is reasonable to set encryption mode to Cipher Block Chaining
                // (CBC). Use default options for other symmetric key parameters.
                AESAlgorithm.Mode = CipherMode.CBC;

                // CreateStorage a decryptor to perform the stream transform.
                ICryptoTransform encryptor = AESAlgorithm.CreateEncryptor(AESAlgorithm.Key, AESAlgorithm.IV);

                // CreateStorage the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                // Start encrypting.
                csEncrypt.Write(plaintext, 0, plaintext.Length);

            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();

                // Clear the RijndaelManaged object.
                if (AESAlgorithm != null)
                    AESAlgorithm.Clear();
            }

            // Finish encrypting.
            csEncrypt.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = msEncrypt.ToArray();

            // Convert encrypted data into a base64-encoded string.
//            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return the encrypted bytes from the memory stream.
            return cipherTextBytes;

        }

        #endregion

        #region encryptToBase64(plaintext, cryptoKey, cryptoIV)

        /// <summary>
        /// Encrypts the given plaintext using the given key and IV
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="cryptoKey"></param>
        /// <param name="cryptoIV"></param>
        /// <returns>a base64 encoded string</returns>
        public String encryptToBase64(byte[] plaintext, byte[] cryptoKey, byte[] cryptoIV)
        {

            return Convert.ToBase64String( encrypt(plaintext, cryptoKey, cryptoIV) );

        }

        #endregion


        #region decrypt(cipherText, cryptoKey, cryptoIV)

        /// <summary>
        /// Decrypts the given plaintext using the given key and IV
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="cryptoKey"></param>
        /// <param name="cryptoIV"></param>
        /// <returns></returns>
        public byte[] decrypt(byte[] ciphertext, byte[] Key, byte[] IV)
        {

            // Check arguments.
            if (ciphertext == null || ciphertext.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // TDeclare the streams used
            // to decrypt to an in memory
            // array of bytes.
            MemoryStream    msDecrypt   = null;
            CryptoStream    csDecrypt   = null;
            StreamReader    srDecrypt   = null;

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg      = null;

            // Declare the string used to hold
            // the decrypted text.
//            string plaintext = null;

            try
            {
                // CreateStorage a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // CreateStorage a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // CreateStorage the streams used for decryption.
                msDecrypt = new MemoryStream(ciphertext);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
//                plaintext = srDecrypt.ReadToEnd();


                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold ciphertext;
                // plaintext is never longer than ciphertext.
                byte[] plaintext = new byte[ciphertext.Length];

                // Start decrypting.
                int decryptedByteCount = csDecrypt.Read(plaintext,
                                                           0,
                                                           plaintext.Length);

                // Convert decrypted data into a string. 
                // Let us assume that the original plaintext string was UTF8-encoded.
//                plaintext = Encoding.UTF8.GetString(plainTextBytes,
//                                                           0,
//                                                           decryptedByteCount);

            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            

            // Return decrypted string.   
            return ciphertext;

        }

        #endregion

        #region decryptToBase64(cipherText, cryptoKey, cryptoIV)

        /// <summary>
        /// Decrypts the given plaintext using the given key and IV
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="cryptoKey"></param>
        /// <param name="cryptoIV"></param>
        /// <returns>a base64 encoded string</returns>
        public String decryptToBase64(byte[] ciphertext, byte[] cryptoKey, byte[] cryptoIV)
        {

            return Convert.ToBase64String( decrypt(ciphertext, cryptoKey, cryptoIV) );

        }

        #endregion







    }

}
