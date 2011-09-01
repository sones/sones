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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceOutgoingEdgePredefinition : ServiceAttributePredefinition
    {
        #region Constant

        /// <summary>
        /// The name of the predefined edge type that represents a normal edge.
        /// </summary>
        [DataMember]
        public const string Edge = "Edge";

        /// <summary>
        /// The name of the predefined edge type that represents a edges with an attribute Weight of type double.
        /// </summary>
        [DataMember]
        public const string WeightedEdge = "Weighted";

        /// <summary>
        /// The name of the predefined edge type that represents a edges with an attribute Order.
        /// </summary>
        [DataMember]
        public const string OrderedEdge = "Ordered";

        #endregion
        
        
        
        /// <summary>
        /// The edge type of this edge definition
        /// </summary>
        [DataMember]
        public String EdgeType;


        /// <summary>
        /// The multiplicity of the edge.
        /// </summary>
        [DataMember]
        public ServiceEdgeMultiplicity Multiplicity;

        /// <summary>
        /// The inner edge type of a multi edge.
        /// </summary>
        [DataMember]
        public string InnerEdgeType;

        public OutgoingEdgePredefinition ToOutgoingEdgePredefinition()
        {
            OutgoingEdgePredefinition OutgoingEdgePreDef = new OutgoingEdgePredefinition(this.AttributeName,"");

            OutgoingEdgePreDef.SetAttributeType(this.AttributeType);
            OutgoingEdgePreDef.SetComment(this.Comment);
            OutgoingEdgePreDef.InnerEdgeType = this.InnerEdgeType;

            if (this.Multiplicity == ServiceEdgeMultiplicity.HyperEdge)
                OutgoingEdgePreDef.SetMultiplicityAsHyperEdge();
            if (this.Multiplicity == ServiceEdgeMultiplicity.MultiEdge)
                OutgoingEdgePreDef.SetMultiplicityAsMultiEdge();

            return OutgoingEdgePreDef;
        }
    }
}
