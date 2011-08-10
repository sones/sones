using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.Request;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceIndexPredefinition
    {
                     
        [DataMember]
        public string Edition;

        /// <summary>
        /// The name of the index type.
        /// </summary>
        [DataMember]
        public string TypeName;
        
        /// <summary>
        /// The name of the index
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// The options that will be passed to the index instance
        /// </summary>
        [DataMember]
        public String IndexOptions;

        

        /// <summary>
        /// The set of properties that will be indexed.
        /// </summary>
        [DataMember]
        public IEnumerable<String> Properties;

        /// <summary>
        /// The vertexTypeName that defines the index.
        /// </summary>
        [DataMember]
        public string VertexTypeName;

        [DataMember]
        public string Comment;


        public IndexPredefinition ToIndexPredefinition()
        {
            IndexPredefinition IndexPreDef = new IndexPredefinition(this.Name);

            IndexPreDef.SetComment(this.Comment);
            IndexPreDef.SetEdition(this.Edition);

            foreach (var Property in this.Properties)
            {
                IndexPreDef.AddProperty(Property);
            }

            IndexPreDef.SetVertexType(this.VertexTypeName);

            return IndexPreDef;
        }

    }
}
