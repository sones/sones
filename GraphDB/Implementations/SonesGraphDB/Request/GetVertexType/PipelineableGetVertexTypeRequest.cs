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
using sones.GraphDB.TypeSystem;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get vertex tyoe on the database
    /// </summary>
    public sealed class PipelineableGetVertexTypeRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetVertexType _request;

        /// <summary>
        /// The vertex type that has been fetched from the graphDB
        /// </summary>
        private IVertexType _fetchedVertexType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertex type request
        /// </summary>
        /// <param name="myGetVertexTypeRequest">The get vertex type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableGetVertexTypeRequest(
                                                RequestGetVertexType myGetVertexTypeRequest, 
                                                SecurityToken mySecurity,
                                                Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetVertexTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            if (_request.VertexTypeName == null)
            {
                _fetchedVertexType = myMetaManager.VertexTypeManager.ExecuteManager.GetType(_request.VertexTypeID, Int64, SecurityToken);   
            }
            else
            {
                _fetchedVertexType = myMetaManager.VertexTypeManager.ExecuteManager.GetType(_request.VertexTypeName, Int64, SecurityToken);   
            }
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertex type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedVertexType);
        }

        #endregion

    }
}