using System;
using sones.GraphDB.Request;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using sones.Plugins.GraphDS.IO.XML_IO;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.Request.GetIndex;
using sones.GraphDB.Request.Delete;
using sones.GraphDB.Request.Update;
using sones.GraphDB.Request.DropType;
using sones.GraphDB.Request.DropIndex;
using sones.GraphDB.Request.CreateIndex;
using sones.GraphDB.Request.RebuildIndices;

namespace sones.GraphDS.GraphDSRESTClient
{
    /// <summary>
    /// A GraphDS client that communicates via REST
    /// </summary>
    public sealed class GraphDS_RESTClient : IGraphDSClient
    {
        #region Data
        private readonly String _Username;

        private readonly String _Password;

        private readonly NetworkCredential _Credentials;

        private String _Host;
        
        private String getHost() 
        {
         return this._Host; 
        }
         
        private void setHost(String myHost){
            if (!myHost.Contains("http://"))
            {
                _Host = "http://" + myHost;
            }
            else
            {
                _Host = myHost;
            }
        }
        
        private readonly uint   _Port;

        private readonly String _GQLPATTERN = "/gql";

        private readonly String _GQLUri;

        private XML_IO _Parser;

        #endregion

        #region Constructors

        public GraphDS_RESTClient(String myHost, String myUsername, String myPassword, uint myPort = 9975U)
        {
            setHost(myHost);
            _Username = myUsername;
            _Password = myPassword;
            _Port = myPort;
            _Credentials = new NetworkCredential(myUsername, myPassword);
            
            _GQLUri = getHost() + ":" + _Port.ToString() + _GQLPATTERN;

            _Parser = new XML_IO();
         }
        #endregion

        #region IGraphDS Members

        public void Shutdown(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            QueryResult result = null;
            String resonseXML = FetchGraphDBOutput(myQueryString);
            result = _Parser.GenerateQueryResult(resonseXML);
            return result;
        }

        

        #endregion

        #region IGraphDB Members / Not Implemented

        public TResult CreateVertexTypes<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Delete<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDelete myRequestDelete, Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(SecurityToken mySecurity, TransactionToken myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllVertexTypes<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllVertexTypes myRequestGetAllVertexTypes, Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllEdgeTypes<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Truncate<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexType myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Update<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestUpdate myRequestUpdate, Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDropType myRequestDropType, Converter.DropTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropIndex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDropIndex myRequestDropIndex, Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateIndex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateIndex myRequestCreateIndex, Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult RebuildIndices<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestRebuildIndices myRequestRebuildIndices, Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ITransactionable Members / Not Implemented

        public TransactionToken BeginTransaction(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members / Not Implemented

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utilities

        private String FetchGraphDBOutput(String myQueryString)
        {
            String fullRESTUri = _GQLUri + "?" + HttpUtility.HtmlEncode(myQueryString);
            try
            {

                HttpWebRequest request = WebRequest.Create(fullRESTUri) as HttpWebRequest;

                request.Credentials = _Credentials;
                request.ContentType = "application/xml";
                request.UserAgent = "GraphDSRESTClient";
                request.KeepAlive = false;
                StreamReader reader;
                StringBuilder resonseXML = null;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;



                if (request.HaveResponse == true && response != null)
                {
                    reader = new StreamReader(response.GetResponseStream());
                    resonseXML = new StringBuilder(reader.ReadToEnd());
                }
                return resonseXML.ToString();

            }
            catch (WebException ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
