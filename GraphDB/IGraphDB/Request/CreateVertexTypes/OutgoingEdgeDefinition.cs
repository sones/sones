using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for outgoing edges
    /// </summary>
    public sealed class OutgoingEdgeDefinition
    {
        #region Data

        /// <summary>
        /// The name of the edge
        /// </summary>
        public readonly String EdgeName;

        /// <summary>
        /// The species of the edge
        /// </summary>
        public readonly EdgeSpecies EdgeSpecies;

        /// <summary>
        /// The name of the edge type
        /// </summary>
        public readonly String EdgeTypeName;

        /// <summary>
        /// The vertex type this edge points to
        /// </summary>
        public readonly String VertexTypeName;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a definition for an outgoing edge
        /// </summary>
        /// <param name="myEdgeName">The name of the edge</param>
        /// <param name="myEdgeTypeName">The type of the edge</param>
        /// <param name="myVertexSchemeName">The vertex type this edge points to</param>
        /// <param name="myEdgeSpecies">The species of the edge</param>
        public OutgoingEdgeDefinition(String myEdgeName, String myEdgeTypeName, String myVertexSchemeName,
                                      EdgeSpecies myEdgeSpecies)
        {
            EdgeName = myEdgeName;
            EdgeTypeName = myEdgeTypeName;
            VertexTypeName = myVertexSchemeName;
            EdgeSpecies = myEdgeSpecies;
        }

        #endregion
    }
}