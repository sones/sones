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

using System.Collections.Generic;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a create edge types on the database
    /// </summary>
    public sealed class PipelineableCreateEdgeTypesRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestCreateEdgeTypes _request;

        /// <summary>
        /// The types that has been created during execution
        /// </summary>
        private IEnumerable<IEdgeType> _createdEdgeTypes;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable create edge type request
        /// </summary>
        /// <param name="myCreateEdgeTypeRequest">The create edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableCreateEdgeTypesRequest(RequestCreateEdgeTypes myCreateEdgeTypeRequest,
                                                   SecurityToken mySecurity, 
                                                   TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myCreateEdgeTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager
                .EdgeTypeManager
                .CheckManager
                .AddTypes(_request.TypePredefinitions, 
                            TransactionToken, 
                            SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _createdEdgeTypes = myMetaManager
                                    .EdgeTypeManager
                                    .ExecuteManager
                                    .AddTypes(_request.TypePredefinitions, 
                                                TransactionToken,
                                                SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a create parentVertex type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.CreateEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdEdgeTypes);
        }

        #endregion
    }
}