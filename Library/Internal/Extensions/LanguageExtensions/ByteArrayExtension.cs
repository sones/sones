/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;

namespace sones.Library.LanguageExtensions
{
    public static class ByteArrayExtension
    {
        #region CompareByteArray(myByteArray2)

        /// <summary>
        /// Compares two byte arrays bytewise
        /// </summary>
        /// <param name="myArray1">Array 1</param>
        /// <param name="myArray2">Array 2</param>
        /// <returns></returns>
        public static Int32 CompareByteArray(this Byte[] myByteArray, Byte[] myByteArray2)
        {

            if (myByteArray.Length < myByteArray2.Length)
                return -1;

            if (myByteArray.Length > myByteArray2.Length)
                return 1;

            for (int i = 0; i <= myByteArray.Length - 1; i++)
            {

                if (myByteArray[i] < myByteArray2[i])
                    return -1;

                if (myByteArray[i] > myByteArray2[i])
                    return 1;

            }

            return 0;

        }

        #endregion
    }

}
