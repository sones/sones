using System.Collections.Generic;
using System;
using sones.Library.Commons.VertexStore.Definitions.Update;

namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// This class represents the filesystem update definition for a vertex
    /// </summary>
    public sealed class VertexUpdateDefinition : AGraphElementUpdateDefinition
    {
        #region data

        /// <summary>
        /// The binary properties
        /// </summary>
        public readonly BinaryPropertiesUpdate UpdatedBinaryProperties;

        /// <summary>
        /// The to be updated single edges
        /// </summary>
        public readonly SingleEdgeUpdate UpdatedSingleEdges;

        /// <summary>
        /// The to be updated hyper edges
        /// </summary>
        public readonly HyperEdgeUpdate UpdateHyperEdges;

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new vertex update definition
        /// </summary>
        /// <param name="myCommentUpdate">The comment update</param>
        /// <param name="myUpdatedStructuredProperties">The update for the structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The update for the unstructured properties</param>
        /// <param name="myUpdatedBinaryProperties">The update for the binary properties</param>
        /// <param name="mySingleEdgeUpdate">The update for the single edges</param>
        /// <param name="myHyperEdgeUpdate">The update for the hyper edges</param>
        public VertexUpdateDefinition(
            String myCommentUpdate = null, 
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null, 
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null,
            BinaryPropertiesUpdate myUpdatedBinaryProperties = null,
            SingleEdgeUpdate mySingleEdgeUpdate = null,
            HyperEdgeUpdate myHyperEdgeUpdate = null)
            : base(myCommentUpdate, myUpdatedStructuredProperties, myUpdatedUnstructuredProperties)
        {
            UpdatedBinaryProperties = myUpdatedBinaryProperties;
            UpdatedSingleEdges = mySingleEdgeUpdate;
            UpdateHyperEdges = myHyperEdgeUpdate;
        } 
        
        #endregion
    }
}