using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// A update definition for the hyper edge
    /// </summary>
    public sealed class HyperEdgeUpdateDefinition : AGraphElementUpdateDefinition
    {
        #region data
        
        /// <summary>
        /// The single edges that should be deleted from the hyperedge
        /// </summary>
        public readonly IEnumerable<SingelEdgeVector> ToBeDeletedSingleEdges;
        
        /// <summary>
        /// The single edges that should be updated
        /// </summary>
        public readonly Dictionary<SingelEdgeVector, SingleEdgeUpdateDefinition> ToBeUpdatedSingleEdges;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new HyperEdgeUpdateDefinition
        /// </summary>
        /// <param name="myCommentUpdate">A comment for the graphelement</param>
        /// <param name="myUpdatedStructuredProperties">The structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The unstructured properties</param>
        /// <param name="myToBeDeletedSingleEdges">The single edges that should be deleted from the hyperedge</param>
        /// <param name="myToBeUpdatedSingleEdges">The single edges that should be updated</param>
        public HyperEdgeUpdateDefinition(
            String myCommentUpdate = null,
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null,
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null,
            IEnumerable<SingelEdgeVector> myToBeDeletedSingleEdges = null,
            Dictionary<SingelEdgeVector, SingleEdgeUpdateDefinition> myToBeUpdatedSingleEdges = null)
            : base(myCommentUpdate, myUpdatedStructuredProperties, myUpdatedUnstructuredProperties)
        {
            ToBeDeletedSingleEdges = myToBeDeletedSingleEdges;
            ToBeUpdatedSingleEdges = myToBeUpdatedSingleEdges;
        }

        #endregion
    }
}
