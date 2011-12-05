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


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceIndexDefinition
    {
        public ServiceIndexDefinition(IIndexDefinition myIndexDefinition)
        {
            this.Name = myIndexDefinition.Name;
            this.ID = myIndexDefinition.ID;
            this.IndexTypeName = myIndexDefinition.IndexTypeName;
            this.Edition = myIndexDefinition.Edition;
            this.IsUserdefined = myIndexDefinition.IsUserdefined;
            this.IndexedProperties = myIndexDefinition.IndexedProperties.Select(x => x.Name).ToList();
            this.VertexType = new ServiceVertexType(myIndexDefinition.VertexType);
            this.SourceIndex = (myIndexDefinition.SourceIndex == null) ? null : new ServiceIndexDefinition(myIndexDefinition.SourceIndex);
            this.IsRange = myIndexDefinition.IsRange;
            this.IsVersioned = myIndexDefinition.IsVersioned;            
        }

        [DataMember]
        public String Name;
        [DataMember]
        public Int64 ID;
        [DataMember]
        public String IndexTypeName;
        [DataMember]
        public String Edition;
        [DataMember]
        public Boolean IsUserdefined;
        [DataMember]
        public List<String> IndexedProperties;

        [DataMember]
        public ServiceVertexType VertexType;

        [DataMember]
        public ServiceIndexDefinition SourceIndex;

        [DataMember]
        public Boolean IsRange;
        [DataMember]
        public Boolean IsVersioned;
    }
}
