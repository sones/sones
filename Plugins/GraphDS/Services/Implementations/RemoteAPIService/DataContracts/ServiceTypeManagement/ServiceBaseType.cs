using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceBaseType
    {
        public ServiceBaseType(IBaseType myBaseType)
        {
            this.ID = myBaseType.ID;
            this.Comment = myBaseType.Comment;
            this.Name = myBaseType.Name;
            this.IsAbstract = myBaseType.IsAbstract;
            this.IsUserDefined = myBaseType.IsUserDefined;
            this.IsSealed = myBaseType.IsSealed;
            this.HasChildTypes = myBaseType.HasChildTypes;
            this.HasParentType = myBaseType.HasParentType;
        }

        [DataMember]
        public long ID;
        [DataMember]
        public String Name;
        [DataMember]
        public String Comment;
        [DataMember]
        public Boolean IsAbstract;
        [DataMember]
        public Boolean IsUserDefined;

        [DataMember]
        public Boolean IsSealed;
        [DataMember]
        public Boolean HasChildTypes;
        [DataMember]
        public Boolean HasParentType;

    }
}
