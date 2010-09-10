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
 * IIntegrityCheck - NULLAlgorithm
 * (c) Achim Friedland, 2008-2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// Generates cryptographical secure hashes based on the NULLAlgorithm algorithm
    /// This means this class does actually nothing at all ;)
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

        public Int32 HashStringLength
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region GetHashValue(myClearText)

        public String GetHashValue(String myClearText)
        {
            return myClearText;
        }

        public String GetHashValue(Byte[] myClearText)
        {
            return Encoding.UTF8.GetString(myClearText);
        }

        #endregion

        #region GetHashValueAsByteArray(myClearText)

        public Byte[] GetHashValueAsByteArray(String myClearText)
        {
            return Encoding.UTF8.GetBytes(myClearText);
        }

        public Byte[] GetHashValueAsByteArray(Byte[] myClearText)
        {
            return myClearText;
        }

        #endregion

        #region CheckHashValue

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(String myClearText, String myHashValue)
        {
            return (GetHashValue(myClearText) == myHashValue);
        }

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(Byte[] myClearText, String myHashValue)
        {
            return (GetHashValue(myClearText) == myHashValue);
        }

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(Byte[] myClearText, Byte[] myHashValue)
        {

            if (myHashValue.CompareByteArray(GetHashValueAsByteArray(myClearText)) == 0)
                return true;

            return false;

        }

        /// <summary>
        /// Checks if a Hash Value matches a given Cleartext
        /// </summary>
        /// <param name="ClearText">the text that needs to be checked</param>
        /// <param name="HashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(String myClearText, Byte[] myHashValue)
        {

            if (myHashValue.CompareByteArray(GetHashValueAsByteArray(myClearText)) == 0)
                return true;

            return false;

        }

        #endregion

    }

}
