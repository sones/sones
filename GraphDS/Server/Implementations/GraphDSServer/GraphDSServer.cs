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

namespace sones.GraphDSServer
{
    internal class PasswordValidator : UserNamePasswordValidator
    {
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
        /// The list of supported graph ql types.
        /// </summary>
        private readonly Dictionary<String, IGraphQL>   _queryLanguages;

        #endregion

        #region Constructor

        /// <summary>
        /// The GraphDS server constructor.
        /// </summary>
        /// <param name="myGraphDB">The graph db instance.</param>
        public GraphDSServer(IGraphDB myGraphDB)
        {
            _iGraphDB = myGraphDB;
            _pluginManager = new GraphDSPluginManager();
            _ID = new Guid();
            _queryLanguages = new Dictionary<string, IGraphQL>();

            var qlPluginNames = _pluginManager.GetPluginsForType<IGraphQL>();
            var languageParameters = new Dictionary<String, Object>();
            languageParameters.Add("GraphDB", myGraphDB);

            foreach (var item in qlPluginNames)
            {
                _queryLanguages.Add(item, _pluginManager.GetAndInitializePlugin<IGraphQL>(item, languageParameters));
            }
        }

        #endregion

        #region IGraphDSREST Members

        public void StartRESTService(string myServiceID, ushort myPort, IPAddress myIPAddress)
        {
            try
            {
                    var security = new HTTPSecurity()                     
                    {
                        CredentialType = HttpClientCredentialType.Basic,
                        UserNamePasswordValidator = Validator
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

        public UserNamePasswordValidator Validator
        {
            get { return new PasswordValidator(); }
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

            if (_queryLanguages.TryGetValue(myQueryLanguageName, out queryLanguage))
            {
                return queryLanguage.Query(mySecurityToken, myTransactionToken, myQueryString);
            }
            else
            {
                throw new QueryLanguageNotFoundException(String.Format("The GraphDS server does not support the query language {0}", myQueryLanguageName));
            }
        }

        #endregion

        #region IGraphDB Members

        public TResult CreateVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
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

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetEdgeType<TResult>(mySecurityToken, myTransactionToken, myRequestGetEdgeType,
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
