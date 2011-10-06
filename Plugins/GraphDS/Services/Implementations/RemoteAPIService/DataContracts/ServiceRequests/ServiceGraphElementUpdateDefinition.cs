using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [KnownType(typeof(ServiceSingleEdgeUpdateDefinition))]
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public abstract class ServiceGraphElementUpdateDefinition
    {
        [DataMember]
        public string CommentUpdate;

        /// <summary>
        /// Maps server side property UpdatedStructuredProperties.Updated in class <c>sones.Library.Commons.VertexStore.Definitions.Update.AGraphElementUpdateDefinition</c>
        /// </summary>
        [DataMember]
        public Dictionary<Int64, IComparable> UpdatedStructuredProperties;

        /// <summary>
        /// Maps server side property UpdatedStructuredProperties.Deleted in class <c>sones.Library.Commons.VertexStore.Definitions.Update.AGraphElementUpdateDefinition</c>
        /// </summary>
        [DataMember]
        public List<Int64> DeletedStructuredProperties;

        /// <summary>
        /// Maps server side property UpdatedUnstructuredProperties.Updated in class <c>sones.Library.Commons.VertexStore.Definitions.Update.AGraphElementUpdateDefinition</c>
        /// </summary>
        [DataMember]
        public Dictionary<String, Object> UpdatedUnstructuredProperties;

        /// <summary>
        /// Maps server side property UpdatedUnstructuredProperties.Deleted in class <c>sones.Library.Commons.VertexStore.Definitions.Update.AGraphElementUpdateDefinition</c>
        /// </summary>
        [DataMember]
        public List<String> DeletedUnstructuredProperties;
    }
}
