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
 * ForestUUID
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class ForestUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 229; } }

        #endregion

        #region Constructors

        #region ForestUUID()

        public ForestUUID()
            : base()
        {
        }

        #endregion

        #region ForestUUID(myUInt64)

        public ForestUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region ForestUUID(myString)

        public ForestUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region ForestUUID(mySerializedData)

        public ForestUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region ForestUUID(myUUID)

        /// <summary>
        /// Generates a UUID based on the content of myUUID
        /// </summary>
        /// <param name="myUUID">A UUID</param>
        public ForestUUID(UUID myUUID)
            : base(myUUID)
        {   
        }

        #endregion

        #endregion

        #region NewUUID

        public new static ForestUUID NewUUID
        {
            get
            {
                return new ForestUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
