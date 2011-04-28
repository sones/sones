using System;
using System.IdentityModel.Tokens;
using System.Net;
using System.Collections.Generic;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDSServer.ErrorHandling;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Networking.HTTP;
using sones.Plugins.GraphDS.RESTService;
using sones.GraphDS.PluginManager.GraphDSPluginManager;
using sones.GraphQL;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.Request.GetIndex;
using sones.Plugins.GraphDS;
using sones.Library.VersionedPluginManager;
using sones.GraphDS.PluginManager;

namespace sones.GraphDSServer
{
    internal class PasswordValidator : UserNamePasswordValidator
    {
        private readonly IUserAuthentication _dbauth;

        public PasswordValidator(IUserAuthentication dbAuthentcator)
        {
            _dbauth = dbAuthentcator;
        }

        public override void Validate(string userName, string password)
        {
            if (!(userName == "test" && password == "test"))
            {
                throw new SecurityTokenException("Username or password incorrect.");
            }
        }
    }
    
    public sealed class GraphDSServer : IGraphDSServer
    {
        #region Data

        /// <summary>
        /// The internal iGraphDB instance
        /// </summary>
        private readonly IGraphDB                       _iGraphDB;

        /// <summary>
        /// The web server, which starts the REST service.
        /// </summary>
        private HTTPServer<GraphDSREST_Service>         _httpServer;

        /// <summary>
        /// The service guid.
        /// </summary>
        private readonly Guid                           _ID;

        /// <summary>
        /// The graph ds plugin manager.
        /// </summary>
        private readonly GraphDSPluginManager           _pluginManager;

        /// <summary>
        /// The list of supported graph ql type instances
        /// </summary>
        private readonly Dictionary<String, IGraphQL>   _QueryLanguages;
        private readonly Dictionary<String, ISonesRESTService> _sonesRESTServices;
        private readonly List<KeyValuePair<String,IDrainPipe>> _DrainPipes;

        #endregion

        #region Constructor

        /// <summary>
        /// The GraphDS server constructor.
        /// </summary>
        /// <param name="myGraphDB">The graph db instance.</param>
        /// <param name="myIPAddress">the IP adress this GraphDS Server should bind itself to</param>
        /// <param name="myPort">the port this GraphDS Server should listen on</param>
        /// <param name="PluginDefinitions">the plugins that shall be loaded and their according parameters</param>
        public GraphDSServer(IGraphDB myGraphDB, ushort myPort, IPAddress myIPAddress,GraphDSPlugins Plugins = null)
        {
            _iGraphDB = myGraphDB;
            _pluginManager = new GraphDSPluginManager();
            _ID = new Guid();
            _QueryLanguages = new Dictionary<string, IGraphQL>();
            _sonesRESTServices = new Dictionary<string, ISonesRESTService>();
            _DrainPipes = new List<KeyValuePair<String, IDrainPipe>>();

            #region Load Configured Plugins
            GraphDSPlugins _plugins = Plugins;

            if (_plugins == null)
            {
                #region set the defaults
                // which are: 
                //  GQL with GraphDB Parameter
                #region GQL
                List<PluginDefinition> QueryLanguages = new List<PluginDefinition>();
                Dictionary<string, object> GQL_Parameters = new Dictionary<string, object>();
                GQL_Parameters.Add("GraphDB", myGraphDB);

                QueryLanguages.Add(new PluginDefinition("sones.gql", GQL_Parameters));
                #endregion

                #region REST Service Plugins
                List<PluginDefinition> SonesRESTServices = new List<PluginDefinition>();
                #endregion

                _plugins = new GraphDSPlugins(SonesRESTServices, QueryLanguages, null);
                #endregion
            }

            // now at least the default plugins or a user setup is stored in the _plugins structure
            // iterate through and instantiate if found
            #region IGraphQL Plugins
            if (_plugins.IGraphQLPlugins != null)
            {
                // we got QL
                foreach (PluginDefinition _pd in _plugins.IGraphQLPlugins)
                {
                    // load!
                    IGraphQL loaded = LoadIGraphQLPlugins(_pd);
                    
                    // add!
                    if (loaded != null)
                    {
                        _QueryLanguages.Add(_pd.NameOfPlugin, loaded);
                    }
                    //                    else
                    //                        System.Diagnostics.Debug.WriteLine("Could not load plugin " + _pd.NameOfPlugin);
                }
            }
            #endregion

            #region ISonesRESTService Plugins
            if (_plugins.ISonesRESTServicePlugins != null)
            {
                // we got ISonesRESTServicePlugins
                foreach (PluginDefinition _pd in _plugins.ISonesRESTServicePlugins)
                {
                    // load!
                    ISonesRESTService loaded = LoadISonesRESTServicePlugins(_pd);
                    // add!
                    if (loaded != null)
                    {
                        _sonesRESTServices.Add(_pd.NameOfPlugin, loaded);
                    }
                    //                    else
                    //                        System.Diagnostics.Debug.WriteLine("Could not load plugin " + _pd.NameOfPlugin);
                }
            }
            #endregion

            #region IDrainPipe Plugins
            if (_plugins.IDrainPipePlugins != null)
            {
                // we got IDrainPipePlugins
                foreach (PluginDefinition _pd in _plugins.IDrainPipePlugins)
                {
                    // load!
                    IDrainPipe loaded = LoadIDrainPipes(_pd);
                    // add!
                    if (loaded != null)
                    {
                        _DrainPipes.Add(new KeyValuePair<string,IDrainPipe>(_pd.NameOfPlugin, loaded));
                    }
                    //                    else
                    //                        System.Diagnostics.Debug.WriteLine("Could not load plugin " + _pd.NameOfPlugin);

                }
            }
            #endregion

            #endregion

            try
            {
                var security = new HTTPSecurity()
                {
                    CredentialType = HttpClientCredentialType.Basic,
                    UserNamePasswordValidator = new PasswordValidator(_iGraphDB)
                };

                var restService = new GraphDSREST_Service();
                restService.Initialize(this, myPort, myIPAddress);

                _httpServer = new HTTPServer<GraphDSREST_Service>(
                                    myIPAddress,
                                    myPort,
                                    restService,
                                    myAutoStart: true)
                {
                    HTTPSecurity = security,
                };
            }
            catch (Exception e)
            {
                throw new RESTServiceCouldNotStartetException(e.Message);
            }
        }

