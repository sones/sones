using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.GraphDSRESTClient;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using System.ServiceModel;
using System.Net;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using GraphDSRemoteClient.GraphElements;
using System.Diagnostics;
using sones.GraphDB.Request;
using sones.GraphQL.Result;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient
{
    class GraphDS_RemoteClient : IGraphDSClient, ITransactionable, IServiceToken
    {
        #region Data

        private GraphDS _GraphDSService;
        private VertexTypeService _VertexTypeService;
        private VertexInstanceService _VertexInstanceService;
        private EdgeTypeService _EdgeTypeService;
        private EdgeInstanceService _EdgeInstanceService;
        private SecurityToken _SecurityToken;
        private Int64 _TransactionToken;
        
        #endregion

        
        #region Constructor

        public GraphDS_RemoteClient(Uri myServiceAddress)
        {
            BasicHttpBinding BasicBinding = new BasicHttpBinding();
            BasicBinding.Name = "sonesBasic";
            BasicBinding.MessageEncoding = WSMessageEncoding.Text;
            BasicBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            try
            {
                var Request = HttpWebRequest.Create(myServiceAddress.ToString());
                var Response = Request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new Exception("The GraphDB remote service is unreachable! Maybe the service was not started.", ex);
            }

            try
            { 
                _GraphDSService = ChannelFactory<GraphDS>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _VertexTypeService = ChannelFactory<VertexTypeService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _VertexInstanceService = ChannelFactory<VertexInstanceService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _EdgeTypeService = ChannelFactory<EdgeTypeService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _EdgeInstanceService = ChannelFactory<EdgeInstanceService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
            }
            catch (Exception ex)
            {
                throw new Exception("The GraphDB is reachably but there occured an error, creating the native services!", ex);
            }
        }

        #endregion


        #region IServiceToken

        public SecurityToken SecurityToken
        {
            get { return _SecurityToken; }
        }
        public Int64 TransactionToken
        {
            get { return _TransactionToken; }
        }

        public VertexTypeService VertexTypeService
        {
            get { return _VertexTypeService; }
        }

        public VertexInstanceService VertexService
        {
            get { return _VertexInstanceService; }
        }

        public EdgeTypeService EdgeTypeService
        {
            get { return _EdgeTypeService; }
        }

        public EdgeInstanceService EdgeService
        {
            get { return _EdgeInstanceService; }
        }

        public GraphDS GraphDSService
        {
            get { return _GraphDSService; }
        }

        #endregion


        #region IUserAuthentication

        public SecurityToken LogOn(IUserCredentials myUserCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken mySecurityToken)
        {
            _GraphDSService.LogOff(mySecurityToken);
        }

        #endregion


        #region ITransactionable

        public long BeginTransaction(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            _TransactionToken = _GraphDSService.BeginTransaction(mySecurityToken);
            return _TransactionToken;
        }

        public void CommitTransaction(SecurityToken mySecurityToken, long myTransactionID)
        {
            _GraphDSService.CommitTransaction(mySecurityToken, myTransactionID);
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, long myTransactionID)
        {
            _GraphDSService.RollbackTransaction(mySecurityToken, myTransactionID);
        }

        #endregion


        #region IGraphDSClient

        public sones.GraphQL.Result.QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            return _GraphDSService.Query(mySecurityToken, myTransactionToken, myQueryString, myQueryLanguageName).ToQueryResult();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
            _GraphDSService.Shutdown(mySecurityToken);
        }

        public TResult CreateVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestCreateVertexTypes myRequestCreateVertexTypes, sones.GraphDB.Request.Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertexTypes = _GraphDSService.CreateVertexTypes(mySecurityToken, myTransactionID, myRequestCreateVertexTypes.VertexTypeDefinitions.Select(x => new ServiceVertexTypePredefinition(x)).ToList());
            var vertexTypes = svcVertexTypes.Select(x => new RemoteVertexType(x, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertexTypes);
        }

        public TResult CreateVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestCreateVertexType myRequestCreateVertexType, sones.GraphDB.Request.Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertexType = _GraphDSService.CreateVertexType(mySecurityToken, myTransactionID, new ServiceVertexTypePredefinition(myRequestCreateVertexType.VertexTypeDefinition));
            var vertexType = new RemoteVertexType(svcVertexType, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertexType);
        }

        public TResult AlterVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestAlterVertexType myRequestAlterVertexType, sones.GraphDB.Request.Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertexType = _GraphDSService.AlterVertexType(mySecurityToken, myTransactionID,
                new ServiceVertexType(myRequestAlterVertexType.TypeName),
                new ServiceAlterVertexChangeset(myRequestAlterVertexType));
            var vertexType = new RemoteVertexType(svcVertexType, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertexType);
        }

        public TResult CreateEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestCreateEdgeType myRequestCreateEdgeType, sones.GraphDB.Request.Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcEdgeType = _GraphDSService.CreateEdgeType(mySecurityToken, myTransactionID,
                new ServiceEdgeTypePredefinition((EdgeTypePredefinition)myRequestCreateEdgeType.EdgeTypePredefinition));
            var edgeType = new RemoteEdgeType(svcEdgeType, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), edgeType);
        }

        public TResult CreateEdgeTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestCreateEdgeTypes myRequestCreateEdgeTypes, sones.GraphDB.Request.Converter.CreateEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcEdgeTypes = _GraphDSService.CreateEdgeTypes(mySecurityToken, myTransactionID, myRequestCreateEdgeTypes.TypePredefinitions.Select(x => new ServiceEdgeTypePredefinition((EdgeTypePredefinition)x)).ToList());
            var edgeTypes = svcEdgeTypes.Select(x => new RemoteEdgeType(x, this)).ToList();
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), edgeTypes);
        }

        public TResult AlterEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestAlterEdgeType myRequestAlterEdgeType, sones.GraphDB.Request.Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcEdgeType = _GraphDSService.AlterEdgeType(mySecurityToken, myTransactionID,
                new ServiceEdgeType(myRequestAlterEdgeType.TypeName),
                new ServiceAlterEdgeChangeset(myRequestAlterEdgeType));
            var edgeType = new RemoteEdgeType(svcEdgeType, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), edgeType);
        }

        public TResult Clear<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestClear myRequestClear, sones.GraphDB.Request.Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var result = _GraphDSService.Clear(mySecurityToken, myTransactionID);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), result);
        }

        public TResult Delete<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDelete myRequestDelete, sones.GraphDB.Request.Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var result = _GraphDSService.Delete(mySecurityToken, myTransactionID,
                new ServiceVertexType(myRequestDelete.ToBeDeletedVertices.VertexTypeName),
                myRequestDelete.ToBeDeletedVertices.VertexIDs.ToList(),
                new ServiceDeletePayload(myRequestDelete));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), result.Item1.Select(x => (IComparable)x), result.Item2.Select(x => (IComparable)x));
        }

        public TResult Insert<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestInsertVertex myRequestInsert, sones.GraphDB.Request.Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertex = _GraphDSService.Insert(mySecurityToken, myTransactionID, myRequestInsert.VertexTypeName, new ServiceInsertPayload(myRequestInsert));
            var vertex = new RemoteVertex(svcVertex, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertex);
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.RequestTruncate myRequestTruncate, sones.GraphDB.Request.Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Update<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestUpdate myRequestUpdate, sones.GraphDB.Request.Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDropVertexType myRequestDropType, sones.GraphDB.Request.Converter.DropVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDropEdgeType myRequestDropType, sones.GraphDB.Request.Converter.DropEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDropIndex myRequestDropIndex, sones.GraphDB.Request.Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestCreateIndex myRequestCreateIndex, sones.GraphDB.Request.Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult RebuildIndices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestRebuildIndices myRequestRebuildIndices, sones.GraphDB.Request.Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertex myRequestGetVertex, sones.GraphDB.Request.Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertices myRequestGetVertices, sones.GraphDB.Request.Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, long myTransactionID, sones.GraphDB.Request.RequestTraverseVertex myRequestTraverseVertex, sones.GraphDB.Request.Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertexType myRequestGetVertexType, sones.GraphDB.Request.Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetAllVertexTypes myRequestGetAllVertexTypes, sones.GraphDB.Request.Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetEdgeType myRequestGetEdgeType, sones.GraphDB.Request.Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllEdgeTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, sones.GraphDB.Request.Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDescribeIndex myRequestDescribeIndex, sones.GraphDB.Request.Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDescribeIndex myRequestDescribeIndex, sones.GraphDB.Request.Converter.DescribeIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexCount<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertexCount myRequestGetVertexCount, sones.GraphDB.Request.Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
