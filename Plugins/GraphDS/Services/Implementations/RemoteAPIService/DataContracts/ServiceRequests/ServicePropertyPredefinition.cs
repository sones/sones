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
    public class ServicePropertyPredefinition : ServiceAttributePredefinition
    {
       
        /// <summary>
        /// Should there be an index on the property?
        /// </summary>
        [DataMember]
        public Boolean IsIndexed;

        /// <summary>
        /// Should this property be mandatory?
        /// </summary>
        [DataMember]
        public Boolean IsMandatory;

        /// <summary>
        /// Should this property be unique?
        /// </summary>
        [DataMember]
        public Boolean IsUnique;

        /// <summary>
        /// The default value for this property.
        /// </summary>
        [DataMember]
        public String DefaultValue;

        /// <summary>
        /// The multiplicity of this property.
        /// </summary>
        [DataMember]
        public ServicePropertyMultiplicity Multiplicity;

        
        public PropertyPredefinition ToPropertyPredefinition()
        {
            var property =  new PropertyPredefinition(this.AttributeName,this.AttributeType); //Todo insert attribute type
            property.SetAttributeType(this.AttributeType != null ? this.AttributeType : null);
            property.SetComment(this.Comment != null ? this.Comment : null);
                        
            if (this.IsIndexed)
                property.SetAsIndexed();
            if (this.IsMandatory)
                property.SetAsMandatory();
            if (this.IsUnique)
                property.SetAsUnique();
            if (this.DefaultValue != null)
                property.SetDefaultValue(this.DefaultValue);

            if (this.Multiplicity.Equals(ServicePropertyMultiplicity.List))
                property.SetMultiplicityToList();
            if (this.Multiplicity.Equals(ServicePropertyMultiplicity.Set))
                property.SetMultiplicityToSet();
            
            return property;
        }

    }
}
