using System;
using System.Collections.Generic;
using System.Threading;
using sones.Library.PropertyHyperGraph;
using sones.Library.Security;
using sones.Library.Settings;
using sones.Library.Transaction;
using sones.Library.VersionedPluginManager;
using sones.Library.VertexStore;
using sones.Library.VertexStore.Definitions;

namespace sones.GraphDB.Manager.Transaction
{
    /// <summary>
    /// A basic transaction manager that handles NO transactions...
    /// So every request is directly redirected to the underlying vertex store
    /// </summary>
    public sealed class BasicTransactionManager : ITransactionManager
    {
        #region Data

        /// <summary>
        /// The underlying vertex store
        /// </summary>
        private IVertexStore _vertexStore;

        /// <summary>
        /// A simple transaction counter...
        /// also used to generate transaction ids
        /// </summary>
        private Int64 _transactionCounter;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new transaction manager 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// DO NOT USE THIS ONE IF YOU DIRECTLY INITIALIZE THIS COMPONENT
        /// </summary>
        public BasicTransactionManager()
        {

        }

        /// <summary>
        /// Creates a new TransactionManager
        /// </summary>
        /// <param name="myVertexStore">The underlying vertex store</param>
        public BasicTransactionManager(IVertexStore myVertexStore)
        {
            Init(myVertexStore);
        }
        
        #endregion

        #region ITransactionable Members

        public TransactionToken BeginTransaction(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            return new TransactionToken(Interlocked.Increment(ref _transactionCounter));
        }

        public void CommitTransaction(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //do nothing
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            //do nothing
        }

        #endregion

        #region IVertexStore Members
        //redirect everything to the underlying vertex store

        public bool VertexExists(long myVertexID, long myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            return _vertexStore.VertexExists(myVertexID, myVertexTypeID, myEdition, myVertexRevisionID);
        }

        public IVertex GetVertex(long myVertexID, long myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            return _vertexStore.GetVertex(myVertexID, myVertexTypeID, myEdition, myVertexRevisionID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, Func<string, bool> myEditionsFilterFunc = null, Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc = null)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myInterestingVertexIDs, myEditionsFilterFunc, myInterestingRevisionIDFilterFunc);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myInterestingVertexIDs, myInterestingEditionNames, myInterestingRevisionIDs);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myInterestingVertexIDs);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<VertexRevisionID> myInterestingRevisions)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myInterestingRevisions);
        }

        public IEnumerable<string> GetVertexEditions(long myVertexID, long myVertexTypeID)
        {
            return _vertexStore.GetVertexEditions(myVertexID, myVertexTypeID);
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(long myVertexID, long myVertexTypeID, IEnumerable<string> myInterestingEditions = null)
        {
            return _vertexStore.GetVertexRevisionIDs(myVertexID, myVertexTypeID, myInterestingEditions);
        }

        public bool RemoveVertexRevision(long myVertexID, long myVertexTypeID, string myInterestingEdition, VertexRevisionID myToBeRemovedRevisionID)
        {
            return _vertexStore.RemoveVertexRevision(myVertexID, myVertexTypeID, myInterestingEdition, myToBeRemovedRevisionID);
        }

        public bool RemoveVertexEdition(long myVertexID, long myVertexTypeID, string myToBeRemovedEdition)
        {
            return _vertexStore.RemoveVertexEdition(myVertexID, myVertexTypeID, myToBeRemovedEdition);
        }

        public bool RemoveVertex(long myVertexID, long myVertexTypeID)
        {
            return _vertexStore.RemoveVertex(myVertexID, myVertexTypeID);
        }

        public IVertex AddVertex(VertexAddDefinition myVertexDefinition, VertexRevisionID myVertexRevisionID = null, bool myCreateIncomingEdges = true)
        {
            return _vertexStore.AddVertex(myVertexDefinition, myVertexRevisionID, myCreateIncomingEdges);
        }

        public IVertex UpdateVertex(long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID, VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null, VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            return _vertexStore.UpdateVertex(myToBeUpdatedVertexID, myCorrespondingVertexTypeID, myVertexUpdate, myToBeUpdatedEditions, myToBeUpdatedRevisionIDs, myCreateNewRevision);
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "BasicTransactionManager"; }
        }

        public Dictionary<String, Type> SetableParameters
        {
            get { return new Dictionary<string, Type> { { "vertexStore", typeof(IVertexStore) } }; }
        }

        public IPluginable InitializePlugin(Dictionary<String, Object> myParameters)
        {
            return new BasicTransactionManager((IVertexStore)myParameters["vertexStore"]);
        }

        #endregion

        #region private helper

        private void Init(IVertexStore myVertexStore)
        {
            _vertexStore = myVertexStore;
        }

        #endregion
    }
}
