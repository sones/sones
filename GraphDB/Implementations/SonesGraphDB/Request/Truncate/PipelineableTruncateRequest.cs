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
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request
{
    public sealed class PipelineableTruncateRequest : APipelinableRequest
    {
        #region data

        private readonly RequestTruncate _request;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable truncate request
        /// </summary>
        /// <param name="myClearRequest">The truncate request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableTruncateRequest(RequestTruncate myTruncateRequest, 
                                            SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken)
            : base(mySecurityToken, myTransactionToken)
        {
            _request = myTruncateRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexTypeManager.CheckManager.TruncateVertexType(_request.VertexTypeName, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            var vertexType = myMetaManager.VertexTypeManager.ExecuteManager.GetVertexType(_request.VertexTypeName, TransactionToken, SecurityToken);

            myMetaManager.VertexTypeManager.ExecuteManager.TruncateVertexType(vertexType.ID, TransactionToken, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        /// <summary>
        /// Generates the myResult of a truncate request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
