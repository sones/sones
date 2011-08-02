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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Collections;
using System.Collections.Generic;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a clear on the database
    /// </summary>
    public sealed class PipelineableClearRequest : APipelinableRequest
    {
        #region data

        private readonly RequestClear _request;

        private IEnumerable<long> _deletedVertexTypeIDs;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable clear request
        /// </summary>
        /// <param name="myClearRequest">The clear request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableClearRequest(RequestClear myClearRequest, 
                                        SecurityToken mySecurity,
                                        TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myClearRequest;
        }

        #endregion

        #region APipelineableRequest Member

        public override void Validate(IMetaManager myMetaManager)
        {
            if(_request == null)
                throw new NotImplementedException();
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _deletedVertexTypeIDs = myMetaManager.VertexTypeManager.ExecuteManager.ClearTypes(TransactionToken, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        /// <summary>
        /// Generates the myResult of a clear request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _deletedVertexTypeIDs);
        }
    }
}