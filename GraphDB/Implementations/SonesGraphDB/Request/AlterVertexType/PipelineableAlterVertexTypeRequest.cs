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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.AlterType
{
    public sealed class PipelineableAlterVertexTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request which is executet
        /// </summary>
        public RequestAlterVertexType _request;

        private IVertexType _alteredVertexType;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable alter type request
        /// </summary>
        /// <param name="myRequest">The alter type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableAlterVertexTypeRequest(RequestAlterVertexType myRequest, 
                                                    SecurityToken mySecurityToken, 
                                                    TransactionToken myTransactionToken)
            :base(mySecurityToken, myTransactionToken)
        {
            _request = myRequest;
        }

        #endregion

        #region APipelineable Members

        /// <summary>
        /// Validates if the given request can be executed
        /// </summary>
        public override void Validate(IMetaManager myMetaManager)
        {
            RequestUpdate update;

            myMetaManager
                .VertexTypeManager
                .CheckManager
                .AlterType(_request, 
                            TransactionToken, 
                            SecurityToken,
                            out update);
        }

        /// <summary>
        /// Executes the given request
        /// </summary>
        public override void Execute(IMetaManager myMetaManager)
        {
            RequestUpdate update;

            _alteredVertexType = 
                myMetaManager
                .VertexTypeManager
                .ExecuteManager
                .AlterType(_request,
                            TransactionToken,
                            SecurityToken,
                            out update);

            myMetaManager
                .VertexManager
                .ExecuteManager
                .UpdateVertices(update,
                                TransactionToken,
                                SecurityToken);
        }

        /// <summary>
        /// Returns the alter type request
        /// </summary>
        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a alter type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _alteredVertexType);
        }

        #endregion
    }
}
