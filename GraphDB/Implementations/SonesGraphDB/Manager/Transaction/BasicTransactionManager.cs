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
using System.Threading;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.VertexStore.Definitions;

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

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, IEnumerable<long> myInterestingVertexIDs, IEnumerable<string> myInterestingEditionNames, IEnumerable<Int64> myInterestingRevisionIDs)
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

        public IVertex UpdateVertex(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myToBeUpdatedVertexID, long myCorrespondingVertexTypeID, VertexUpdateDefinition myVertexUpdate, Boolean myCreateIncomingEdges = true, string myToBeUpdatedEditions = null, Int64 myToBeUpdatedRevisionIDs = 0L, bool myCreateNewRevision = false)
        {
            return _vertexStore.UpdateVertex(mySecurityToken, myTransactionToken, myToBeUpdatedVertexID, myCorrespondingVertexTypeID, myVertexUpdate, myCreateIncomingEdges, myToBeUpdatedEditions, myToBeUpdatedRevisionIDs, myCreateNewRevision);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, IEnumerable<long> myInterestingVertexIDs, VertexStoreFilter.EditionFilter myEditionsFilterFunc, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, myTypeID, myInterestingVertexIDs, myEditionsFilterFunc,
                                             myInterestingRevisionIDFilterFunc);
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myTypeID, string myEdition, VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurityToken, myTransactionToken, myTypeID, myEdition, myInterestingRevisionIDFilterFunc);
        }

        public void RemoveVertices(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexTypeID, IEnumerable<long> myToBeDeltedVertices = null)
        {
            _vertexStore.RemoveVertices(mySecurityToken, myTransactionToken, myVertexTypeID, myToBeDeltedVertices);
        }

        public ulong GetVertexCount(SecurityToken mySecurityToken, TransactionToken myTransactionToken, long myVertexTypeID)
        {
            return _vertexStore.GetVertexCount(mySecurityToken, myTransactionToken, myVertexTypeID);
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "sones.basictransactionmanager"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type> { { "vertexStore", typeof(IVertexStore) } }; }
        }

        

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<String, Object> myParameters)
        {
            return new BasicTransactionManager((IVertexStore)myParameters["vertexStore"]);
        }

        public void Dispose()
        { }

        #endregion

        #region private helper

        private void Init(IVertexStore myVertexStore)
        {
            _vertexStore = myVertexStore;
        }

        #endregion
    }
}
