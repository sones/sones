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
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.CreateIndex
{
    public sealed class PipelineableCreateIndexRequest : APipelinableRequest
    {
        #region data

        private readonly RequestCreateIndex _request;
        private IIndexDefinition IndexDef;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable clear request
        /// </summary>
        /// <param name="myClearRequest">The clear request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableCreateIndexRequest(RequestCreateIndex myCreateIndexRequest, SecurityToken mySecurity, Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myCreateIndexRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            if(string.IsNullOrWhiteSpace(_request.IndexDefinition.VertexTypeName))
            {
                throw new InvalidIndexAttributeException(_request.IndexDefinition.VertexTypeName, "");
            }
            else if(string.IsNullOrWhiteSpace(_request.IndexDefinition.Name) )
            {
                throw new InvalidIndexAttributeException(_request.IndexDefinition.Name, "");
            }
            //else if (string.IsNullOrWhiteSpace(_request.IndexDefinition.TypeName))
            //{
            //    throw new InvalidIndexAttributeException(_request.IndexDefinition.TypeName, "");
            //}
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            IndexDef = myMetaManager.IndexManager.CreateIndex(_request.IndexDefinition, SecurityToken, Int64);

            if (IndexDef == null)
            {
                throw new IndexCreationException(_request.IndexDefinition, "");
            }
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, IndexDef);
        }
    }
}
