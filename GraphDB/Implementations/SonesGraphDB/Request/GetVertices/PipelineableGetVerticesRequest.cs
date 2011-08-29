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
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Expression.QueryPlan;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get vertices on the database
    /// </summary>
    public sealed class PipelineableGetVerticesRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetVertices _request;

        /// <summary>
        /// The vertices that have been fetched by the Graphdb
        /// it is used for generating the output
        /// </summary>
        private IEnumerable<IVertex> _fetchedIVertices;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertices request
        /// </summary>
        /// <param name="myGetVerticesRequest">The get vertices type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableGetVerticesRequest(
                                                RequestGetVertices myGetVerticesRequest, 
                                                SecurityToken mySecurity,
                                                Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetVerticesRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
             myMetaManager.VertexManager.CheckManager.GetVertices(_request, Int64, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _fetchedIVertices = myMetaManager.VertexManager.ExecuteManager.GetVertices(_request, Int64, SecurityToken);

        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertices request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedIVertices);
        }

        #endregion

    }
}