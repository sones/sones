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
using System.ServiceModel.Description;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    public partial class RPCServiceContract : AbstractBaseTypeService, IVertexTypeService
    {
        
        public bool HasBinaryProperty(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public ServiceBinaryPropertyDefinition GetBinaryPropertyDefinition(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasBinaryProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceBinaryPropertyDefinition> GetBinaryProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasIncomingEdge(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public ServiceIncomingEdgeDefinition GetIncomingEdgeDefinition(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasIncomingEdges(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceIncomingEdgeDefinition> GetIncomingEdgeDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdge(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public ServiceOutgoingEdgeDefinition GetOutgoingEdgeDefinition(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdges(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasIndexDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceIndexDefinition> GetIndexDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }
    }
}
