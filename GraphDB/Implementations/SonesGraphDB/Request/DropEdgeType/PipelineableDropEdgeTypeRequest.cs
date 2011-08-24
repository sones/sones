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
using sones.GraphDB.TypeSystem;
using sones.Library.ErrorHandling;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Request
{
    public sealed class PipelineableDropEdgeTypeRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestDropEdgeType _request;

        private Dictionary<Int64, String> _deletedTypeIDs;

        #endregion
        
        #region constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableDropEdgeTypeRequest(RequestDropEdgeType myDropEdgeTypeRequest, 
                                                SecurityToken mySecurity,
                                                Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDropEdgeTypeRequest;
        }

        #endregion
        
        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager
                .EdgeTypeManager
                .CheckManager
                .GetType(_request.TypeName, 
                            Int64, 
                            SecurityToken);

            myMetaManager
                .EdgeTypeManager
                .CheckManager
                .RemoveTypes(new List<IEdgeType> { myMetaManager
                                                        .EdgeTypeManager
                                                        .ExecuteManager
                                                        .GetType(_request.TypeName, 
                                                                    Int64, 
                                                                    SecurityToken) }, 
                                Int64, 
                                SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            IEdgeType graphDBType = myMetaManager
                                        .EdgeTypeManager
                                        .ExecuteManager
                                        .GetType(_request.TypeName, 
                                                    Int64, 
                                                    SecurityToken);

            if (graphDBType == null)
                throw new TypeDoesNotExistException<IEdgeType>(_request.TypeName);

            _deletedTypeIDs = myMetaManager
                                .EdgeTypeManager
                                .ExecuteManager
                                .RemoveTypes(new List<IEdgeType> { graphDBType }, 
                                                Int64, 
                                                SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertex request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.DropEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _deletedTypeIDs);
        }

        #endregion 
    }
}
