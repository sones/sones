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
 * AIntegrityCheck
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;

#endregion

namespace sones.Lib.Cryptography.IntegrityCheck
{

    /// <summary>
    /// Abstract class for generating cryptographical secure hashes
    /// </summary>

    public abstract class AIntegrityCheck : IIntegrityCheck
    {

        #region Data

        protected System.Security.Cryptography.HashAlgorithm _Hasher;

        #endregion

        #region Properties

        #region HashSize

        public abstract Int32 HashSize { get; }

        #endregion

        #region HashStringLength

        public abstract Int32 HashStringLength { get; }

        #endregion

        #endregion

        #region Constructor

        public AIntegrityCheck()
        {
        }

        #endregion
                

        #region GetHashValue(myClearText)

        public String GetHashValue(String myClearText)
        {

            if (myClearText == null || myClearText.Length == 0)
                return String.Empty;

            return _Hasher.ComputeHash(Encoding.UTF8.GetBytes(myClearText)).ToHexString(SeperatorTypes.NONE);

        }

        public String GetHashValue(Byte[] myClearText)
        {

            if (!myClearText.Any())
                return String.Empty;

            return _Hasher.ComputeHash(myClearText).ToHexString(SeperatorTypes.NONE);

        }

        #endregion

        #region GetHashValueAsByteArray(myClearText)

        public Byte[] GetHashValueAsByteArray(String myClearText)
        {

            if (myClearText == null || myClearText.Length == 0)
                return new Byte[0];

            return _Hasher.ComputeHash(Encoding.UTF8.GetBytes(myClearText));

        }

        public Byte[] GetHashValueAsByteArray(Byte[] myClearText)
        {

            if (!myClearText.Any())
                return new Byte[0];

            return _Hasher.ComputeHash(myClearText);

        }

        #endregion

        #region CheckHashValue(...)

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(String myClearText, String myHashValue)
        {
            return GetHashValue(myClearText) == myHashValue;
        }

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(Byte[] myClearText, String myHashValue)
        {
            return GetHashValue(myClearText) == myHashValue;
        }

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
        /// <returns>true if matches, false if does not match</returns>
        public Boolean CheckHashValue(Byte[] myClearText, Byte[] myHashValue)
        {

            if (myHashValue.CompareByteArray(GetHashValueAsByteArray(myClearText)) == 0)
                return true;

            return false;

        }

        /// <summary>
        /// Checks if a Hash Value matches a given myClearText
        /// </summary>
        /// <param name="myClearText">the text that needs to be checked</param>
        /// <param name="myHashValue">the hash value</param>
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
