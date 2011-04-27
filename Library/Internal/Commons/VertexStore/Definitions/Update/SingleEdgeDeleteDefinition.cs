using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The definition of the direction of a single edge
    /// </summary>
    public sealed class SingleEdgeDeleteDefinition
    {
        #region data

        /// <summary>
        /// The source vertex.
        /// </summary>
        public readonly VertexInformation SourceVertex;
        
        /// <summary>
        /// The target vertex.
        /// </summary>
        public readonly VertexInformation TargetVertex;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of SingleEdgeDeleteDefinition.
        /// </summary>
        /// <param name="mySourceVertex">The vertex where the edge begins.</param>
        /// <param name="myTargetVertex">The vertex where the edge ends.</param>
        public SingleEdgeDeleteDefinition(VertexInformation mySourceVertex, VertexInformation myTargetVertex)
        {
            SourceVertex = mySourceVertex;
            TargetVertex = myTargetVertex;
        }

        #endregion
    }
}
