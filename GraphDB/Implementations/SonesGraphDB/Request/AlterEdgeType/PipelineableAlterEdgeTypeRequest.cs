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
using System.Linq;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Request.AlterType
{
    /// <summary>
    /// This class is responsible for realizing a alter type on the database
    /// </summary>
    public sealed class PipelineableAlterEdgeTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request which is executed
        /// </summary>
        public RequestAlterEdgeType _request;

        private IEdgeType _alteredEdgeType = null;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable alter edge type request
        /// </summary>
        /// <param name="myRequest">The alter edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableAlterEdgeTypeRequest(RequestAlterEdgeType myRequest, 
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
                .EdgeTypeManager
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

            _alteredEdgeType = 
                myMetaManager
                    .EdgeTypeManager
                    .ExecuteManager
                    .AlterType(_request,
                                TransactionToken,
                                SecurityToken,
                                out update);

            #region update vertices

            //if (update.GetVerticesRequest == null)
            //{
            //    var getVerticesRequest = CreateGetVerticesRequest(myMetaManager, update);

            //    if (getVerticesRequest != null)
            //    {
            //        update.SetGetVerticesRequest(getVerticesRequest);

            //        myMetaManager
            //            .VertexManager
            //            .ExecuteManager
            //            .UpdateVertices(update,
            //                            TransactionToken,
            //                            SecurityToken);
            //    }
            //}

            #endregion
        }

        /// <summary>
        /// Returns the alter edge type request
        /// </summary>
        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a alter edge type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _alteredEdgeType);
        }

        internal RequestGetVertices CreateGetVerticesRequest(IMetaManager myMetaManager, 
                                                                RequestUpdate myRequestUpdate)
        {
            var outEdges = myRequestUpdate.UpdateOutgoingEdgesProperties;

            if (outEdges != null)
            {
                var userdefTypes = myMetaManager
                        .VertexTypeManager
                        .ExecuteManager
                        .GetAllTypes(TransactionToken,
                                        SecurityToken)
                            .Where(_ => _
                                        .IsUserDefined);

                if (userdefTypes != null && userdefTypes.Count() > 0)
                {
                    var types =
                        userdefTypes
                            .Where(_ => _
                                        .GetOutgoingEdgeDefinitions(false)
                                        .Any(x => x.EdgeType
                                                    .ID
                                                    .Equals(outEdges.First().EdgeTypeID) ||
                                                    x
                                                    .InnerEdgeType
                                                    .ID
                                                    .Equals(outEdges.First().EdgeTypeID)));

                    if(types != null && types.Count() > 0)
                        return new RequestGetVertices(types.FirstOrDefault().ID);
                }
            }
            else
                throw new TypeDoesNotExistException<IVertexType>(
                            "Empty", 
                            "There is no type found to update after alter type.");

            return null;
        }

        #endregion
    }
}