        #endregion

        #region Plugin Loading Helpers
        /// <summary>
        /// Load the IDrainPipes plugins
        /// </summary>
        private IDrainPipe LoadIDrainPipes(PluginDefinition myIDrainPipesPlugin)
        {
            return _pluginManager.GetAndInitializePlugin<IDrainPipe>(myIDrainPipesPlugin.NameOfPlugin, myIDrainPipesPlugin.PluginParameter);
        }

        /// <summary>
        /// Load the IGraphQL plugins
        /// </summary>
        private IGraphQL LoadIGraphQLPlugins(PluginDefinition myIGraphQLPlugin)
        {
            return _pluginManager.GetAndInitializePlugin<IGraphQL>(myIGraphQLPlugin.NameOfPlugin, myIGraphQLPlugin.PluginParameter);
        }

        /// <summary>
        /// Load the ISonesRESTService plugins
        /// </summary>
        private ISonesRESTService LoadISonesRESTServicePlugins(PluginDefinition myISonesRESTServicePlugin)
        {
            return _pluginManager.GetAndInitializePlugin<ISonesRESTService>(myISonesRESTServicePlugin.NameOfPlugin, myISonesRESTServicePlugin.PluginParameter);
        }

        #endregion

        #region IGraphDSREST Members

        public void StartRESTService(string myServiceID, ushort myPort, IPAddress myIPAddress)
        {
            
        }

        public bool StopRESTService(string myServiceID)
        {
            try
            {
                _httpServer.StopAndWait();
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        #endregion

        #region IGraphDS Members

        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
            _httpServer.StopAndWait();
            _httpServer.Dispose(); 
        }

        public QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            IGraphQL queryLanguage;

            if (_QueryLanguages.TryGetValue(myQueryLanguageName, out queryLanguage))
            {
                // drain every query
                foreach (KeyValuePair<String,IDrainPipe> _drainpipe in _DrainPipes)
                {
                    _drainpipe.Value.Query(mySecurityToken, myTransactionToken, myQueryLanguageName, myQueryLanguageName);
                }

                return queryLanguage.Query(mySecurityToken, myTransactionToken, myQueryString);
            }
            else
            {
                throw new QueryLanguageNotFoundException(String.Format("The GraphDS server does not support the query language {0}", myQueryLanguageName));
            }
        }

        #endregion

        #region IGraphDB Members

        public TResult CreateVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.CreateVertexType<TResult>(mySecurityToken, myTransactionToken, myRequestCreateVertexType, myOutputconverter);
        }

        public TResult Clear<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Clear<TResult>(mySecurityToken, myTransactionToken, myRequestClear, myOutputconverter);
        }

        public TResult Insert<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Insert<TResult>(mySecurityToken, myTransactionToken, myRequestInsert, myOutputconverter);
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertices<TResult>(mySecurityToken, myTransactionToken, myRequestGetVertices, myOutputconverter);
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, TransactionToken myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.TraverseVertex<TResult>(mySecurity, myTransactionToken, myRequestTraverseVertex, myOutputconverter);
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertexType<TResult>(mySecurityToken, myTransactionToken, myRequestGetVertexType, myOutputconverter);
        }

        public TResult GetAllVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllVertexTypes myRequestGetAllVertexTypes, Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetAllVertexTypes<TResult>(mySecurityToken, myTransactionToken, myRequestGetAllVertexTypes, myOutputconverter);
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetEdgeType<TResult>(mySecurityToken, myTransactionToken, myRequestGetEdgeType,
                                                  myOutputconverter);
        }

        public TResult GetAllEdgeTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetAllEdgeTypes<TResult>(mySecurityToken, myTransactionToken, myRequestGetAllEdgeTypes,
                                                  myOutputconverter);
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertex<TResult>(mySecurityToken, myTransactionToken, myRequestGetVertex,
                                                myOutputconverter);
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Truncate<TResult>(mySecurityToken, myTransactionToken, myRequestTruncate, myOutputconverter);
        }

        public TResult DescribeIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { return _ID; }
        }

        #endregion

        #region ITransactionable Members

        public TransactionToken BeginTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            return _iGraphDB.BeginTransaction(mySecurityToken, myLongrunning, myIsolationLevel);
        }

        public void CommitTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _iGraphDB.CommitTransaction(mySecurityToken, myTransactionToken);
        }

        public void RollbackTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _iGraphDB.RollbackTransaction(mySecurityToken, myTransactionToken);
        }

        #endregion

        #region IUserAuthentication Members

        public sones.Library.Commons.Security.SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            return _iGraphDB.LogOn(toBeAuthenticatedCredentials);
        }

        public void LogOff(sones.Library.Commons.Security.SecurityToken toBeLoggedOfToken)
        {
            _iGraphDB.LogOff(toBeLoggedOfToken);
        }

        #endregion
    }
}
