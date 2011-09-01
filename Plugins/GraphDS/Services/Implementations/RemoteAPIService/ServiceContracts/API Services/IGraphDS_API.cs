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
    [ServiceContract(Namespace = "http://www.sones.com", Name = "IGraphDS")]
    public interface IGraphDS_API
    {
        [OperationContract]
        ServiceVertexType CreateVertexType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexTypePredefinition myVertexTypePreDef);
        
        [OperationContract]
        ServiceVertexType AlterVertexType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset);
        
        [OperationContract]
        ServiceEdgeType AlterEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset);  
               

        [OperationContract]
        ServiceTransactionToken BeginTransaction(SecurityToken mySecurityToken);
        
        [OperationContract]
        List<Int64> Clear(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken);

        [OperationContract]
        ServiceEdgeType CreateEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceEdgeTypePredefinition myEdgeTypePreDef);

        [OperationContract]
        List<ServiceEdgeType> CreateEdgeTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            IEnumerable<ServiceEdgeTypePredefinition> myEdgeTypePreDef);

        [OperationContract]
        ServiceIndexDefinition CreateIndex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceVertexType> CreateVertexTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, 
            List<ServiceVertexTypePredefinition> myVertexTypePreDef);
                
        [OperationContract]
        List<Int64> Delete(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null); 

        [OperationContract]
        ServiceIndexDefinition DescribeIndex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName);

        [OperationContract]
        List<ServiceIndexDefinition> DescribeIndices(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        Dictionary<Int64, String> DropEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, 
            ServiceEdgeType myEdgeType);
                
        [OperationContract]
        void DropIndex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, String myIndexName, String myEdition);

        [OperationContract]
        Dictionary<Int64, String> DropVertexType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceEdgeType> GetAllEdgeTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            String myEdition);

        [OperationContract]
        List<ServiceVertexType> GetAllVertexTypes(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            String myEdition);

        [OperationContract]
        ServiceEdgeType GetEdgeType(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            Int64 myEdgeTypeID, String myEdition = null);

        [OperationContract]
        ServiceVertexInstance GetVertex(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken,
            ServiceVertexType myVertexType, Int64 myVertexID);

        [OperationContract]
        UInt64 GetVertexCount(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexType myVertexType);

        [OperationContract]
        List<ServiceVertexInstance> GetVertices(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, ServiceVertexType myVertexType);

        [OperationContract]
        ServiceVertexInstance Insert(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, String myVertexTypeName,
            ServiceInsertPayload myPayload);

        [OperationContract]
        void LogOff(SecurityToken mySecurityToken);

        [OperationContract]
        SecurityToken LogOn(String myLogin, String myPassword);

        [OperationContract]
        ServiceQueryResult Query(SecurityToken mySecurityToken, ServiceTransactionToken myTransactionToken, String myQueryString, String myLanguage); 

        //Traverse Vertex

        //Update 
      
                   
                                                 
    }
}
