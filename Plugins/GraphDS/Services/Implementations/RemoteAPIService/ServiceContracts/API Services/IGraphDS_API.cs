/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.PayloadObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests.Expression;


namespace sones.GraphDS.Services.RemoteAPIService.API_Services
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "GraphDSService")]
    public interface IGraphDS_API
    {
        [OperationContract]
        ServiceVertexType CreateVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexTypePredefinition myVertexTypePreDef);

        [OperationContract]
        ServiceVertexType AlterVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset);

        [OperationContract]
        ServiceEdgeType AlterEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset);

        [OperationContract(Name="GetVertexTypeByName")]
        ServiceVertexType GetVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken, String myVertexTypeName);

        [OperationContract(Name="GetVertexTypeByID")]
        ServiceVertexType GetVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken, Int64 myVertexTypeID);

        [OperationContract]
        Int64 BeginTransaction(SecurityToken mySecurityToken);

        [OperationContract]
        void CommitTransaction(SecurityToken mySecurityToken, Int64 myTransactionToken);

        [OperationContract]
        void RollbackTransaction(SecurityToken mySecurityToken, Int64 myTransactionToken);

        [OperationContract]
        void Shutdown(SecurityToken mySecurityToken);

        [OperationContract]
        List<Int64> Clear(SecurityToken mySecurityToken, Int64 myTransactionToken);

        [OperationContract]
        ServiceEdgeType CreateEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef);

        [OperationContract]
        List<ServiceEdgeType> CreateEdgeTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            IEnumerable<ServiceEdgeTypePredefinition> myEdgeTypePreDef);

        [OperationContract]
        ServiceIndexDefinition CreateIndex(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceVertexType> CreateVertexTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            List<ServiceVertexTypePredefinition> myVertexTypePreDef);

        [OperationContract]
        Tuple<IEnumerable<IComparable>, IEnumerable<IComparable>> Delete(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null);

        [OperationContract]
        ServiceIndexDefinition DescribeIndex(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myVertexTypeName, String myIndexName);

        [OperationContract]
        List<ServiceIndexDefinition> DescribeIndices(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myVertexTypeName);

        [OperationContract]
        List<ServiceIndexDefinition> DescribeIndicesByNames(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myVertexTypeName, List<String> myIndexNames);

        [OperationContract]
        Dictionary<Int64, String> DropEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeType myEdgeType);

        [OperationContract]
        void DropIndex(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition);

        [OperationContract]
        Dictionary<Int64, String> DropVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceEdgeType> GetAllEdgeTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdition);

        [OperationContract]
        List<ServiceVertexType> GetAllVertexTypes(SecurityToken mySecurityToken, Int64 myTransactionToken,
            String myEdition);

        [OperationContract(Name="GetEdgeTypeByName")]
        ServiceEdgeType GetEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken, String myEdgeTypeName, String myEdition = null);

        [OperationContract(Name="GetEdgeTypeByID")]
        ServiceEdgeType GetEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken, Int64 myEdgeTypeID, String myEdition = null);

        [OperationContract]
        ServiceVertexInstance GetVertex(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, Int64 myVertexID);

        [OperationContract]
        UInt64 GetVertexCount(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType);

        [OperationContract(Name = "GetVerticesByType")]
        List<ServiceVertexInstance> GetVertices(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType);

        [OperationContract(Name = "GetVerticesByExpression")]
        [ServiceKnownType(typeof(ServiceBinaryExpression))]
        [ServiceKnownType(typeof(ServiceBinaryOperator))]
        [ServiceKnownType(typeof(ServiceUnaryExpression))]
        [ServiceKnownType(typeof(ServiceUnaryLogicOperator))]
        [ServiceKnownType(typeof(ServicePropertyExpression))]
        [ServiceKnownType(typeof(ServiceSingleLiteralExpression))]
        [ServiceKnownType(typeof(ServiceRangeLiteralExpression))]
        [ServiceKnownType(typeof(ServiceCollectionLiteralExpression))]
        List<ServiceVertexInstance> GetVertices(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceBaseExpression myExpression);

        [OperationContract]
        ServiceVertexInstance Insert(SecurityToken mySecurityToken, Int64 myTransactionToken, String myVertexTypeName,
            ServiceInsertPayload myPayload);

        [OperationContract]
        void LogOff(SecurityToken mySecurityToken);

        [OperationContract]
        SecurityToken LogOn(ServiceUserPasswordCredentials myUserCredentials);

        [OperationContract]
        [ServiceKnownType(typeof(ServiceHyperEdgeView))]
        [ServiceKnownType(typeof(ServiceSingleEdgeView))]
        ServiceQueryResult Query(SecurityToken mySecToken, Int64 myTransactionToken, String myQueryString, String myLanguage);

        //[OperationContract] There is no plan to implement the Travers method yet, because there is no way to transport traverser logic from the client
        //List<ServiceVertexInstance>Traverse(SecurityToken mySecToken, Int64 myTransactionToken);

        [OperationContract]
        List<ServiceVertexInstance> Update(SecurityToken mySecurityToken, Int64 myTransactionToken, ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs,
            ServiceUpdateChangeset myUpdateChangeset);

        [OperationContract]
        void RebuildIndices(SecurityToken mySecurityToken, Int64 myTransactionToken, IEnumerable<String> myVertexTypeNames);

        [OperationContract]
        void TruncateVertexType(SecurityToken mySecurityToken, Int64 myTransactionToken, String myVertexTypeName);
    }
}
