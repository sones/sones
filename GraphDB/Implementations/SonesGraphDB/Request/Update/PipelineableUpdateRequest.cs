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
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request.Update
{
    public sealed class PipelineableUpdateRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestUpdate _request;

        private IEnumerable<IVertex> updatedVertices;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransaction">The transaction token</param>
        public PipelineableUpdateRequest(   RequestUpdate myUpdateRequest, 
                                            SecurityToken mySecurity,
                                            Int64 myTransaction)
            : base(mySecurity, myTransaction)
        {
            _request = myUpdateRequest;
        }

        #endregion

        /// <summary>
        /// Validates the given request.
        /// </summary>
        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexManager.CheckManager.UpdateVertices(_request, Int64, SecurityToken);
        }

        /// <summary>
        /// Executes the given request.
        /// </summary>
        public override void Execute(IMetaManager myMetaManager)
        {
            updatedVertices = myMetaManager.VertexManager.ExecuteManager.UpdateVertices(_request, Int64, SecurityToken);
        }

        /// <summary>
        /// Returns the update request.
        /// </summary>
        public override IRequest GetRequest()
        {
            return _request;
        }

        /// <summary>
        /// Generates the myResult of a update request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, updatedVertices);
        }
    }
}
