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
 * NULLAlgorithm - Generates cryptographical secure hashes based on the NULLAlgorithm algorithm
 *        This means this class does actually nothing at all ;)
 *        
 * (c) Achim Friedland, 2008 - 2009
 * (c) Daniel Kirstenpfad 2009
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

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// Generates cryptographical secure hashes based on the NULLAlgorithm algorithm
    /// </summary>

    
    public class NULLAlgorithm : IIntegrityCheck
    {

        #region HashSize

        public int HashSize
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region HashStringLength
        public int HashStringLength
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region GetHashValue

        public string GetHashValue(string ClearText)
        {
            return String.Empty;
        }

        public string GetHashValue(byte[] ClearText)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetHashValueAsByteArray

        public byte[] GetHashValueAsByteArray(string ClearText)
        {
            throw new NotImplementedException();
        }

        public byte[] GetHashValueAsByteArray(byte[] ClearText)
        {
            return new Byte[0];
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
