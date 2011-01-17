using System;
using System.Collections.Generic;

namespace GraphFS.Element
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertex : IGraphElement
    {
        /// Returns all vertices that aim to this vertex
        /// </summary>
        /// <param name="myTypeName">The vertex type of the incoming vertex</param>
        /// <param name="myEdgeName">The name of the incoming edge</param>
        /// <returns>All incoming vertices corresponding to their vertex type and edge</returns>
        IEnumerable<IVertex> GetIncomingVertices(string myTypeName, string myEdgeName);

        /// <summary>
        /// Returns all vertices that are connected via an outgoing edge
        /// </summary>
        /// <param name="myEdgeName">The name of the edge that targets on the interesting vertices</param>
        /// <returns>Outgoing vertices</returns>
        IEnumerable<IVertex> GetOutgoingVertices(string myEdgeName);

        /// <summary>
        /// Returns an outgoing edge
        /// </summary>
        /// <param name="myEdgeName">The name of the edge</param>
        /// <returns>An outgoing edge</returns>
        IEdge GetOutgoingEdge(string myEdgeName);

        /// <summary>
        /// Returns all incoming vertices
        /// </summary>
        /// <returns>An IEnumerable of incoming vertices</returns>
        IEnumerable<IVertex> GetAllIncomingVertices();

        /// <summary>
        /// Returns all outgoing edges
        /// </summary>
        /// <returns>An IEnumerable of outgoing edges</returns>
        IEnumerable<IEdge> GetAllOutgoingEdges();

        /// <summary>
        /// A vertex is identified by the name of its vertex type and Guid
        /// </summary>
        /// <returns>A vertex id</returns>
        VertexID GetVertexID();
    }
}
