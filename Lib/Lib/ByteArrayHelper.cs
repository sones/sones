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
 * ByteArrayHelper
 * (c) Achim Friedland 2008-2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace sones.Lib
{

    /// <summary>
    /// This static class provides some Helper methods for byte arrays
    /// </summary>
    public static class ByteArrayHelper
    {

        #region FromUTF8String(myUTF8String)

        public static Byte[] FromUTF8String(String myUTF8String)
        {
            return new UTF8Encoding().GetBytes(myUTF8String);
        }

        #endregion

        #region FromHexString(myHexString)

        private static Regex _Regex = new Regex("[^0-9A-Fa-f]");

        public static Byte[] FromHexString(String myHexString)
        {
            return FromHexString(myHexString, false);
        }

        ///<summary>
        /// converts hexadecimal representation into a byte Array
        /// </summary>
        /// <param name="byteArray">the input byte array</param>
        /// <returns>the array as a hex-string</returns>
        public static Byte[] FromHexString(String myHexString, bool throwException)
        {
            String _HexString;

            if (!throwException)
            {
                _HexString = _Regex.Replace(myHexString, "");
            }
            else
            {
                _HexString = myHexString;
            }

            if (_HexString.Length == 0 || _HexString == "")
                return new Byte[0];

            if (_HexString.Length % 2 == 1)
                _HexString = "0" + _HexString;

            var _ByteArray = new Byte[_HexString.Length / 2];

            for (int i = 0; i < _HexString.Length/2; i++)
            {

                try
                {
                    _ByteArray[i] = Byte.Parse(_HexString.Substring(2*i, 2), System.Globalization.NumberStyles.HexNumber);
                }

                catch
                {
                    throw new ArgumentException("The given hex string is invalid!");
                }

            }

            return _ByteArray;

        }

        #endregion

    }

}
