using System;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.Security
{
    /// <summary>
    /// A basis security manager that does not secure anything
    /// </summary>
    public sealed class BasicSecurityManager : ISecurityManager
    {
        #region data

        /// <summary>
        /// The underlying vertex store
        /// </summary>
        private IVertexStore _vertexStore;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new security manager 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// DO NOT USE THIS ONE IF YOU DIRECTLY INITIALIZE THIS COMPONENT
        /// </summary>
        public BasicSecurityManager()
        {

        }

        /// <summary>
        /// Creates a new basic security manager
        /// </summary>
        /// <param name="myVertexStore">The underlying vertex store</param>
        public BasicSecurityManager (IVertexStore myVertexStore)
        {
            Init(myVertexStore);
        }

        #endregion


        #region ISecurityManager Members

        public bool AllowedToCreateVertexType(SecurityToken mySecuritytoken)
        {
            return true;
        }

        #endregion

        #region IUserAuthentication Members

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphElementAuthentication Members

        public bool Authenticate(SecurityToken mySecurityToken, ulong myToBeCheckedVertexID, ulong myCorrespondingVertexTypeID, Right myWantedAction = Right.Traverse)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVertexStore Members
        //redirect everything to the underlying vertex store

        public bool VertexExists(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID, string myEdition = null, Int64 myVertexRevisionID = 0L)
        {
            return _vertexStore.VertexExists(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID, myEdition, myVertexRevisionID);
        }

        public IVertex GetVertex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID, string myEdition = null, Int64 myVertexRevisionID = 0L)
        {
            return _vertexStore.GetVertex(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID, myEdition, myVertexRevisionID);
        }

        public IVertex GetVertex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID, VertexStoreFilter.EditionFilter myEditionsFilterFunc = null, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return _vertexStore.GetVertex(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID, myEditionsFilterFunc, myInterestingRevisionIDFilterFunc);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<Int64> myInterestingRevisionIDs = null)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, myTypeID, myInterestingVertexIDs, myInterestingEditionNames, myInterestingRevisionIDs);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, IEnumerable<long> myInterestingVertexIDs)
        {
            return _vertexStore.GetVerticesByTypeIDAndRevisions(mySecurityToken, myTransactionToken, myTypeID, myInterestingVertexIDs);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeIDAndRevisions(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, IEnumerable<Int64> myInterestingRevisions)
        {
            return _vertexStore.GetVerticesByTypeIDAndRevisions(mySecurityToken, myTransactionToken, myTypeID, myInterestingRevisions);
        }

        public IEnumerable<string> GetVertexEditions(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID)
        {
            return _vertexStore.GetVertexEditions(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID);
        }

        public IEnumerable<Int64> GetVertexRevisionIDs(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID, IEnumerable<string> myInterestingEditions = null)
        {
            return _vertexStore.GetVertexRevisionIDs(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID, myInterestingEditions);
        }

        public bool RemoveVertexRevision(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID, string myInterestingEdition, Int64 myToBeRemovedRevisionID)
        {
            return _vertexStore.RemoveVertexRevision(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID, myInterestingEdition, myToBeRemovedRevisionID);
        }

        public bool RemoveVertexEdition(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID, string myToBeRemovedEdition)
        {
            return _vertexStore.RemoveVertexEdition(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID, myToBeRemovedEdition);
        }

        public bool RemoveVertex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexID, long myVertexTypeID)
        {
            return _vertexStore.RemoveVertex(mySecurityToken, myTransactionToken, myVertexID, myVertexTypeID);
        }

        public IVertex AddVertex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, VertexAddDefinition myVertexDefinition, Int64 myVertexRevisionID = 0L, bool myCreateIncomingEdges = true)
        {
            return _vertexStore.AddVertex(mySecurityToken, myTransactionToken, myVertexDefinition, myVertexRevisionID, myCreateIncomingEdges);
        }

        public IVertex UpdateVertex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID, VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null, Int64 myToBeUpdatedRevisionIDs = 0L, bool myCreateNewRevision = false)
        {
            return _vertexStore.UpdateVertex(mySecurityToken, myTransactionToken, myToBeUpdatedVertexID, myCorrespondingVertexTypeID, myVertexUpdate, myToBeUpdatedEditions, myToBeUpdatedRevisionIDs, myCreateNewRevision);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, VertexStoreFilter.EditionFilter myEditionsFilterFunc = null, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, myTypeID, myInterestingVertexIDs, myEditionsFilterFunc,
                                             myInterestingRevisionIDFilterFunc);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, string myEdition = null, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, myTypeID, myEdition, myInterestingRevisionIDFilterFunc);
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "BasicSecurityManager"; }
        }

        public Dictionary<String, Type> SetableParameters
        {
            get { return new Dictionary<string, Type> { { "vertexStore", typeof(IVertexStore) } }; }
        }

        public IPluginable InitializePlugin(Dictionary<String, Object> myParameters)
        {
            return new BasicSecurityManager((IVertexStore)myParameters["vertexStore"]);
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
