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


namespace sones.GraphDS.Services.RemoteAPIService.API_Services
{
    [ServiceContract(Namespace = "http://www.sones.com", Name = "IGraphDS")]
    public interface IGraphDS_API
    {
        //[OperationContract]
        //ServiceEdgeType AlterEdgeType(ServiceEdgeTypePredefinition myEdgeTypePreDef);
        
        [OperationContract]
        ServiceVertexType AlterVertexType(ServiceVertexTypePredefinition myVertexTypePreDef);
        
        [OperationContract]
        void Clear();

        //[OperationContract]
        //ServiceEdgeType CreateEdgeType(ServiceEdgeTypePredefinition myVertexTypePreDef);

        [OperationContract]
        ServiceIndexDefinition CreateIndex(ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        ServiceVertexType CreateVertexType(ServiceVertexTypePredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceVertexType> CreateVertexTypes(List<ServiceVertexTypePredefinition> myVertexTypePreDef);
                
        [OperationContract]
        void Delete(ServiceVertexTypePredefinition myVertexTypePreDef);

        [OperationContract]
        ServiceIndexDefinition DescribeIndex(ServiceIndexPredefinition myVertexTypePreDef);

        //[OperationContract]
        //ServiceIndexDefinition DescribeIndices(ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        void DropIndex(ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        ServiceIndexDefinition DropType(ServiceIndexPredefinition myVertexTypePreDef);

        [OperationContract]
        List<ServiceEdgeType> GetAllEdgeTypes();

        [OperationContract]
        List<ServiceVertexType> GetAllVertexTypes();

        [OperationContract]
        List<ServiceEdgeType> GetEdgeType();

        [OperationContract]
        List<ServiceEdgeType> GetVertex();

        [OperationContract]
        List<ServiceEdgeType> GetVertexCount();

        [OperationContract]
        List<ServiceEdgeType> GetVertices();

        [OperationContract]
        ServiceVertexType Insert();
      
                   
                                                 
    }
}
