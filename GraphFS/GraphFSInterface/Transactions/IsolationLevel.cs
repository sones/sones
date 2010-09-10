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


#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.Transactions
{

    public enum IsolationLevel
    {

        /// <summary>
        /// Data records retrieved by a query are not prevented from modification by some other transactions. 
        /// Non-repeatable reads may occur, meaning data retrieved in a SELECT statement may be modified by some 
        /// other transaction when it commits. In this isolation level, read locks are acquired on selected data 
        /// but they are released immediately whereas write locks are released at the end of the transaction.
        /// </summary>
        ReadCommitted,

        /// <summary>
        /// In this isolation level, dirty reads are allowed. One transaction may see uncommitted changes made by some other transaction.
        /// </summary>
        ReadUncommitted,

        /// <summary>
        /// All data records read by a SELECT statement cannot be changed; however, if the SELECT statement 
        /// contains any ranged WHERE clauses, phantom reads can occur. In this isolation level, the transaction acquires 
        /// read locks on all retrieved data, but does not acquire range locks.
        /// </summary>
        RepeatableRead,

        /// <summary>
        /// This isolation level specifies that all transactions occur in a completely isolated fashion; i.e., as if all transactions 
        /// in the system had executed serially, one after the other. The DBMS may execute two or more transactions at the same time 
        /// only if the illusion of serial execution can be maintained. At this isolation level, phantom reads cannot occur.
        /// </summary>
        Serializable,

    }

}
