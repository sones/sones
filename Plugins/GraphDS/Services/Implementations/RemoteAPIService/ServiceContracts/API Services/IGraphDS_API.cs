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
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "GraphDS")]
    public interface IGraphDS_API
    {
        [OperationContract]
        ServiceVertexType CreateVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexTypePredefinition myVertexTypePreDef);
        
        [OperationContract]
        ServiceVertexType AlterVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset);
        
        [OperationContract]
        ServiceEdgeType AlterEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset);

        [OperationContract]
        ServiceVertexType GetVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransToken, String myVertexTypeName);
               

        [OperationContract]
        Int64 BeginTransaction(ServiceSecurityToken mySecurityToken);

        [OperationContract]
        void CommitTransaction(ServiceSecurityToken mySecurityToken, Int64 myTransToken);

        [OperationContract]
        void RollbackTransaction(ServiceSecurityToken mySecurityToken, Int64 myTransToken);

        [OperationContract]
        void Shutdown(ServiceSecurityToken mySecurityToken);
        
        [OperationContract]
        List<Int64> Clear(ServiceSecurityToken mySecurityToken, Int64 myTransToken);

        [OperationContract]
        ServiceEdgeType CreateEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef);

        [OperationContract]
        ServiceIndexDefinition CreateIndex(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceVertexType> CreateVertexTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken, 
            List<ServiceVertexTypePredefinition> myVertexTypePreDef);
                
        [OperationContract]
        List<Int64> Delete(ServiceSecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null); 

        [OperationContract]
        ServiceIndexDefinition DescribeIndex(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexType myVertexType, String myIndexName);

        [OperationContract]
        List<ServiceIndexDefinition> DescribeIndices(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        Dictionary<Int64, String> DropEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransToken, 
            ServiceEdgeType myEdgeType);
                
        [OperationContract]
        void DropIndex(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition);

        [OperationContract]
        Dictionary<Int64, String> DropVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceEdgeType> GetAllEdgeTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            String myEdition);

        [OperationContract]
        List<ServiceVertexType> GetAllVertexTypes(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            String myEdition);

        [OperationContract]
        ServiceEdgeType GetEdgeType(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            String myEdgeTypeName, String myEdition = null);

        [OperationContract]
        ServiceVertexInstance GetVertex(ServiceSecurityToken mySecurityToken, Int64 myTransToken,
            ServiceVertexType myVertexType, Int64 myVertexID);

        [OperationContract]
        UInt64 GetVertexCount(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myVertexType);

        [OperationContract(Name = "GetVerticesByType")]
        List<ServiceVertexInstance> GetVertices(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myVertexType);

        [OperationContract(Name = "GetVerticesByExpression")]
        [ServiceKnownType(typeof(ServiceBinaryExpression))]
        [ServiceKnownType(typeof(ServiceBinaryOperator))]
        [ServiceKnownType(typeof(ServiceUnaryExpression))]
        [ServiceKnownType(typeof(ServiceUnaryLogicOperator))]
        [ServiceKnownType(typeof(ServicePropertyExpression))]
        [ServiceKnownType(typeof(ServiceSingleLiteralExpression))]
        [ServiceKnownType(typeof(ServiceRangeLiteralExpression))]
        [ServiceKnownType(typeof(ServiceCollectionLiteralExpression))]
        List<ServiceVertexInstance> GetVertices(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceBaseExpression myVertexType);


        [OperationContract]
        ServiceVertexInstance Insert(ServiceSecurityToken mySecurityToken, Int64 myTransToken, String myVertexTypeName,
            ServiceInsertPayload myPayload);

        [OperationContract]
        void LogOff(ServiceSecurityToken mySecurityToken);

        [OperationContract]
        ServiceSecurityToken LogOn(String myLogin, String myPassword);

        [OperationContract]
        [ServiceKnownType(typeof(ServiceHyperEdgeView))]
        [ServiceKnownType(typeof(ServiceSingleEdgeView))]
        ServiceQueryResult Query(ServiceSecurityToken mySecToken, Int64 myTransToken, String myQueryString, String myLanguage); 
        
        //[OperationContract] There is no plan to implement the Travers method yet, because there is no way to transport traverser logic from the client
        //List<ServiceVertexInstance>Traverse(ServiceSecurityToken mySecToken, Int64 myTransToken);

        [OperationContract]
        List<ServiceVertexInstance> Update(ServiceSecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs,
            ServiceUpdateChangeset myUpdateChangeset);
                   
        [OperationContract]
        void RebuildIndices(ServiceSecurityToken mySecurityToken, Int64 myTransToken, IEnumerable<String> myVertexTypeNames);

        [OperationContract]
        void TruncateVertexType(ServiceSecurityToken mySecurityToken, Int64 myTransToken, String myVertexTypeNames);
    }
}
