/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/*
 * MD5 - Generates cryptographical secure hashes based on the MD5 algorithm
 * 
 * (c) Achim Friedland, 2008 - 2009
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 *      Daniel Kirstenpfad
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// Generates cryptographical secure hashes based on the MD5 algorithm
    /// </summary>

    
    public class MD5 : IIntegrityCheck
    {
        #region HashSize
        public int HashSize
        {
            get
            {
                return 16;
            }
        }
        #endregion

        #region HashStringLength
        public int HashStringLength
        {
            get
            {
                return 32;
            }
        }
        #endregion

        #region GetHashValue
        public string GetHashValue(string ClearText)
        {

            if ((ClearText == null) || (ClearText.Length == 0))
            {
                return String.Empty;
            }

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            Byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(ClearText));

            return result.ToHexString(SeperatorTypes.NONE);

        }

        public string GetHashValue(byte[] ClearText)
        {

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            Byte[] result = md5.ComputeHash(ClearText);

            return result.ToHexString(SeperatorTypes.NONE);

        }
        #endregion

        #region GetHashValueAsByteArray
        public byte[] GetHashValueAsByteArray(string ClearText)
        {

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            return md5.ComputeHash(Encoding.UTF8.GetBytes(ClearText));

        }

        public byte[] GetHashValueAsByteArray(byte[] ClearText)
        {

            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(ClearText);
        
        }
        #endregion

        #region CheckHashValue
        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public bool CheckHashValue(string ClearText, string HashValue)
        {
            return (GetHashValue(ClearText) == HashValue);
        }

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public bool CheckHashValue(byte[] ClearText, string HashValue)
        {
            return (GetHashValue(ClearText) == HashValue);
        }

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public bool CheckHashValue(byte[] ClearText, byte[] HashValue)
        {

            if (HashValue.CompareByteArray(GetHashValueAsByteArray(ClearText)) == 0)
                return true;

            return false;

        }

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public bool CheckHashValue(string ClearText, byte[] HashValue)
        {

            if (HashValue.CompareByteArray(GetHashValueAsByteArray(ClearText)) == 0)
                return true;

            return false;

        }


        #endregion


    }

}
