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
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceVertexTypePredefinition
    {
        /// <summary>
        /// The name of the vertex type that is going to be created
        /// </summary>
        [DataMember(IsRequired = true)]
        public string VertexTypeName;
        
        [DataMember]
        private IEnumerable<ServiceUniquePredefinition> Uniques;

        [DataMember]
        private IEnumerable<ServiceIndexPredefinition> Indices;

        /// <summary>
        /// The name of the vertex type this vertex types inherites from.
        /// </summary>
        [DataMember]
        public string SuperVertexTypeName;

        /// <summary>
        /// The properties of the vertex type.
        /// </summary>
        [DataMember]
        public IEnumerable<ServicePropertyPredefinition> Properties;

        
        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        [DataMember]
        public IEnumerable<ServiceOutgoingEdgePredefinition> OutgoingEdges;
        

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        [DataMember]
        public IEnumerable<ServiceIncomingEdgePredefinition> IncomingEdges;
      
        /// <summary>
        /// Gets if the vertex type will be sealed.
        /// </summary>
        [DataMember]
        public bool IsSealed;

        /// <summary>
        /// Gets if the vertex type will be abstract.
        /// </summary>
        [DataMember]
        public bool IsAbstract;
        
     

        /// <summary>
        /// Gets the comment for this vertex type.
        /// </summary>
        [DataMember]
        public string Comment;


        public VertexTypePredefinition ToVertexTypePredefinition()
        {
            VertexTypePredefinition VertexTypePreDef = new VertexTypePredefinition(this.VertexTypeName);
            
            if (this.IsAbstract)
                VertexTypePreDef.MarkAsAbstract();
            if (this.IsSealed)
                VertexTypePreDef.MarkAsSealed();

            VertexTypePreDef.SetComment(this.Comment);
            VertexTypePreDef.SetSuperTypeName(this.SuperVertexTypeName);

            if (this.Properties != null)
            {
                foreach (var Property in this.Properties)
                {
                    VertexTypePreDef.AddProperty(Property.ToPropertyPredefinition());
                }
            }

            if (this.Uniques != null)
            {
                foreach (var Unique in this.Uniques)
                {
                    VertexTypePreDef.AddUnique(Unique.ToUniquePredefinition());
                }
            }

            if (this.IncomingEdges != null)
            {
                foreach (var IncomingEdge in this.IncomingEdges)
                {
                    VertexTypePreDef.AddIncomingEdge(IncomingEdge.ToIncomingEdgePredefinition());
                }
            }

            if (this.OutgoingEdges != null)
            {
                foreach (var OutgoingEdge in this.OutgoingEdges)
                {
                    VertexTypePreDef.AddOutgoingEdge(OutgoingEdge.ToOutgoingEdgePredefinition());
                }
            }

            if (this.Indices != null)
            {
                foreach (var Index in this.Indices)
                {
                    VertexTypePreDef.AddIndex(Index.ToIndexPredefinition());
                }
            }
            return VertexTypePreDef;
        }


    }
}
