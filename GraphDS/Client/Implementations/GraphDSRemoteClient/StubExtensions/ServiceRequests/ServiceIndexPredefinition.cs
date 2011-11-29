using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceIndexPredefinition
    {
        internal ServiceIndexPredefinition(IndexPredefinition myIndexPredefinition)
        {
            this.Edition = myIndexPredefinition.Edition;
            this.IndexTypeName = myIndexPredefinition.TypeName;
            this.Name = myIndexPredefinition.Name;
            this.IndexOptions = myIndexPredefinition.IndexOptions;
            this.Properties = myIndexPredefinition.Properties.ToList();
            this.VertexTypeName = myIndexPredefinition.VertexTypeName;
            this.Comment = myIndexPredefinition.Comment;
        }
    }
}
