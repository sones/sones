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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request.GetEdgeType
{
    public sealed class PipelineableGetAllEdgeTypesRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetAllEdgeTypes _request;

        /// <summary>
        /// The edge type that has been fetched from the graphDB
        /// </summary>
        private IEnumerable<IEdgeType> _fetchedEdgeTypes;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get edge type request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableGetAllEdgeTypesRequest(RequestGetAllEdgeTypes myGetAllEdgeTypesRequest, 
                                                    SecurityToken mySecurity,
                                                    TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetAllEdgeTypesRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager
                .EdgeTypeManager
                .CheckManager
                .GetAllTypes(TransactionToken, 
                                SecurityToken);                
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _fetchedEdgeTypes = myMetaManager
                                    .EdgeTypeManager
                                    .ExecuteManager
                                    .GetAllTypes(TransactionToken, 
                                                    SecurityToken);                
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get edge type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedEdgeTypes);
        }

        #endregion
    }
}
