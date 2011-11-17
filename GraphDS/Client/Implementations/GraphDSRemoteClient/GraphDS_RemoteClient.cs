using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.GraphDSRESTClient;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using System.ServiceModel;
using System.Net;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;
using System.Diagnostics;
using sones.GraphDB.Request;
using sones.GraphQL.Result;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.TypeManagement;
using sones.Library.PropertyHyperGraph;
using System.ServiceModel.Description;
using System.Xml;
using sones.GraphDS.GraphDSRemoteClient.ErrorHandling;

namespace sones.GraphDS.GraphDSRemoteClient
{
    public class GraphDS_RemoteClient : IGraphDSClient, ITransactionable, IServiceToken
    {
        #region Data

        private GraphDSService _GraphDSService;
        private VertexTypeService _VertexTypeService;
        private VertexInstanceService _VertexInstanceService;
        private EdgeTypeService _EdgeTypeService;
        private EdgeInstanceService _EdgeInstanceService;
        private StreamedService _StreamedService;
        private SecurityToken _SecurityToken;
        private Int64 _TransactionToken;
        
        #endregion

        
        #region Constructor

        public GraphDS_RemoteClient(Uri myServiceAddress, bool myIsSecure = false)
        {
            BasicHttpBinding BasicBinding = new BasicHttpBinding();
            BasicBinding.Name = "sonesBasic";
            BasicBinding.MessageEncoding = WSMessageEncoding.Text;
            BasicBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            BasicBinding.MaxBufferSize = 268435456;
            BasicBinding.MaxReceivedMessageSize = 268435456;
            BasicBinding.SendTimeout = new TimeSpan(1, 0, 0);
            BasicBinding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxDepth = 2147483647;
            readerQuotas.MaxStringContentLength = 2147483647;
            readerQuotas.MaxBytesPerRead = 2147483647;
            readerQuotas.MaxNameTableCharCount = 2147483647;
            readerQuotas.MaxStringContentLength = int.MaxValue;
            readerQuotas.MaxArrayLength = 2147483647;
            BasicBinding.ReaderQuotas = readerQuotas;

            BasicHttpBinding StreamedBinding = new BasicHttpBinding();
            StreamedBinding.Name = "sonesStreamed";
            StreamedBinding.MessageEncoding = WSMessageEncoding.Text;
            StreamedBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            StreamedBinding.TransferMode = TransferMode.Streamed;
            StreamedBinding.MaxReceivedMessageSize = 2147483648;
            StreamedBinding.MaxBufferSize = 4096;
            StreamedBinding.SendTimeout = new TimeSpan(1, 0, 0, 0);

            if (myIsSecure == true)
            {
                BasicBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                StreamedBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            try
            {
                //request to test connection
                var Request = HttpWebRequest.Create(myServiceAddress.ToString());
                var Response = Request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new Exception("The GraphDB remote service is unreachable! Maybe the service was not started.", ex);
            }

            try
            {
                ChannelFactory<GraphDSService> factory = new ChannelFactory<GraphDSService>(BasicBinding, new EndpointAddress(myServiceAddress));
                foreach (var op in factory.Endpoint.Contract.Operations)
                {
                    DataContractSerializerOperationBehavior dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                    if (dataContractBehavior != null)
                    {
                        dataContractBehavior.MaxItemsInObjectGraph = 2147483647;
                    }
                }
                _GraphDSService = factory.CreateChannel();
                
                _VertexTypeService = ChannelFactory<VertexTypeService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _VertexInstanceService = ChannelFactory<VertexInstanceService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _EdgeTypeService = ChannelFactory<EdgeTypeService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _EdgeInstanceService = ChannelFactory<EdgeInstanceService>.CreateChannel(BasicBinding, new EndpointAddress(myServiceAddress));
                _StreamedService = ChannelFactory<StreamedService>.CreateChannel(StreamedBinding, new EndpointAddress(myServiceAddress + "/streamed"));
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

        public GraphDSService GraphDSService
        {
            get { return _GraphDSService; }
        }

        public StreamedService StreamedService
        {
            get { return _StreamedService; }
        }

        #endregion


        #region IUserAuthentication

        public SecurityToken LogOn(IUserCredentials myUserCredentials)
        {
            if (myUserCredentials is RemoteUserPasswordCredentials)
            {
                _SecurityToken = _GraphDSService.LogOn(((RemoteUserPasswordCredentials)myUserCredentials).ServiceObject);
                return _SecurityToken;
            }
            return null;
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

        public IQueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            return _GraphDSService.Query(mySecurityToken, myTransactionToken, myQueryString, myQueryLanguageName).ToQueryResult(this);
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
            if (myRequestInsert.BinaryProperties != null)
                foreach (var item in myRequestInsert.BinaryProperties)
                {
                    _StreamedService.SetBinaryProperty(new SetBinaryPropertyMessage(item.Key, _SecurityToken, _TransactionToken, svcVertex.VertexID, svcVertex.TypeID, item.Value));
                }
            
            
            var vertex = new RemoteVertex(svcVertex, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertex);
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.RequestTruncate myRequestTruncate, sones.GraphDB.Request.Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            _GraphDSService.TruncateVertexType(mySecurityToken, myTransactionID, myRequestTruncate.VertexTypeName);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)));
        }

