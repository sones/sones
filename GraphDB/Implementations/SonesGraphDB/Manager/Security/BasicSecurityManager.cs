using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Security;
using sones.Library.PropertyHyperGraph;
using sones.Library.VertexStore.Definitions;
using sones.Library.VertexStore;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;

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

        public bool VertexExists(long myVertexID, long myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(long myVertexID, long myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, Func<string, bool> myEditionsFilterFunc = null, Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<long> myInterestingVertexIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(long myTypeID, IEnumerable<VertexRevisionID> myInterestingRevisions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetVertexEditions(long myVertexID, long myVertexTypeID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(long myVertexID, long myVertexTypeID, IEnumerable<string> myInterestingEditions = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexRevision(long myVertexID, long myVertexTypeID, string myInterestingEdition, VertexRevisionID myToBeRemovedRevisionID)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexEdition(long myVertexID, long myVertexTypeID, string myToBeRemovedEdition)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(long myVertexID, long myVertexTypeID)
        {
            throw new NotImplementedException();
        }

        public IVertex AddVertex(VertexAddDefinition myVertexDefinition, VertexRevisionID myVertexRevisionID = null, bool myCreateIncomingEdges = true)
        {
            throw new NotImplementedException();
        }

        public IVertex UpdateVertex(long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID, VertexUpdateDefinition myVertexUpdate, string myToBeUpdatedEditions = null, VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            throw new NotImplementedException();
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

        public IPluginable InitializePlugin(Dictionary<String, Object> myParameters, GraphApplicationSettings mySettings)
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
