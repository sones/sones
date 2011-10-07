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
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceUpdateChangeset
    {
        [DataMember]
        public String Comment;

        [DataMember]
        public String Edition;

        [DataMember]
        public Dictionary<string, IEnumerable<IComparable>> AddedElementsToCollectionProperties;

        [DataMember]
        public Dictionary<string, IEnumerable<IComparable>> RemovedElementsFromCollectionProperties;

        [DataMember]
        public Dictionary<string, ServiceEdgePredefinition> AddedElementsToCollectionEdges;

        [DataMember]
        public Dictionary<string, ServiceEdgePredefinition> RemovedElementsFromCollectionEdges;

        [DataMember]
        public Dictionary<String, IComparable> UpdatedUnstructuredProperties;

        [DataMember]
        public Dictionary<String, IComparable> UpdatedStructuredProperties;

        //[DataMember]
        //public IDictionary<String, Stream> UpdatedBinaryProperties;

        [DataMember]
        public List<ServiceEdgePredefinition> UpdatedOutgoingEdges;

        [DataMember]
        public List<ServiceSingleEdgeUpdateDefinition> UpdateOutgoingEdgesProperties;

        [DataMember]
        public Dictionary<string, object> UpdatedUnknownProperties;

        [DataMember]
        public List<string> RemovedAttributes;
    }
}
