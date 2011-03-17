using System.Collections.Generic;
using System;

namespace sones.GraphFS.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public sealed class HyperEdgeAddDefinition
    {
        #region data

        /// <summary>
        /// The property id of the edge
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The single edges that are contained in this hyperedge
        /// </summary>
        public readonly IEnumerable<SingleEdgeAddDefinition> ContainedSingleEdges;

        /// <summary>
        /// Properties
        /// </summary>
        public readonly GraphElementInformation GraphElementInformation;

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
        /// <param name="myGraphElementInformation">The graph element properties for this hyperedge</param>
        /// <param name="mySourceVertex">The source vertex of this hyper edge</param>
        /// <param name="myContainedSingleEdges">The single edges that are contained within this hyper edge</param>
        public HyperEdgeAddDefinition(
            Int64 myPropertyID,
            GraphElementInformation myGraphElementInformation,
            VertexInformation mySourceVertex,
            IEnumerable<SingleEdgeAddDefinition> myContainedSingleEdges)
        {
            PropertyID = myPropertyID;
            SourceVertex = mySourceVertex;
            ContainedSingleEdges = myContainedSingleEdges;
            GraphElementInformation = myGraphElementInformation;
        }

        #endregion
    }
}