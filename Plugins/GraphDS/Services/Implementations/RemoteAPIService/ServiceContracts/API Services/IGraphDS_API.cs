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


namespace sones.GraphDS.Services.RemoteAPIService.API_Services
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "GraphDS")]
    public interface IGraphDS_API
    {
        [OperationContract]
        ServiceVertexType CreateVertexType(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexTypePredefinition myVertexTypePreDef);
        
        [OperationContract]
        ServiceVertexType AlterVertexType(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset);
        
        [OperationContract]
        ServiceEdgeType AlterEdgeType(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset);

        [OperationContract]
        ServiceVertexType GetVertexType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, String myVertexTypeName);
               

        [OperationContract]
        ServiceTransactionToken BeginTransaction(SecurityToken mySecToken);

        [OperationContract]
        void CommitTransaction(SecurityToken mySecToken, ServiceTransactionToken myTransToken);

        [OperationContract]
        void RollbackTransaction(SecurityToken mySecToken, ServiceTransactionToken myTransToken);

        [OperationContract]
        void Shutdown(SecurityToken mySecurityToken);
        
        [OperationContract]
        List<Int64> Clear(SecurityToken mySecToken, ServiceTransactionToken myTransToken);

        [OperationContract]
        ServiceEdgeType CreateEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef);

        [OperationContract]
        ServiceIndexDefinition CreateIndex(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceVertexType> CreateVertexTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken, 
            List<ServiceVertexTypePredefinition> myVertexTypePreDef);
                
        [OperationContract]
        List<Int64> Delete(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null); 

        [OperationContract]
        ServiceIndexDefinition DescribeIndex(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexType myVertexType, String myIndexName);

        [OperationContract]
        List<ServiceIndexDefinition> DescribeIndices(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        Dictionary<Int64, String> DropEdgeType(SecurityToken mySecToken, ServiceTransactionToken myTransToken, 
            ServiceEdgeType myEdgeType);
                
        [OperationContract]
        void DropIndex(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition);

        [OperationContract]
        Dictionary<Int64, String> DropVertexType(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceEdgeType> GetAllEdgeTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            String myEdition);

        [OperationContract]
        List<ServiceVertexType> GetAllVertexTypes(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            String myEdition);

        [OperationContract]
        ServiceEdgeType GetEdgeType(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            String myEdgeTypeName, String myEdition = null);

        [OperationContract]
        ServiceVertexInstance GetVertex(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexType myVertexType, Int64 myVertexID);

        [OperationContract]
        UInt64 GetVertexCount(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceVertexInstance> GetVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myVertexType);

        [OperationContract]
        ServiceVertexInstance Insert(SecurityToken mySecToken, ServiceTransactionToken myTransToken, String myVertexTypeName,
            ServiceInsertPayload myPayload);

        [OperationContract]
        void LogOff(SecurityToken mySecToken);

        [OperationContract]
        SecurityToken LogOn(String myLogin, String myPassword);

        //[OperationContract] There is no plan to implement the Query method yet, because client libs are already available
        //ServiceQueryResult Query(SecurityToken mySecToken, ServiceTransactionToken myTransToken, String myQueryString, String myLanguage); 
        
        //[OperationContract] There is no plan to implement the Travers method yet, because there is no way to transport traverser logic from the client
        //List<ServiceVertexInstance>Traverse(SecurityToken mySecToken, ServiceTransactionToken myTransToken);

        [OperationContract]
        List<ServiceVertexInstance> Update(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs,
            ServiceUpdateChangeset myUpdateChangeset);
                   
                                                 
    }
}
