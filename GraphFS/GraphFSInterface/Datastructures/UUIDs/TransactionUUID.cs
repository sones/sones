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
 * TransactionUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class TransactionUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 251; } }

        #endregion

        #region Constructors

        #region TransactionUUID()

        public TransactionUUID()
            : base()
        {
        }

        #endregion

        #region TransactionUUID(myUInt64)

        public TransactionUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region TransactionUUID(myString)

        public TransactionUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region TransactionUUID(mySerializedData)

        public TransactionUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion

        #region NewUUID

        public new static TransactionUUID NewUUID
        {
            get
            {
                return new TransactionUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
