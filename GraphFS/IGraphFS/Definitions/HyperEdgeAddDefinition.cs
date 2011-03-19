using System.Collections.Generic;
using System;

namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public sealed class HyperEdgeAddDefinition : AGraphElementDefinition
    {
        #region data

        /// <summary>
        /// The property id of the edge
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The edge type id of this edge
        /// </summary>
        public readonly Int64 EdgeTypeID;

        /// <summary>
        /// The single edges that are contained in this hyperedge
        /// </summary>
        public readonly IEnumerable<SingleEdgeAddDefinition> ContainedSingleEdges;

        /// <summary>
        /// The source vertex for this hyper edge
        /// </summary>
        public readonly VertexInformation SourceVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new hyper edge definition
        /// </summary>
        /// <param name="myPropertyID">The property id of the edge</param>
        /// <param name="myEdgeTypeID">The id of this edge type</param>
        /// <param name="mySourceVertex">The source vertex of this hyper edge</param>
        /// <param name="myContainedSingleEdges">The single edges that are contained within this hyper edge</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public HyperEdgeAddDefinition(
            Int64 myPropertyID,
            Int64 myEdgeTypeID,
            VertexInformation mySourceVertex,
            IEnumerable<SingleEdgeAddDefinition> myContainedSingleEdges,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            Dictionary<Int64, Object> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
            : base(myComment, myCreationDate, myModificationDate, myStructuredProperties, myUnstructuredProperties)
        {
            PropertyID = myPropertyID;
            EdgeTypeID = myEdgeTypeID;
            SourceVertex = mySourceVertex;
            ContainedSingleEdges = myContainedSingleEdges;
        }

        #endregion
    }
}