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
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceEdgePredefinition
    {
        [DataMember]
        public String EdgeName;

        [DataMember]
        public List<ServiceEdgePredefinition> ContainedEdges;

        [DataMember]
        public String Comment;

        [DataMember]
        public List<Tuple<Int64, List<Int64>>> VertexIDsByID;

        [DataMember]
        public List<StructuredProperty> StructuredProperties;

        [DataMember]
        public List<UnstructuredProperty> UnstructuredProperties;

        public EdgePredefinition ToEdgePredefinition()
        {
            EdgePredefinition EdgePredef = new EdgePredefinition(this.EdgeName);
            if (!String.IsNullOrEmpty(Comment))
                EdgePredef.Comment = this.Comment;

            if (ContainedEdges != null)
            {
                foreach (var Edge in this.ContainedEdges)
                {
                    EdgePredef.AddEdge(Edge.ToEdgePredefinition());
                }
            }

            if (StructuredProperties != null)
            {
                foreach (var Property in this.StructuredProperties)
                {
                    EdgePredef.AddStructuredProperty(Property.PropertyName, Property.PropertyValue as IComparable);
                }
            }

            if (UnstructuredProperties != null)
            {
                foreach (var Property in this.UnstructuredProperties)
                {
                    EdgePredef.AddUnstructuredProperty(Property.PropertyName, Property.PropertyValue);
                }
            }

            if (VertexIDsByID != null)
            {
                foreach (var VertexTypeSet in this.VertexIDsByID)
                {
                    foreach (var Vertex in VertexTypeSet.Item2)
                    {
                        EdgePredef.AddVertexID(VertexTypeSet.Item1, Vertex);
                    }
                }
            }
            
            return EdgePredef;
        }
    }
}
