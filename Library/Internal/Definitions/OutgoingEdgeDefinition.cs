using System;

namespace sones.Library.Internal.Definitions
{
    /// <summary>
    /// The definition for outgoing edges
    /// </summary>
    public sealed class OutgoingEdgeDefinition
    {
        #region Data

        /// <summary>
        /// The species of the edge
        /// </summary>
        readonly public EdgeSpeciesEnum EdgeSpecies;

        /// <summary>
        /// The vertex type this edge points to
        /// </summary>
        readonly public String VertexTypeName;

        /// <summary>
        /// The name of the edge
        /// </summary>
        readonly public String EdgeName;

        /// <summary>
        /// The name of the edge type
        /// </summary>
        readonly public String EdgeTypeName;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a definition for an outgoing edge
        /// </summary>
        /// <param name="myEdgeName">The name of the edge</param>
        /// <param name="myEdgeTypeName">The type of the edge</param>
        /// <param name="myVertexSchemeName">The vertex type this edge points to</param>
        /// <param name="myEdgeSpecies">The species of the edge</param>
        public OutgoingEdgeDefinition(String myEdgeName, String myEdgeTypeName, String myVertexSchemeName, EdgeSpeciesEnum myEdgeSpecies)
        {
            EdgeName = myEdgeName;
            EdgeTypeName = myEdgeTypeName;
            VertexTypeName = myVertexSchemeName;
            EdgeSpecies = myEdgeSpecies;
        }

        #endregion
    }
}
