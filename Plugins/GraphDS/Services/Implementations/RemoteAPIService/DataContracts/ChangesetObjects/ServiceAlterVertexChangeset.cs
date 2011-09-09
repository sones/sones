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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceAlterVertexChangeset
    {
        [DataMember]
        public String NewTypeName;
        
        [DataMember]
        public String Comment;
        
        [DataMember]
        public List<ServicePropertyPredefinition> ToBeAddedProperties;
        
        [DataMember]
        public List<ServiceIncomingEdgePredefinition> ToBeAddedIncomingEdges;

        [DataMember]
        public List<ServiceOutgoingEdgePredefinition> ToBeAddedOutgoingEdges;

        [DataMember]
        public List<ServiceIndexPredefinition> ToBeAddedIndices;

        [DataMember]
        public List<ServiceUniquePredefinition> ToBeAddedUniques;

        [DataMember]
        public List<ServiceMandatoryPredefinition> ToBeAddedMandatories;

        [DataMember]
        public List<ServicePropertyPredefinition> ToBeRemovedProperties;

        [DataMember]
        public List<ServiceIncomingEdgePredefinition> ToBeRemovedIncomingEdges;

        [DataMember]
        public List<ServiceOutgoingEdgePredefinition> ToBeRemovedOutgoingEdges;

        [DataMember]
        public List<ServiceIndexPredefinition> ToBeRemovedIndices;

        [DataMember]
        public List<ServiceUniquePredefinition> ToBeRemovedUniques;

        [DataMember]
        public List<ServiceMandatoryPredefinition> ToBeRemovedMandatories;

        [DataMember]
        public Dictionary<String, String> ToBeRenamedProperties;
    }
}
