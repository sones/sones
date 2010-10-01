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
 * GraphFSStreamUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class GraphFSStreamUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 230; } }

        #endregion

        #region Constructor(s)

        #region GraphFSStreamUUID()

        public GraphFSStreamUUID()
            : base()
        {
        }

        #endregion

        #region GraphFSStreamUUID(myUInt64)

        public GraphFSStreamUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region GraphFSStreamUUID(myString)

        public GraphFSStreamUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region GraphFSStreamUUID(mySerializedData)

        public GraphFSStreamUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion

        #region NewUUID

        public new static GraphFSStreamUUID NewUUID
        {
            get
            {
                return new GraphFSStreamUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
