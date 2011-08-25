using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;
using sones.Plugins.GraphDS;
using sones.GraphQL.Result;
using System.IO;

namespace QueryLog
{
    public class QueryLog: IDrainPipe, IPluginable
    {
        private StreamWriter _logFile;

        public QueryLog()
        {
        }

        public QueryLog(string myUniqueString, Dictionary<string, object> myParameters)
        {
            if (myParameters == null)
                throw new ArgumentNullException("myParameters");

            #region AppendLogPathAndName

            var path = (myParameters.ContainsKey("AppendLogPath"))
                                       ? (String) myParameters["AppendLogPath"]
                                       : "sones.drainpipelog";

            path = Path.Combine(path, myUniqueString) + ".log";

            

            #endregion

            var dir = Path.GetDirectoryName(path);
            
            if (dir != null)
                Directory.CreateDirectory(dir);

            _logFile = File.CreateText(path);
            
        }

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.querylog"; }
        }

        public string PluginShortName
        {
            get { return "querylog"; }
        }

        public string PluginDescription
        {
            get { return "This class realizes a query log."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type> { { "AppendLogPath", typeof(String) } }; }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var result = new QueryLog(myUniqueString, myParameters);
            return (IPluginable)result;
        }

        public void Dispose()
        { }

        #endregion

        #region IGraphDS Members

        public QueryResult Query(SecurityToken mySecurityToken, Int64 myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            _logFile.WriteLine(myQueryString);
            _logFile.Flush();
            return null;
        }

        #endregion

        #region IGraphDB Members

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            _logFile.Flush();
            _logFile.Close();
        }

        #endregion

        #region ITransactionable Members

        public Int64 BeginTransaction(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            throw new NotImplementedException();
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

        #region IReadWriteGraphDB Members

        public TResult CreateVertexTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexTypes, Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateVertexType myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult AlterVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestAlterVertexType myRequestAlterVertexType, Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateEdgeType myRequestCreateEdgeType, Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateEdgeTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateEdgeTypes myRequestCreateEdgeTypes, Converter.CreateEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult AlterEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestAlterEdgeType myRequestAlterEdgeType, Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Delete<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDelete myRequestDelete, Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Truncate<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Update<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestUpdate myRequestUpdate, Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDropVertexType myRequestDropType, Converter.DropVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDropEdgeType myRequestDropType, Converter.DropEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropIndex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDropIndex myRequestDropIndex, Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateIndex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateIndex myRequestCreateIndex, Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult RebuildIndices<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestRebuildIndices myRequestRebuildIndices, Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IReadOnlyGraphDB Members

        public TResult GetVertex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(SecurityToken mySecurity, Int64 myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllVertexTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetAllVertexTypes myRequestGetAllVertexTypes, Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllEdgeTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndices<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexCount<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertexCount myRequestGetVertexCount, Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void DrainQueryResult(QueryResult result)
        {
            DateTime now = DateTime.Now;

            String resultType;

            if (result != null)
            {
                 switch(result.TypeOfResult)
                 {
                     case ResultType.Failed:
                         resultType = "failed";
                         break;
                     case ResultType.Successful:
                         resultType = "successful";
                         break;
                     default:
                         resultType = "undetermined";
                         break;
                 }
                 String ErrorMessage = "";

                 if (result.Error != null)
                     ErrorMessage = (result.Error.Message + "(" + result.Error.StackTrace + ")").Replace("\n", "").Replace("\r", "");   // without any carriage returns or line feeds

                _logFile.WriteLine(now.ToShortDateString() + " " + now.ToShortTimeString() + " " + resultType + " " + result.Duration + " " + result.NumberOfAffectedVertices + " " + result.NameOfQuerylanguage + " '" + result.Query+ "' " + ErrorMessage);
            }
            else
            {
                _logFile.WriteLine(now.ToShortDateString() + " " + now.ToShortTimeString() + " failed 0 0 none 'none' no result was returned (error)");
            }

            _logFile.Flush();
        }
    }
}
