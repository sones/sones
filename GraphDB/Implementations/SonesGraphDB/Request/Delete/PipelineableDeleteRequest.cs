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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Request.Delete
{
    public sealed class PipelineableDeleteRequest : APipelinableRequest
    {
        #region data

        private readonly RequestDelete _request;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable clear request
        /// </summary>
        /// <param name="myClearRequest">The clear request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableDeleteRequest(RequestDelete myDeleteRequest, SecurityToken mySecurity,
                                        Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDeleteRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexManager.CheckManager.Delete(_request, SecurityToken, Int64);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            myMetaManager.VertexManager.ExecuteManager.Delete(_request, SecurityToken, Int64);            
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _request.DeletedAttributes, _request.DeletedVertices);
        }

    }
}
