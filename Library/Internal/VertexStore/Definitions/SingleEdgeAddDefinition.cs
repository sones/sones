using System;
using System.Collections.Generic;

namespace sones.Library.VertexStore.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public struct SingleEdgeAddDefinition
    {
        #region data

        /// <summary>
        /// A comment for the vertex
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        public readonly long CreationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        public readonly long ModificationDate;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly Dictionary<Int64, IComparable> StructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly Dictionary<String, Object> UnstructuredProperties;

        /// <summary>
        /// The property id of the edge
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The edge type id of this edge
        /// </summary>
        public readonly Int64 EdgeTypeID;

        /// <summary>
        /// The source vertex information
        /// </summary>
        public readonly VertexInformation SourceVertexInformation;

        /// <summary>
        /// The target vertex informantion
        /// </summary>
        public readonly VertexInformation TargetVertexInformation;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new single edge definition
        /// </summary>
        /// <param name="myPropertyID">The id of the edge property</param>
        /// <param name="myEdgeTypeID">The id of this edge type</param>
        /// <param name="mySourceVertexInformation">The target vertex informantion</param>
        /// <param name="myTargetVertexInformation">The graph element properties</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public SingleEdgeAddDefinition(
            Int64 myPropertyID,
            Int64 myEdgeTypeID,
            VertexInformation mySourceVertexInformation,
            VertexInformation myTargetVertexInformation,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            Dictionary<Int64, IComparable> myStructuredProperties,
            Dictionary<String, Object> myUnstructuredProperties)
        {
            PropertyID = myPropertyID;
            EdgeTypeID = myEdgeTypeID;
            SourceVertexInformation = mySourceVertexInformation;
            TargetVertexInformation = myTargetVertexInformation;
            Comment = myComment;
            CreationDate = myCreationDate;
            ModificationDate = myModificationDate;
            StructuredProperties = myStructuredProperties;
            UnstructuredProperties = myUnstructuredProperties;

        }

        #endregion
    }
}