using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.ErrorHandling.IndexErrors;

namespace sones.GraphDB.Request.RebuildIndices
{
    public sealed class PipelineableRebuildIndicesRequest : APipelinableRequest
    {
        #region data

        private readonly RequestRebuildIndices _request;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableRebuildIndicesRequest(   RequestRebuildIndices myRebuildIndicesRequest,
                                                    SecurityToken mySecurity,
                                                    TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myRebuildIndicesRequest;
        }

        #endregion

        public override void Validate(Manager.IMetaManager myMetaManager)
        {
            
        }

        public override void Execute(Manager.IMetaManager myMetaManager)
        {
            IEnumerable<IVertexType> typesToRebuild = null;
            
            if (_request.Types == null)
            {
                typesToRebuild = myMetaManager.VertexTypeManager.ExecuteManager.GetAllVertexTypes(TransactionToken, SecurityToken);
            }
            else
            {

                #region Get types by name and return on error

                foreach (var typeName in _request.Types)
                {
                    var type = myMetaManager.VertexTypeManager.ExecuteManager.GetVertexType(typeName, TransactionToken, SecurityToken);

                    if (type == null)
                    {
                        throw new IndexTypeDoesNotExistException(typeName, "");
                    }
                    
                    (typesToRebuild as HashSet<IVertexType>).Add(type);
                }

                #endregion

            }

            foreach (var aType in typesToRebuild)
            {
                myMetaManager.IndexManager.RebuildIndices(aType.ID, TransactionToken, SecurityToken);
            }
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
