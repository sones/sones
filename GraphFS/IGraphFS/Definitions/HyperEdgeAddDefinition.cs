using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.PropertyHyperGraph;
using System.IO;
using sones.GraphFS.Definitions;

namespace sones.GraphFS
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public struct HyperEdgeAddDefinition
    {
        #region data

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
        /// <param name="myGraphElementInformation">The graph element properties for this hyperedge</param>
        /// <param name="mySourceVertex">The source vertex of this hyper edge</param>
        /// <param name="myContainedSingleEdges">The single edges that are contained within this hyper edge</param>
        public HyperEdgeAddDefinition(
            GraphElementInformation myGraphElementInformation,
            VertexInformation mySourceVertex,
            IEnumerable<SingleEdgeAddDefinition> myContainedSingleEdges)
        {
            SourceVertex = mySourceVertex;
            ContainedSingleEdges = myContainedSingleEdges;
            GraphElementInformation = myGraphElementInformation;
        }

        #endregion
    }
}
