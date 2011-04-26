using System;
using System.Net;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Networking.HTTP;
using System.IdentityModel.Selectors;
using sones.Plugins.GraphDS.RESTService;
using System.ServiceModel;

namespace sones.GraphDSServer
{
    public sealed class GraphDSServer : IGraphDSServer
    {
        #region Data

        /// <summary>
        /// The internal iGraphDB instance
        /// </summary>
        private readonly IGraphDB                  _iGraphDB;
        private HTTPServer<GraphDSREST_Service>    _httpServer;
        private UserNamePasswordValidator          _httpCredentials;

        #endregion

        #region Constructor

        public GraphDSServer(IGraphDB myGraphDB)
        {
            _iGraphDB = myGraphDB;
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
                        UserNamePasswordValidator = _httpCredentials
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

                // Register the REST service within the list of services
                // to stop before shutting down the GraphDSSharp instance
                /*myAGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
                {
                    _HttpWebServer.StopAndWait();
                });*/                

            }
            catch (Exception e)
            {
                throw e;
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

        #endregion

        #region IGraphDS Members

        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
        
            _httpServer.StopAndWait();
            _httpServer.Dispose(); 
        }

        public QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphDB Members

        public TResult CreateVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, TransactionToken myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ITransactionable Members

        public TransactionToken BeginTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members

        public sones.Library.Commons.Security.SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            _httpCredentials = toBeAuthenticatedCredentials.CreateHttpCredentials();
            //var credentials = _iGraphDB.LogOn(toBeAuthenticatedCredentials);
            return null;
        }

        public void LogOff(sones.Library.Commons.Security.SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
