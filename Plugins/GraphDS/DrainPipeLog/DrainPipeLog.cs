using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Request;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.VersionedPluginManager;
using sones.Plugins.GraphDS.DrainPipeLog.Storage;

namespace sones.Plugins.GraphDS.DrainPipeLog
{
    /// <summary>
    /// this is a GraphDS plugin which can be used to create a GraphDS bypass if you like. This
    /// plugin will be notified of each and every GQL and API query and can react uppon this
    /// </summary>
    public class DrainPipeLog : IGraphDS, IPluginable
    {
        private AppendLog _AppendLog = null;

        #region IPluginable
        public string PluginName
        {
            get { return "DrainPipeLog"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type> 
                { 
                    { "AppendLogPathAndName", typeof(String) },
                };
            }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new DrainPipeLog();
        }
        #endregion
        
        #region IGraphDS
        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public sones.GraphQL.Result.QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, sones.Library.Commons.Transaction.TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
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
            throw new NotImplementedException();
        }

        public void LogOff(sones.Library.Commons.Security.SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
