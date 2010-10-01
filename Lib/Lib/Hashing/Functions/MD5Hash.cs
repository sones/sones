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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace sones.Lib.Hashing.Functions
{
    public class MD5Hash : IHashFunction
    {
        private static MD5 _md5 = MD5.Create();

        public Int64 Hash(byte[] myData)
        {
            return Hash(myData, myData.Length);
        }

        public Int64 Hash(byte[] myData, int myLength)
        {
            return Hash(myData, myLength, 0);
        }

        public Int64 Hash(byte[] myData, int myLength, uint mySeed)
        {
            byte[] result = _md5.ComputeHash(myData,0 ,myLength);

            return BitConverter.ToInt64(result, 0);
        }
    }
}
