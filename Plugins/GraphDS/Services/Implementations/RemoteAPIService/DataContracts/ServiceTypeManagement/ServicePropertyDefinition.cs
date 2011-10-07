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
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServicePropertyDefinition : ServiceAttributeDefinition
    {
        public ServicePropertyDefinition(IPropertyDefinition myPropertyDefinition)
            : base(myPropertyDefinition)
        {
            this.IsMandatory = myPropertyDefinition.IsMandatory;
            this.InIndices = myPropertyDefinition.InIndices.Select(x => x.Name).ToList();
            this.Multiplicity = ConvertHelper.ToServicePropertyMultiplicity(myPropertyDefinition.Multiplicity);
            this.IsUserDefinedType = myPropertyDefinition.IsUserDefinedType;
            this.BaseType = myPropertyDefinition.BaseType.AssemblyQualifiedName;
            this.DefaultValue = myPropertyDefinition.DefaultValue;
        }

        [DataMember]
        public Boolean IsMandatory;

        [DataMember]
        public Boolean IsUserDefinedType;

        [DataMember]
        public String BaseType;

        [DataMember]
        public ServicePropertyMultiplicity Multiplicity;

        [DataMember]
        public IComparable DefaultValue;

        [DataMember]
        public List<String> InIndices;
    }
}
