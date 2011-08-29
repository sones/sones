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
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Request.DropIndex
{
    public sealed class PipelineableDropIndexRequest : APipelinableRequest
    {
        #region data

        private readonly RequestDropIndex _request;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableDropIndexRequest(RequestDropIndex myDropTypeRequest,
                                            SecurityToken mySecurity,
                                            Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDropTypeRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexTypeManager.CheckManager.GetType(_request.TypeName, Int64, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            myMetaManager.IndexManager.DropIndex(_request, Int64, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
