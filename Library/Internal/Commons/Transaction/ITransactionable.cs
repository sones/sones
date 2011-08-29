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
using sones.Library.Commons.Security;

namespace sones.Library.Commons.Transaction
{
    public interface ITransactionable
    {
        /// <summary>
        /// Starts a new transaction
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myLongrunning">Is this a long running transaction</param>
        /// <param name="myIsolationLevel">The isolation level</param>
        /// <returns>A transaction id</returns>
        Int64 BeginTransaction(SecurityToken mySecurityToken,
                                            Boolean myLongrunning = false,
                                            IsolationLevel myIsolationLevel = IsolationLevel.Serializable);

        /// <summary>
        /// Commits a transaction
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The transaction id that identifies the transaction that shoulb be commited</param>
        void CommitTransaction( SecurityToken mySecurityToken,
                                Int64 myTransactionID);

        /// <summary>
        /// Rollback of a transaction
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The transaction id that identifies the transaction that should be rolled back</param>
        void RollbackTransaction(   SecurityToken mySecurityToken,
                                    Int64 myTransactionID);
    }
}