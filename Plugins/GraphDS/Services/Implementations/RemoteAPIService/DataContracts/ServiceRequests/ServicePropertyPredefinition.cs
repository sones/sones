using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
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
            var property =  new PropertyPredefinition(this.AttributeName);
            property.SetAttributeType(this.AttributeType != null ? this.AttributeType : null);
            property.SetComment(this.Comment != null ? this.Comment : null);
                        
            if (this.IsIndexed)
                property.SetAsIndexed();
            if (this.IsMandatory)
                property.SetAsMandatory();
            if (this.IsUnique)
                property.SetAsUnique();

            if (this.Multiplicity.Equals(ServicePropertyMultiplicity.List))
                property.SetMultiplicityToList();
            if (this.Multiplicity.Equals(ServicePropertyMultiplicity.Set))
                property.SetMultiplicityToSet();
            
            return property;
        }

    }
}
