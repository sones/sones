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
using sones.GraphDB.Manager;
using sones.Library.ErrorHandling;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The abstract class for all pipelineable requests
    /// </summary>
    public abstract class APipelinableRequest
    {
        #region data

        /// <summary>
        /// The id of the pipelineable request
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// The security token of the request initiator
        /// </summary>
        protected SecurityToken SecurityToken { get; private set; }

        /// <summary>
        /// The transaction token of the request initiator
        /// </summary>
        protected TransactionToken TransactionToken { get; private set; }

        /// <summary>
        /// The request statistics
        /// </summary>
        public IRequestStatistics Statistics { get; set; }

        /// <summary>
        /// The exception that might happend
        /// </summary>
        public ASonesException Exception { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable request
        /// </summary>
        /// <param name="mySecurity">The security token</param>
        /// <param name="myTransaction">The transaction token</param>
        protected APipelinableRequest(SecurityToken mySecurity, TransactionToken myTransaction)
        {
            ID = Guid.NewGuid();
            SecurityToken = mySecurity;
            TransactionToken = myTransaction;
        }

        #endregion

        #region abstract methods

        /// <summary>
        /// Validation of the request
        /// </summary>
        /// <param name="myMetaManager">A manager that contains every other manager</param>
        public abstract void Validate(IMetaManager myMetaManager);

        /// <summary>
        /// Execute the request
        /// </summary>
        /// <param name="myMetaManager">A manager that contains every other manager</param>
        public abstract void Execute(IMetaManager myMetaManager);

        /// <summary>
        /// Get the request that has been executed
        /// </summary>
        /// <returns>An IRequest</returns>
        public abstract IRequest GetRequest();

        #endregion
    }
}