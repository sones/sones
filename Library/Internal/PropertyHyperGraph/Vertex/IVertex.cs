using System;
using System.Collections.Generic;
using System.IO;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertex : IGraphElement, IVertexProperties
    {
        #region Edges

        #region Incoming

        /// <summary>
        /// Are there incoming vertices on this vertex?
        /// </summary>
        /// <param name="myVertexTypeID">The id of the vertex type that defines the edge</param>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there are incoming vertices, otherwise false</returns>
        Boolean HasIncomingVertices(Int64 myVertexTypeID, Int64 myEdgePropertyID);

        /// <summary>
        /// Returns all incoming vertices
        /// </summary>
        /// <param name="myFilter">A function to filter those incoming edges (VertexTypeID, EdgeID, ISingleEdges, Bool)</param>
        /// <returns>An IEnumerable of incoming edges</returns>
        IEnumerable<Tuple<Int64, Int64, IEnumerable<IVertex>>> GetAllIncomingVertices(
            PropertyHyperGraphFilter.IncomingVerticesFilter myFilter = null);

        /// <summary>
        /// Return all incoming vertices
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type that points to this IVertex</param>
        /// <param name="myEdgePropertyID">The edge property id that points to this vertex</param>
        /// <returns>All incoming vertices</returns>
        IEnumerable<IVertex> GetIncomingVertices(Int64 myVertexTypeID, Int64 myEdgePropertyID);

        #endregion

        #region Outgoing

        /// <summary>
        /// Is there a specified outgoing edge?
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasOutgoingEdge(Int64 myEdgePropertyID);

        /// <summary>
        /// Returns all outgoing edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, IEdge, Bool)</param>
        /// <returns>An IEnumerable of all outgoing edges</returns>
        IEnumerable<Tuple<Int64, IEdge>> GetAllOutgoingEdges(
            PropertyHyperGraphFilter.OutgoingEdgeFilter myFilter = null);

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, IHyperEdge, Bool)</param>
        /// <returns>An IEnumerable of propertyID/hyper edge KVP</returns>
        IEnumerable<Tuple<Int64, IHyperEdge>> GetAllOutgoingHyperEdges(
            PropertyHyperGraphFilter.OutgoingHyperEdgeFilter myFilter = null);

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges (EdgeID, ISingleEdge, Bool)</param>
        /// <returns>An IEnumerable of all single edges</returns>
        IEnumerable<Tuple<Int64, ISingleEdge>> GetAllOutgoingSingleEdges(
            PropertyHyperGraphFilter.OutgoingSingleEdgeFilter myFilter = null);

        /// <summary>
        /// Returns a specified edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>An IEdge</returns>
        IEdge GetOutgoingEdge(Int64 myEdgePropertyID);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A hyper edge</returns>
        IHyperEdge GetOutgoingHyperEdge(Int64 myEdgePropertyID);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A single edge</returns>
        ISingleEdge GetOutgoingSingleEdge(Int64 myEdgePropertyID);

        #endregion

        #endregion

        #region Binary data

        /// <summary>
        /// Returns a specified binary property
        /// </summary>
        /// <param name="myPropertyID">The property id of the specified binary</param>
        /// <returns>A stream</returns>
        Stream GetBinaryProperty(Int64 myPropertyID);

        /// <summary>
        /// Returns all binary properties
        /// </summary>
        /// <param name="myFilter">A function to filter the binary properties</param> 
        /// <returns>An IEnumerable of PropertyID/stream KVP</returns>
        IEnumerable<Tuple<Int64, Stream>> GetAllBinaryProperties(PropertyHyperGraphFilter.BinaryPropertyFilter myFilter = null);

        #endregion
    }
}