        public TResult Update<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestUpdate myRequestUpdate, sones.GraphDB.Request.Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertices = _GraphDSService.Update(
                mySecurityToken,
                myTransactionID,
                new ServiceVertexType(myRequestUpdate.GetVerticesRequest.VertexTypeName),
                myRequestUpdate.GetVerticesRequest.VertexIDs.ToList(),
                new ServiceUpdateChangeset(myRequestUpdate));
            var vertices = svcVertices.Select(x => new RemoteVertex(x, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertices);
        }

        public TResult DropVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDropVertexType myRequestDropType, sones.GraphDB.Request.Converter.DropVertexTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var result = _GraphDSService.DropVertexType(mySecurityToken, myTransactionID, new ServiceVertexType(myRequestDropType.TypeName));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), result);
        }

        public TResult DropEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDropEdgeType myRequestDropType, sones.GraphDB.Request.Converter.DropEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var result = _GraphDSService.DropEdgeType(mySecurityToken, myTransactionID, new ServiceEdgeType(myRequestDropType.TypeName));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), result);
        }

        public TResult DropIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDropIndex myRequestDropIndex, sones.GraphDB.Request.Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            _GraphDSService.DropIndex(mySecurityToken, myTransactionID, new ServiceVertexType(myRequestDropIndex.TypeName), myRequestDropIndex.IndexName, myRequestDropIndex.Edition);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)));
        }

        public TResult CreateIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestCreateIndex myRequestCreateIndex, sones.GraphDB.Request.Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcIndexDef = _GraphDSService.CreateIndex(mySecurityToken, myTransactionID, new ServiceIndexPredefinition(myRequestCreateIndex.IndexDefinition));
            var indexDef = new RemoteIndexDefinition(svcIndexDef, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), indexDef);
        }

        public TResult RebuildIndices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestRebuildIndices myRequestRebuildIndices, sones.GraphDB.Request.Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            _GraphDSService.RebuildIndices(mySecurityToken, myTransactionID, myRequestRebuildIndices.Types.ToList());
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)));
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertex myRequestGetVertex, sones.GraphDB.Request.Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertex = _GraphDSService.GetVertex(mySecurityToken, myTransactionID, new ServiceVertexType(myRequestGetVertex.VertexTypeName), myRequestGetVertex.VertexID);
            var vertex = new RemoteVertex(svcVertex, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertex);
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertices myRequestGetVertices, sones.GraphDB.Request.Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            List<ServiceVertexInstance> svcVertices;
            if (myRequestGetVertices.VertexTypeName != null)
            {
                svcVertices = _GraphDSService.GetVerticesByType(mySecurityToken, myTransactionID, new ServiceVertexType(myRequestGetVertices.VertexTypeName));
            }
            else
            {
                svcVertices = _GraphDSService.GetVerticesByExpression(mySecurityToken, myTransactionID, ConvertHelper.ToServiceExpression(myRequestGetVertices.Expression));
            }
            var vertices = svcVertices.Select(x => new RemoteVertex(x, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertices);
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, long myTransactionID, sones.GraphDB.Request.RequestTraverseVertex myRequestTraverseVertex, sones.GraphDB.Request.Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertexType myRequestGetVertexType, sones.GraphDB.Request.Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            ServiceVertexType svcVertexType;
            if(myRequestGetVertexType.VertexTypeName != null)
                svcVertexType = _GraphDSService.GetVertexTypeByName(mySecurityToken, myTransactionID, myRequestGetVertexType.VertexTypeName);
            else
                svcVertexType = _GraphDSService.GetVertexTypeByID(mySecurityToken, myTransactionID, myRequestGetVertexType.VertexTypeID);
            var vertexType = new RemoteVertexType(svcVertexType, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertexType);
        }

        public TResult GetAllVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetAllVertexTypes myRequestGetAllVertexTypes, sones.GraphDB.Request.Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcVertexTypes = _GraphDSService.GetAllVertexTypes(mySecurityToken, myTransactionID, myRequestGetAllVertexTypes.Edition);
            var vertexTypes = svcVertexTypes.Select(x => new RemoteVertexType(x, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertexTypes);
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetEdgeType myRequestGetEdgeType, sones.GraphDB.Request.Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            ServiceEdgeType svcEdgeType;
            if (myRequestGetEdgeType.EdgeTypeName != null)
                svcEdgeType = _GraphDSService.GetEdgeTypeByName(mySecurityToken, myTransactionID, myRequestGetEdgeType.EdgeTypeName, myRequestGetEdgeType.Edition);
            else
                svcEdgeType = _GraphDSService.GetEdgeTypeByID(mySecurityToken, myTransactionID, myRequestGetEdgeType.EdgeTypeID, myRequestGetEdgeType.Edition);
            var edgeType = new RemoteEdgeType(svcEdgeType, this);
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), edgeType);
        }

        public TResult GetAllEdgeTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, sones.GraphDB.Request.Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcEdgeTypes = _GraphDSService.GetAllEdgeTypes(mySecurityToken, myTransactionID, myRequestGetAllEdgeTypes.Edition);
            var edgeTypes = svcEdgeTypes.Select(x => new RemoteEdgeType(x, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), edgeTypes);
        }

        public TResult DescribeIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDescribeIndex myRequestDescribeIndex, sones.GraphDB.Request.Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcIndex = _GraphDSService.DescribeIndex(mySecurityToken, myTransactionID, myRequestDescribeIndex.TypeName, myRequestDescribeIndex.IndexName);
            List<IIndexDefinition> index = new List<IIndexDefinition>();
            index.Add(new RemoteIndexDefinition(svcIndex, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), index);
        }

        public TResult DescribeIndices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestDescribeIndex myRequestDescribeIndex, sones.GraphDB.Request.Converter.DescribeIndicesResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var svcIndices = _GraphDSService.DescribeIndices(mySecurityToken, myTransactionID, myRequestDescribeIndex.TypeName);
            var indices = svcIndices.Select(x => new RemoteIndexDefinition(x, this));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), indices);
        }

        public TResult GetVertexCount<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, long myTransactionID, sones.GraphDB.Request.RequestGetVertexCount myRequestGetVertexCount, sones.GraphDB.Request.Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            Stopwatch RunningTime = Stopwatch.StartNew();
            var vertexCount = _GraphDSService.GetVertexCount(mySecurityToken, myTransactionID, new ServiceVertexType(myRequestGetVertexCount.VertexTypeName));
            RunningTime.Stop();
            return myOutputconverter(new RequestStatistics(new TimeSpan(RunningTime.ElapsedTicks)), vertexCount);
        }

        #endregion
    }
}
