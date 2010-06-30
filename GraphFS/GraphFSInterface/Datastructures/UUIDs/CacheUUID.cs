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
 * CacheUUID
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class CacheUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 235; } }

        #endregion

        #region Constructors

        #region CacheUUID()

        public CacheUUID()
            : base()
        {
        }

        #endregion

        #region CacheUUID(myUInt64)

        public CacheUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region CacheUUID(myString)

        public CacheUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region CacheUUID(mySerializedData)

        public CacheUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public new static CacheUUID NewUUID
        {
            get
            {
                return new CacheUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


    }

}
