using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;
using sones.GraphDB.TypeSystem;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceIndexDefinition
    {
        public ServiceIndexDefinition(IIndexDefinition myIndexDefinition)
        {
            this.Edition = myIndexDefinition.Edition;
            this.ID = myIndexDefinition.ID;
            this.IndexTypeName = myIndexDefinition.IndexTypeName;
            this.IsRange = myIndexDefinition.IsRange;
            this.IsSingle = myIndexDefinition.IsSingle;
            this.IsUserdefined = myIndexDefinition.IsUserdefined;
            this.IsVersioned = myIndexDefinition.IsVersioned;
            this.Name = myIndexDefinition.Name;
            //this.IndexedProperties = ServiceVertexTypeConverter.ConvertAllPropertiesToService(myIndexDefinition.IndexedProperties);
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
        public IEnumerable<ServicePropertyDefinition> IndexedProperties;

        [DataMember]
        public ServiceVertexType VertexType;

        [DataMember]
        public ServiceIndexDefinition SourceIndex;

        [DataMember]
        public Boolean IsSingle;
        [DataMember]
        public Boolean IsRange;
        [DataMember]
        public Boolean IsVersioned;
    }
}
