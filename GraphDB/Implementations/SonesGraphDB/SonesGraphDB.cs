using System;
using sones.GraphDB.Manager;
using sones.GraphDB.Request;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphFS;
using sones.Library.VersionedPluginManager;
using sones.GraphDB.Manager.Transaction;
using sones.GraphDB.Manager.Security;
using sones.Library.Settings;

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
        private IRequestManager _requestManager;

        /// <summary>
        /// A manager to dynamically load versioned plugins
        /// </summary>
        private PluginManager _pluginManager;

        /// <summary>
        /// A manager that is responsible for transactions
        /// </summary>
        private ITransactionManager _transactionManager;

        /// <summary>
        /// A manager that is responsible for security
        /// </summary>
        private ISecurityManager _securityManager;

        /// <summary>
        /// The persistence layer
        /// </summary>
        private IGraphFS _iGraphFS;

        /// <summary>
        /// A globally unique identifier for this graphdb instance
        /// </summary>
        private readonly Guid _id;

        /// <summary>
        /// The settings of the graph db
        /// </summary>
        private readonly GraphApplicationSettings _applicationSettings;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones graphdb instance
        /// </summary>
        /// <param name="mySettings">The application settings</param>
        /// <param name="myIGraphFSDefinition">The definition of the IGraphFS plugin</param>
        /// <param name="myTransactionManagerPlugin">The definition of the transaction manager plugin</param>
        /// <param name="mySecurityManagerPlugin">The definition of the transaction manager plugin</param>
        /// <param name="myRequestSchedulerPlugin">The definition of the request scheduler plugin</param>
        /// <param name="myLogicExpressionOptimizerPlugin">The definition of the logic expression optimizer plugin</param>
        /// <param name="myRequestManagerPlugin">The definition of the request manager plugin</param>
        public SonesGraphDB(
            GraphApplicationSettings mySettings,
            PluginDefinition myIGraphFSDefinition,
            PluginDefinition myTransactionManagerPlugin         = null,
            PluginDefinition mySecurityManagerPlugin            = null,
            PluginDefinition myRequestSchedulerPlugin           = null,
            PluginDefinition myLogicExpressionOptimizerPlugin   = null,
            PluginDefinition myRequestManagerPlugin             = null)
        {
            _id = Guid.NewGuid();

            #region plugin manager

            InitializePluginManager();
            
            #endregion

            #region IGraphFS

            LoadGraphFsPlugin(myIGraphFSDefinition);

            #endregion

            #region transaction

            LoadTransactionManagerPlugin(myTransactionManagerPlugin);

            #endregion

            #region security

            LoadSecurityManager(mySecurityManagerPlugin);

            #endregion

            #region requests

            LoadRequestManager(myRequestManagerPlugin, myRequestSchedulerPlugin);

            #endregion

            #region expression optimizer

            LoadLogicExpressionOptimizer(myLogicExpressionOptimizerPlugin);

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
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private helper

        /// <summary>
        /// Load an logic expression optimizer
        /// </summary>
        /// <param name="myLogicExpressionOptimizerPlugin">A plugin definition</param>
        private void LoadLogicExpressionOptimizer(PluginDefinition myLogicExpressionOptimizerPlugin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load a request manager
        /// </summary>
        /// <param name="myRequestManagerPlugin">The actual request manager plugin definition</param>
        /// <param name="myRequestSchedulerPlugin">The request scheduler plugin that is needed for the request manager</param>
        private void LoadRequestManager(PluginDefinition myRequestManagerPlugin, PluginDefinition myRequestSchedulerPlugin)
        {
            MetaManager metaManager = CreateMetamanager();

            IRequestScheduler requestScheduler = LoadRequestScheduler(myRequestSchedulerPlugin);

            //load values via settings
            //_requestManager = new RequestManager()

            throw new NotImplementedException();

        }

        /// <summary>
        /// Load a request scheduler
        /// </summary>
        /// <param name="myRequestSchedulerPlugin">The scheduler plugin definition</param>
        /// <returns>A request scheduler</returns>
        private IRequestScheduler LoadRequestScheduler(PluginDefinition myRequestSchedulerPlugin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a meta manager
        /// </summary>
        /// <returns>A meta manager</returns>
        private MetaManager CreateMetamanager()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Loads the security manager
        /// </summary>
        /// <param name="mySecurityManagerPlugin">The security manager plugin definition</param>
        private void LoadSecurityManager(PluginDefinition mySecurityManagerPlugin)
        {
            _securityManager = new BasicSecurityManager(_transactionManager);
        }

        /// <summary>
        /// Initialize the plugin manager for the sones GraphDB
        /// </summary>
        private void InitializePluginManager()
        {
            _pluginManager = new PluginManager();

            // Change the version if there are ANY changes which will prevent loading the plugin.
            // As long as there are still some plugins which does not have their own assembly you need to change the compatibility of ALL plugins of the GraphDB and GraphFSInterface assembly.
            // So, if any plugin in the GraphDB changes you need to change the AssemblyVersion of the GraphDB AND modify the compatibility version of the other plugins.
            _pluginManager = new PluginManager()
                .Register<IGraphFS>(IGraphFSVersionCompatibility.MinVersion, IGraphFSVersionCompatibility.MaxVersion)
                .Register<ITransactionManager>(ITransactionManagerVersionCompatibility.MinVersion, ITransactionManagerVersionCompatibility.MaxVersion)
                .Register<ISecurityManager>(ISecurityManagerVersionCompatibility.MinVersion, ISecurityManagerVersionCompatibility.MaxVersion)
                .Register<IRequestScheduler>(IRequestSchedulerVersionCompatibility.MinVersion, IRequestSchedulerVersionCompatibility.MaxVersion)
                .Register<IRequestManager>(IRequestManagerVersionCompatibility.MinVersion, IRequestManagerVersionCompatibility.MaxVersion)
                .Register<ILogicExpressionOptimizer>(ILogicExpressionOptimizerVersionCompatibility.MinVersion, ILogicExpressionOptimizerVersionCompatibility.MaxVersion);

            _pluginManager.Discover();
        }

        /// <summary>
        /// Load the transaction manager
        /// </summary>
        /// <param name="myTransactionManagerPlugin">The transaction manager plugin definition</param>
        private void LoadTransactionManagerPlugin(PluginDefinition myTransactionManagerPlugin)
        {
            if (_iGraphFS.IsTransactional)
            {
                _transactionManager = (ITransactionManager)_iGraphFS;
            }
            else
            {
                _transactionManager = new BasicTransactionManager(_iGraphFS);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the IGraphFS
        /// </summary>
        /// <param name="myIGraphFSDefinition">The IGraphFS plugin definition</param>
        private void LoadGraphFsPlugin(PluginDefinition myIGraphFSDefinition)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}