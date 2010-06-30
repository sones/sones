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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.DataStructures.UUID;

namespace sones.GraphFS.InternalObjects
{

    public sealed class RightUUID : UUID
    {

        #region TypeCode 
        public override UInt32 TypeCode { get { return 225; } }
        #endregion

        #region Constructors

        #region RightUUID()

        public RightUUID()
            : base()
        {
        }

        #endregion

        #region RightUUID(myUInt64)

        public RightUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region RightUUID(myString)

        public RightUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region RightUUID(mySerializedData)

        public RightUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion
    }

}
