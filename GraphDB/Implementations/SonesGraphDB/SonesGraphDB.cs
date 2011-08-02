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
using System.Globalization;
using System.Threading;
using sones.GraphDB.Manager;
using sones.GraphDB.Manager.Plugin;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Request;
using sones.GraphDB.Request.AlterType;
using sones.GraphDB.Request.CreateIndex;
using sones.GraphDB.Request.DecribeIndex;
using sones.GraphDB.Request.Delete;
using sones.GraphDB.Request.DropIndex;
using sones.GraphDB.Request.DropType;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.Request.GetType;
using sones.GraphDB.Request.RebuildIndices;
using sones.GraphDB.Request.Update;
using sones.GraphDB.Settings;
using sones.GraphFS;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB
{
    /// <summary>
    /// The sones implemention of the graphdb interface
    /// </summary>
    public sealed class SonesGraphDB : IGraphDB
    {
        #region data

        /// <summary>
        /// The Culture information of this GraphDB instance.
        /// </summary>
        private readonly CultureInfo _graphDBCulture;

        /// <summary>
        /// A manager for handling incoming requests
        /// </summary>
        private readonly IRequestManager _requestManager;

        /// <summary>
        /// A manager to dynamically load versioned plugins
        /// </summary>
        private readonly GraphDBPluginManager _graphDBPluginManager;

        /// <summary>
        /// A manager that is responsible for transactions
        /// </summary>
        private readonly ITransactionManager _transactionManager;

        /// <summary>
        /// A manager that is responsible for security
        /// </summary>
        private readonly ISecurityManager _securityManager;

        /// <summary>
        /// The persistence layer
        /// </summary>
        private readonly IGraphFS _iGraphFS;

        /// <summary>
        /// A globally unique identifier for this graphdb instance
        /// </summary>
        private readonly Guid _id;

        /// <summary>
        /// The settings of the graph db
        /// </summary>
        private readonly GraphApplicationSettings _applicationSettings;

        /// <summary>
        /// The cancellation token of the sones graphdb
        /// </summary>
        private readonly CancellationTokenSource _cts;

        private readonly SecurityToken _security = null;

        private readonly TransactionToken _transaction = null;
        private IDManager _idManager;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones graphdb instance
        /// </summary>
        /// <param name="myPlugins">The plugins that are valid for the sones GraphDB component</param>
        /// <param name="myCreate">Should the sones graphdb created?</param>
        /// <param name="myCulture">the culture of this instance, defaults to en-us</param>
        public SonesGraphDB(
            GraphDBPlugins myPlugins = null,
            Boolean myCreate = true, CultureInfo myCulture = null)
        {
            _id = Guid.NewGuid();

            _cts = new CancellationTokenSource();

            if (myPlugins == null)
            {
                myPlugins = new GraphDBPlugins();
            }

            if (myCulture == null)
            {
                // defaults to en-us
                myCulture = new CultureInfo("en-us");
            }

            _graphDBCulture = myCulture;

            #region settings

            _applicationSettings = new GraphApplicationSettings(ConstantsSonesGraphDB.ApplicationSettingsLocation);

            #endregion

            #region plugin manager

            _graphDBPluginManager = new GraphDBPluginManager();
            
            #endregion

            #region IGraphFS

            _iGraphFS = LoadGraphFsPlugin(myPlugins.IGraphFSDefinition);

            #endregion

            #region transaction

            _transactionManager = LoadTransactionManagerPlugin(myPlugins.TransactionManagerPlugin);

            #endregion

            #region security

            _securityManager = LoadSecurityManager(myPlugins.SecurityManagerPlugin);

            #endregion

            #region ids

            _idManager = new IDManager();

            #endregion

            #region requests

            _requestManager = LoadRequestManager(CreateMetamanager(myPlugins));

            #endregion
        }
        
        #endregion

        #region IGraphDB Members

        #region requests

        #region create VertexType(s)

        public TResult CreateVertexTypes<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,
            RequestCreateVertexTypes myRequestCreateVertexType,
            Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableCreateVertexTypesRequest(myRequestCreateVertexType,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableCreateVertexTypesRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexType myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableCreateVertexTypeRequest(myRequestCreateVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableCreateVertexTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region clear

        public TResult Clear<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,                  
            RequestClear myRequestClear, 
            Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableClearRequest(myRequestClear,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableClearRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region delete

        public TResult Delete<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestDelete myRequestDelete,
            Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableDeleteRequest(myRequestDelete,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableDeleteRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region drop type

        public TResult DropType<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestDropVertexType myRequestDropType,
            Converter.DropVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableDropVertexTypeRequest(myRequestDropType,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableDropVertexTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region create index

        public TResult CreateIndex<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestCreateIndex myRequestCreateIndex,
            Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableCreateIndexRequest(myRequestCreateIndex,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableCreateIndexRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region drop index

        public TResult DropIndex<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestDropIndex myRequestDropIndex,
            Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableDropIndexRequest(myRequestDropIndex,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableDropIndexRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region rebuild indices

        public TResult RebuildIndices<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestRebuildIndices myRequestRebuildIndices,
            Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableRebuildIndicesRequest(myRequestRebuildIndices,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableRebuildIndicesRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Insert

        public TResult Insert<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,
            RequestInsertVertex myRequestInsert,
            Converter.InsertResultConverter<TResult> myOutputconverter)
        {

            var executedRequest = _requestManager.SynchronExecution(new PipelineableInsertRequest(myRequestInsert,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableInsertRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertices

        public TResult GetVertices<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestGetVertices myRequestGetVertices,
            Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetVerticesRequest(myRequestGetVertices,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableGetVerticesRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region TraverseVertex

        public TResult TraverseVertex<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestTraverseVertex myRequestTraverseVertex,
            Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableTraverseVertexRequest(myRequestTraverseVertex,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableTraverseVertexRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertexType

        public TResult GetVertexType<TResult>(SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken, 
                                                RequestGetVertexType myRequestGetVertexType, 
                                                Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetVertexTypeRequest(myRequestGetVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableGetVertexTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        public TResult GetAllVertexTypes<TResult>(SecurityToken mySecurityToken, 
                                                    TransactionToken myTransactionToken, 
                                                    RequestGetAllVertexTypes myRequestGetVertexType, 
                                                    Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetAllVertexTypesRequest(myRequestGetVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableGetAllVertexTypesRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetEdgeType

        public TResult GetEdgeType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetEdgeTypeRequest(myRequestGetEdgeType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableGetEdgeTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        public TResult GetAllEdgeTypes<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllEdgeTypes myRequestGetEdgeType, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetAllEdgeTypesRequest(myRequestGetEdgeType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableGetAllEdgeTypesRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertex

        public TResult GetVertex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetVertexRequest(myRequestGetVertex,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableGetVertexRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Truncate

        public TResult Truncate<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableTruncateRequest(myRequestTruncate,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableTruncateRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Describe Index

        public TResult DescribeIndex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableDescribeIndexRequest(myRequestDescribeIndex,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableDescribeIndexRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Describe Indices

        public TResult DescribeIndices<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndicesResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableDescribeIndicesRequest(myRequestDescribeIndex,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableDescribeIndicesRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }


        #endregion

        #region Update

        public TResult Update<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestUpdate myRequestUpdate, Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableUpdateRequest(myRequestUpdate,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableUpdateRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region AlterVertexType

        public TResult AlterVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestAlterVertexType myRequestAlterVertexType, Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableAlterVertexTypeRequest(myRequestAlterVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableAlterVertexTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region CreateEdgeType

        public TResult CreateEdgeType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateEdgeType myRequestCreateVertexType, Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableCreateEdgeTypeRequest(myRequestCreateVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableCreateEdgeTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region AlterEdgeType

        public TResult AlterEdgeType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestAlterEdgeType myRequestAlterEdgeType, Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableAlterEdgeTypeRequest(myRequestAlterEdgeType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableAlterEdgeTypeRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertexCount

        public TResult GetVertexCount<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexCount myRequestGetVertexCount, Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            var executedRequest = _requestManager.SynchronExecution(new PipelineableGetVertexCountRequest(myRequestGetVertexCount,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableGetVertexCountRequest)executedRequest).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #endregion

        #region misc

        public Guid ID
        {
            get { return _id; }
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {

            _iGraphFS.Shutdown(mySecurityToken);

            //TODO: shutdown plugins
            _graphDBPluginManager.ShutdownPlugins();

        }

        #endregion

        #region Transaction

        public TransactionToken BeginTransaction(SecurityToken mySecurity, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            return _transactionManager.BeginTransaction(mySecurity, myLongrunning, myIsolationLevel);
        }

        public void CommitTransaction(SecurityToken mySecurity, TransactionToken myTransactionToken)
        {
            _transactionManager.CommitTransaction(mySecurity, myTransactionToken);
        }

        public void RollbackTransaction(SecurityToken mySecurity, TransactionToken myTransactionToken)
        {
            _transactionManager.RollbackTransaction(mySecurity, myTransactionToken);
        }

        #endregion

        #region IUserAuthentication Members

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            return _securityManager.LogOn(toBeAuthenticatedCredentials);
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            _securityManager.LogOff(toBeLoggedOfToken);
        }

        #endregion

        #endregion

        #region private helper

        /// <summary>
        /// Load a request manager
        /// </summary>
        /// <param name="myMetaManager">The meta manager of the graphdb</param>
        private IRequestManager LoadRequestManager(IMetaManager myMetaManager)
        {
            return new SimpleRequestManager(myMetaManager);
        }

        /// <summary>
        /// Create a meta manager
        /// </summary>
        /// <param name="myPlugins">The plugin definitions</param>
        /// <returns>A meta manager</returns>
        private IMetaManager CreateMetamanager(GraphDBPlugins myPlugins)
        {
            return MetaManager.CreateMetaManager(_securityManager, _idManager, myPlugins, _graphDBPluginManager, _applicationSettings, _transaction, _security);
        }
        
        /// <summary>
        /// Loads the security manager
        /// </summary>
        /// <param name="mySecurityManagerPlugin">The security manager plugin definition</param>
        private ISecurityManager LoadSecurityManager(PluginDefinition mySecurityManagerPlugin)
        {
            if (mySecurityManagerPlugin != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<ISecurityManager>(mySecurityManagerPlugin.NameOfPlugin, myParameter: mySecurityManagerPlugin.PluginParameter);
            }

            //so lets take the default one
            var defaultSecurityManagerName = _applicationSettings.Get<DefaultSecurityManagerImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<ISecurityManager>(defaultSecurityManagerName, myParameter: new Dictionary<string, object> { { "vertexStore", _transactionManager } });
        }

        /// <summary>
        /// Load the transaction manager
        /// </summary>
        /// <param name="myTransactionManagerPlugin">The transaction manager plugin definition</param>
        private ITransactionManager LoadTransactionManagerPlugin(PluginDefinition myTransactionManagerPlugin)
        {
            if (myTransactionManagerPlugin != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<ITransactionManager>(myTransactionManagerPlugin.NameOfPlugin, myParameter: myTransactionManagerPlugin.PluginParameter);
            }
            
            //so there is no given plugin... lets try the IGraphFS
            if (_iGraphFS.IsTransactional)
            {
                return (ITransactionManager)_iGraphFS;
            }
            
            //so lets take the default one
            var defaultTransactionManagerName = _applicationSettings.Get<DefaultTransactionManagerImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<ITransactionManager>(defaultTransactionManagerName, myParameter: new Dictionary<string, object> { { "vertexStore", _iGraphFS } });
        }

        /// <summary>
        /// Loads the IGraphFS
        /// </summary>
        /// <param name="myIGraphFSDefinition">The IGraphFS plugin definition</param>
        private IGraphFS LoadGraphFsPlugin(PluginDefinition myIGraphFSDefinition)
        {
            if (myIGraphFSDefinition != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<IGraphFS>(myIGraphFSDefinition.NameOfPlugin, myParameter: myIGraphFSDefinition.PluginParameter);
            }

            //return the default fs
            var defaultFSName = _applicationSettings.Get<DefaultGraphFSImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<IGraphFS>(defaultFSName);
        }

        #endregion
    }
}
