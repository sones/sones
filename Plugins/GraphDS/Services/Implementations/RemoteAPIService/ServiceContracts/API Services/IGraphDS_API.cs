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
        ServiceVertexType CreateVertexType(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexTypePredefinition myVertexTypePreDef);
        
        [OperationContract]
        ServiceVertexType AlterVertexType(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset);
        
        [OperationContract]
        ServiceEdgeType AlterEdgeType(SecurityToken mySecToken, Int64 myTransToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset);

        [OperationContract]
        ServiceVertexType GetVertexType(SecurityToken mySecToken, Int64 myTransToken, String myVertexTypeName);
               

        [OperationContract]
        Int64 BeginTransaction(SecurityToken mySecToken);

        [OperationContract]
        void CommitTransaction(SecurityToken mySecToken, Int64 myTransToken);

        [OperationContract]
        void RollbackTransaction(SecurityToken mySecToken, Int64 myTransToken);

        [OperationContract]
        void Shutdown(SecurityToken mySecurityToken);
        
        [OperationContract]
        List<Int64> Clear(SecurityToken mySecToken, Int64 myTransToken);

        [OperationContract]
        ServiceEdgeType CreateEdgeType(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef);

        [OperationContract]
        ServiceIndexDefinition CreateIndex(SecurityToken mySecToken, Int64 myTransToken, ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceVertexType> CreateVertexTypes(SecurityToken mySecToken, Int64 myTransToken, 
            List<ServiceVertexTypePredefinition> myVertexTypePreDef);
                
        [OperationContract]
        List<Int64> Delete(SecurityToken mySecurityToken, Int64 myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null); 

        [OperationContract]
        ServiceIndexDefinition DescribeIndex(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexType myVertexType, String myIndexName);

        [OperationContract]
        List<ServiceIndexDefinition> DescribeIndices(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        Dictionary<Int64, String> DropEdgeType(SecurityToken mySecToken, Int64 myTransToken, 
            ServiceEdgeType myEdgeType);
                
        [OperationContract]
        void DropIndex(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition);

        [OperationContract]
        Dictionary<Int64, String> DropVertexType(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceEdgeType> GetAllEdgeTypes(SecurityToken mySecToken, Int64 myTransToken,
            String myEdition);

        [OperationContract]
        List<ServiceVertexType> GetAllVertexTypes(SecurityToken mySecToken, Int64 myTransToken,
            String myEdition);

        [OperationContract]
        ServiceEdgeType GetEdgeType(SecurityToken mySecToken, Int64 myTransToken,
            String myEdgeTypeName, String myEdition = null);

        [OperationContract]
        ServiceVertexInstance GetVertex(SecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexType myVertexType, Int64 myVertexID);

        [OperationContract]
        UInt64 GetVertexCount(SecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myVertexType);

        [OperationContract(Name = "GetVerticesByType")]
        List<ServiceVertexInstance> GetVertices(SecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myVertexType);

        [OperationContract(Name = "GetVerticesByExpression")]
        [ServiceKnownType(typeof(ServiceBinaryExpression))]
        [ServiceKnownType(typeof(ServiceBinaryOperator))]
        [ServiceKnownType(typeof(ServiceUnaryExpression))]
        [ServiceKnownType(typeof(ServiceUnaryLogicOperator))]
        [ServiceKnownType(typeof(ServicePropertyExpression))]
        [ServiceKnownType(typeof(ServiceSingleLiteralExpression))]
        [ServiceKnownType(typeof(ServiceRangeLiteralExpression))]
        [ServiceKnownType(typeof(ServiceCollectionLiteralExpression))]
        List<ServiceVertexInstance> GetVertices(SecurityToken mySecToken, Int64 myTransToken, ServiceBaseExpression myVertexType);


        [OperationContract]
        ServiceVertexInstance Insert(SecurityToken mySecToken, Int64 myTransToken, String myVertexTypeName,
            ServiceInsertPayload myPayload);

        [OperationContract]
        void LogOff(SecurityToken mySecToken);

        [OperationContract]
        SecurityToken LogOn(String myLogin, String myPassword);

        //[OperationContract] There is no plan to implement the Query method yet, because client libs are already available
        //ServiceQueryResult Query(SecurityToken mySecToken, Int64 myTransToken, String myQueryString, String myLanguage); 
        
        //[OperationContract] There is no plan to implement the Travers method yet, because there is no way to transport traverser logic from the client
        //List<ServiceVertexInstance>Traverse(SecurityToken mySecToken, Int64 myTransToken);

        [OperationContract]
        List<ServiceVertexInstance> Update(SecurityToken mySecToken, Int64 myTransToken, ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs,
            ServiceUpdateChangeset myUpdateChangeset);
                   
                                                 
    }
}
