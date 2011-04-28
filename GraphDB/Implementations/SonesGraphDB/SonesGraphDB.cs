using System;
using System.Collections.Generic;
using System.Threading;
using sones.GraphDB.Manager;
using sones.GraphDB.Manager.Plugin;
using sones.GraphDB.Request;
using sones.GraphDB.Settings;
using sones.GraphFS;
using sones.Library.Commons.Security;
using sones.Library.Settings;
using sones.Library.Commons.Transaction;
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

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones graphdb instance
        /// </summary>
        /// <param name="myPlugins">The plugins that are valid for the sones GraphDB component</param>
        /// <param name="myCreate">Should the sones graphdb created?</param>
        public SonesGraphDB(
            GraphDBPlugins myPlugins = null,
            Boolean myCreate = true)
        {
            _id = Guid.NewGuid();

            _cts = new CancellationTokenSource();

            if (myPlugins == null)
            {
                myPlugins = new GraphDBPlugins();
            }

            #region settings

            _applicationSettings = new GraphApplicationSettings(ConstantsSonesGraphDB.ApplicationSettingsLocation);

            #endregion

            #region plugin manager

            _graphDBPluginManager = new GraphDBPluginManager();
            
            #endregion

            #region IGraphFS

            _iGraphFS = LoadGraphFsPlugin(myPlugins.IGraphFSDefinition);

            #endregion

            DBCreationManager creationManager = new DBCreationManager();
            creationManager.CreateBaseGraph(_iGraphFS);

            #region transaction

            _transactionManager = LoadTransactionManagerPlugin(myPlugins.TransactionManagerPlugin);

            #endregion

            #region security

            _securityManager = LoadSecurityManager(myPlugins.SecurityManagerPlugin);

            #endregion

            #region requests

            _requestManager = LoadRequestManager(myPlugins.RequestManagerPlugin, myPlugins.RequestSchedulerPlugin, CreateMetamanager(myPlugins));

            #endregion
        }

        #endregion

        #region IGraphDB Members

        #region requests

        #region create VertexType

        public TResult CreateVertexType<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,
            RequestCreateVertexTypes myRequestCreateVertexType,
            Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableCreateVertexTypeRequest(myRequestCreateVertexType,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableCreateVertexTypeRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region clear

        public TResult Clear<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,                  
            RequestClear myRequestClear, 
            Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableClearRequest(myRequestClear, mySecurity,
                                                                             myTransactionToken));

            return ((PipelineableClearRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Insert

        public TResult Insert<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,
            RequestInsertVertex myRequestInsert,
            Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableInsertRequest(myRequestInsert, mySecurity,
                                                                              myTransactionToken));

            return ((PipelineableInsertRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertices

        public TResult GetVertices<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestGetVertices myRequestGetVertices,
            Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableGetVerticesRequest(myRequestGetVertices, mySecurity, myTransactionToken));

            return ((PipelineableGetVerticesRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region TraverseVertex

        public TResult TraverseVertex<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestTraverseVertex myRequestTraverseVertex,
            Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableTraverseVertexRequest(myRequestTraverseVertex, mySecurity, myTransactionToken));

            return ((PipelineableTraverseVertexRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertexType

        public TResult GetVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableGetVertexTypeRequest(myRequestGetVertexType, mySecurityToken, myTransactionToken));

            return ((PipelineableGetVertexTypeRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetEdgeType

        public TResult GetEdgeType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableGetEdgeTypeRequest(myRequestGetEdgeType, mySecurityToken, myTransactionToken));

            return ((PipelineableGetEdgeTypeRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertex

        public TResult GetVertex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableGetVertexRequest(myRequestGetVertex, mySecurityToken, myTransactionToken));

            return ((PipelineableGetVertexRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Truncate

        public TResult Truncate<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableTruncateRequest(myRequestTruncate, mySecurityToken, myTransactionToken));

            return ((PipelineableTruncateRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #endregion

        #region misc

        public Guid ID
        {
            get { return _id; }
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
        /// <param name="myRequestManagerPlugin">The actual request manager plugin definition</param>
        /// <param name="myRequestSchedulerPlugin">The request scheduler plugin that is needed for the request manager</param>
        /// <param name="myMetaManager">The meta manager of the graphdb</param>
        private IRequestManager LoadRequestManager(PluginDefinition myRequestManagerPlugin, PluginDefinition myRequestSchedulerPlugin, IMetaManager myMetaManager)
        {
            #region user defined

            var requestScheduler = LoadRequestScheduler(myRequestSchedulerPlugin);

            if (myRequestManagerPlugin != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<IRequestManager>(myRequestManagerPlugin.NameOfPlugin, myRequestManagerPlugin.PluginParameter);
            }

            #endregion

            #region default

            //so lets take the default one
            var defaultRequestManagerName = _applicationSettings.Get<DefaultRequestManagerImplementation>();

            #region set some parameters

            int queueLengthForIncomingRequests = 10000;
            int executionQueueLength = 10000;

            //the CPU*1,5
            //decreased by one, because of one validation task
            var numberOfOptimalTasks = ((Environment.ProcessorCount * 3) / 2) - 1;

            //use at least 2 tasks
            numberOfOptimalTasks = numberOfOptimalTasks > 1 ? numberOfOptimalTasks : 2;

            #endregion

            return _graphDBPluginManager.
                GetAndInitializePlugin<IRequestManager>(
                    defaultRequestManagerName,
                    new Dictionary<String, Object>
                    {
                        {"queueLengthForIncomingRequests", queueLengthForIncomingRequests},
                        {"executionQueueLength", executionQueueLength},
                        {"executionTaskCount", numberOfOptimalTasks},
                        {"metaManager", myMetaManager},
                        {"requestScheduler", requestScheduler},
                        {"cts", _cts},
                    });
            
            #endregion
        }

        /// <summary>
        /// Load a request scheduler
        /// </summary>
        /// <param name="myRequestSchedulerPlugin">The scheduler plugin definition</param>
        /// <returns>A request scheduler</returns>
        private IRequestScheduler LoadRequestScheduler(PluginDefinition myRequestSchedulerPlugin)
        {
            if (myRequestSchedulerPlugin != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<IRequestScheduler>(myRequestSchedulerPlugin.NameOfPlugin, myRequestSchedulerPlugin.PluginParameter);
            }

            //so lets take the default one
            var defaultRequestSchedulerName = _applicationSettings.Get<DefaultRequestSchedulerImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<IRequestScheduler>(defaultRequestSchedulerName);
        }

        /// <summary>
        /// Create a meta manager
        /// </summary>
        /// <param name="myPlugins">The plugin definitions</param>
        /// <returns>A meta manager</returns>
        private IMetaManager CreateMetamanager(GraphDBPlugins myPlugins)
        {
            return new MetaManager(_securityManager, myPlugins, _graphDBPluginManager, _applicationSettings);
        }
        
        /// <summary>
        /// Loads the security manager
        /// </summary>
        /// <param name="mySecurityManagerPlugin">The security manager plugin definition</param>
        private ISecurityManager LoadSecurityManager(PluginDefinition mySecurityManagerPlugin)
        {
            if (mySecurityManagerPlugin != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<ISecurityManager>(mySecurityManagerPlugin.NameOfPlugin, mySecurityManagerPlugin.PluginParameter);
            }

            //so lets take the default one
            var defaultSecurityManagerName = _applicationSettings.Get<DefaultSecurityManagerImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<ISecurityManager>(defaultSecurityManagerName, new Dictionary<string, object> { { "vertexStore", _transactionManager } });
        }

        /// <summary>
        /// Load the transaction manager
        /// </summary>
        /// <param name="myTransactionManagerPlugin">The transaction manager plugin definition</param>
        private ITransactionManager LoadTransactionManagerPlugin(PluginDefinition myTransactionManagerPlugin)
        {
            if (myTransactionManagerPlugin != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<ITransactionManager>(myTransactionManagerPlugin.NameOfPlugin, myTransactionManagerPlugin.PluginParameter);
            }
            
            //so there is no given plugin... lets try the IGraphFS
            if (_iGraphFS.IsTransactional)
            {
                return (ITransactionManager)_iGraphFS;
            }
            
            //so lets take the default one
            var defaultTransactionManagerName = _applicationSettings.Get<DefaultTransactionManagerImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<ITransactionManager>(defaultTransactionManagerName, new Dictionary<string, object> { { "vertexStore", _iGraphFS } });
        }

        /// <summary>
        /// Loads the IGraphFS
        /// </summary>
        /// <param name="myIGraphFSDefinition">The IGraphFS plugin definition</param>
        private IGraphFS LoadGraphFsPlugin(PluginDefinition myIGraphFSDefinition)
        {
            if (myIGraphFSDefinition != null)
            {
                return _graphDBPluginManager.GetAndInitializePlugin<IGraphFS>(myIGraphFSDefinition.NameOfPlugin, myIGraphFSDefinition.PluginParameter);
            }

            //return the default fs
            var defaultFSName = _applicationSettings.Get<DefaultGraphFSImplementation>();
            return _graphDBPluginManager.GetAndInitializePlugin<IGraphFS>(defaultFSName);
        }

        #endregion
    }
}
