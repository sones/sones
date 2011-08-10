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
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    public static class ServiceVertexTypeConverter
    {
        

        public static IEnumerable<ServiceBinaryPropertyDefinition> ConvertAllBinaryPropertiesToService(IEnumerable<IBinaryPropertyDefinition> myBinaryProperties)
        {
            List<ServiceBinaryPropertyDefinition> binaryProperties = new List<ServiceBinaryPropertyDefinition>();
            foreach (var binaryProperty in myBinaryProperties)
            {
                binaryProperties.Add(new ServiceBinaryPropertyDefinition(binaryProperty));
            }
            return binaryProperties;
        }
        
        public static IEnumerable<ServiceOutgoingEdgeDefinition> ConvertAllOutgoingEdgesToService(IEnumerable<IOutgoingEdgeDefinition> myOutgiongEdges)
        {
            List<ServiceOutgoingEdgeDefinition> outgoingEdges = new List<ServiceOutgoingEdgeDefinition>();
            foreach (var outgoingEdge in myOutgiongEdges)
            {
                outgoingEdges.Add(new ServiceOutgoingEdgeDefinition(outgoingEdge));
            }
            return outgoingEdges;
        }
        
        public static IEnumerable<ServiceIncomingEdgeDefinition> ConvertAllIncomingEdgesToService(IEnumerable<IIncomingEdgeDefinition> myIncomingEdges)
        {
            List<ServiceIncomingEdgeDefinition> incomingEdges = new List<ServiceIncomingEdgeDefinition>();
            foreach (var incomingEdge in myIncomingEdges)
            {
                incomingEdges.Add(new ServiceIncomingEdgeDefinition(incomingEdge));
            }
            return incomingEdges;
        }
        
        public static IEnumerable<ServiceIndexDefinition> ConvertAllIndicesToService(IEnumerable<IIndexDefinition> myInIndices)
        {
            List<ServiceIndexDefinition> inIndices = new List<ServiceIndexDefinition>();
            foreach (var Index in myInIndices)
            {
                inIndices.Add(new ServiceIndexDefinition(Index));
            }
            return inIndices;
        }

        public static IEnumerable<ServicePropertyDefinition> ConvertAllPropertiesToService(IEnumerable<IPropertyDefinition> myPropertyList)
        {
            List<ServicePropertyDefinition> propertyList = new List<ServicePropertyDefinition>();
            foreach (var Property in myPropertyList)
            {
                propertyList.Add(new ServicePropertyDefinition(Property));
            }
            return propertyList;
        }
        
        
    }
}
