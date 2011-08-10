using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServicePropertyDefinition : ServiceAttributeDefinition
    {
        public ServicePropertyDefinition(IPropertyDefinition myPropertyDefinition)
            : base(myPropertyDefinition)
        {
            this.IsMandatory = myPropertyDefinition.IsMandatory;
            //this.InIndices = ServiceVertexTypeConverter.ConvertAllIndicesToService(myPropertyDefinition.InIndices);
            this.Multiplicity = (ServicePropertyMultiplicity)myPropertyDefinition.Multiplicity;
            this.IsUserDefinedType = myPropertyDefinition.IsUserDefinedType;
            this.BaseType = myPropertyDefinition.BaseType.AssemblyQualifiedName;
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
        public IEnumerable<ServiceIndexDefinition> InIndices;
    }
}
