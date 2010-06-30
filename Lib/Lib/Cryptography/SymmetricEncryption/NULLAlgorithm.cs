/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/*
 * NULLAlgorithm - Encrypts and decrypts a bunch of bytes using the NULLAlgorithm algorithm
 *        This means this class does actually nothing at all ;)
 *        
 * Achim Friedland, 2008 - 2009
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
