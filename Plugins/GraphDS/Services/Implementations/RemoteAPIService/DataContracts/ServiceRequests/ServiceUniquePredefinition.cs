using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.Request;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceUniquePredefinition
    {
       
        /// <summary>
        /// The set of properties that will be unique together.
        /// </summary>
        [DataMember]
        public IEnumerable<String> Properties;

        public UniquePredefinition ToUniquePredefinition()
        {
            var uniquepredefinition = new UniquePredefinition();
            foreach (var Property in this.Properties)
            {
                uniquepredefinition.AddPropery(Property);
            }
            return uniquepredefinition;
        
        }
       
    }
}
