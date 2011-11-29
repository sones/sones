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

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceUnknownAttributePredefinition : ServiceAttributePredefinition
    {
        [DataMember]
        public String Multiplicity;

        [DataMember]
        public String DefaultValue;

        [DataMember]
        public String EdgeType;

        [DataMember]
        public String InnerEdgeType;

        [DataMember]
        public bool IsMandatory;

        [DataMember]
        public bool IsUnique;

        public UnknownAttributePredefinition ToUnknownAttributePredefinition()
        {
            var attributePredef = new UnknownAttributePredefinition(this.AttributeName, this.AttributeType);
            attributePredef.SetMultiplicity(this.Multiplicity);
            attributePredef.SetDefaultValue(this.DefaultValue);
            attributePredef.SetEdgeType(this.EdgeType);
            attributePredef.SetInnerEdgeType(this.InnerEdgeType);
            if (this.IsMandatory == true)
            {
                attributePredef.SetAsMandatory();
            }
            if (this.IsUnique == true)
            {
                attributePredef.SetAsUnique();
            }
            return attributePredef;
        }
    }
}
