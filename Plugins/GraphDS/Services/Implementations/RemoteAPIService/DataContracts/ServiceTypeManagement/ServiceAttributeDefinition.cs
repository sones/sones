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
    [KnownType(typeof(ServicePropertyDefinition))]
    [KnownType(typeof(ServiceIncomingEdgeDefinition))]
    [KnownType(typeof(ServiceOutgoingEdgeDefinition))]
    public abstract class ServiceAttributeDefinition
    {
        public ServiceAttributeDefinition(IAttributeDefinition myAttributeDefinition)
        {
            if (myAttributeDefinition != null)
            {
                this.ID = myAttributeDefinition.ID;
                this.Name = myAttributeDefinition.Name;
                this.IsUserDefined = myAttributeDefinition.IsUserDefined;
                this.Kind = (ServiceAttributeType)myAttributeDefinition.Kind;
                this.RelatedTypeName = myAttributeDefinition.RelatedType.Name;
            }
        }

        [DataMember]
        public Int64 ID;

        [DataMember]
        public String Name;

        [DataMember]
        public Boolean IsUserDefined;

        [DataMember]
        public ServiceAttributeType Kind;

        [DataMember]
        public String RelatedTypeName;
    }
}
