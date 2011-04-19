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

        public bool VertexExists(long myVertexID, long myVertexTypeID, string myEdition = null, Int64 myVertexRevisionID = 0L)
        {
            return _vertexStore.VertexExists(myVertexID, myVertexTypeID, myEdition, myVertexRevisionID);
        }

        public IVertex GetVertex(long myVertexID, long myVertexTypeID, string myEdition = null, Int64 myVertexRevisionID = 0L)
        {
            return _vertexStore.GetVertex(myVertexID, myVertexTypeID, myEdition, myVertexRevisionID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<Int64> myInterestingRevisionIDs = null)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myInterestingVertexIDs, myInterestingEditionNames, myInterestingRevisionIDs);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs)
        {
            return _vertexStore.GetVerticesByTypeIDAndRevisions(myTypeID, myInterestingVertexIDs);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID);
        }

        public IEnumerable<IVertex> GetVerticesByTypeIDAndRevisions(long myTypeID, IEnumerable<Int64> myInterestingRevisions)
        {
            return _vertexStore.GetVerticesByTypeIDAndRevisions(myTypeID, myInterestingRevisions);
        }

        public IEnumerable<string> GetVertexEditions(long myVertexID, long myVertexTypeID)
        {
            return _vertexStore.GetVertexEditions(myVertexID, myVertexTypeID);
        }

        public IEnumerable<Int64> GetVertexRevisionIDs(long myVertexID, long myVertexTypeID, IEnumerable<string> myInterestingEditions = null)
        {
            return _vertexStore.GetVertexRevisionIDs(myVertexID, myVertexTypeID, myInterestingEditions);
        }

        public bool RemoveVertexRevision(long myVertexID, long myVertexTypeID, string myInterestingEdition, Int64 myToBeRemovedRevisionID)
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

        public IVertex AddVertex(VertexAddDefinition myVertexDefinition, Int64 myVertexRevisionID = 0L, bool myCreateIncomingEdges = true)
        {
            return _vertexStore.AddVertex(myVertexDefinition, myVertexRevisionID, myCreateIncomingEdges);
        }

        public IVertex UpdateVertex(long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID, VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null, Int64 myToBeUpdatedRevisionIDs = 0L, bool myCreateNewRevision = false)
        {
            return _vertexStore.UpdateVertex(myToBeUpdatedVertexID, myCorrespondingVertexTypeID, myVertexUpdate, myToBeUpdatedEditions, myToBeUpdatedRevisionIDs, myCreateNewRevision);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, VertexStoreFilter.EditionFilter myEditionsFilterFunc = null, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myInterestingVertexIDs, myEditionsFilterFunc,
                                             myInterestingRevisionIDFilterFunc);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, string myEdition = null, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null)
        {
            return _vertexStore.GetVerticesByTypeID(myTypeID, myEdition, myInterestingRevisionIDFilterFunc);
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
