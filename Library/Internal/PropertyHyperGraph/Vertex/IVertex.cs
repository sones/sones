using System;
using System.Collections.Generic;
using System.IO;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertex : IGraphElement, IVertexProperties
    {
        #region Edges

        #region Incoming

        //Incoming edges are always hyper edges

        /// <summary>
        /// Is there a specified incoming edge?
        /// </summary>
        /// <param name="myVertexTypeID">The id of the vertex type that defines the edge</param>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasIncomingEdge(UInt64 myVertexTypeID, UInt64 myEdgePropertyID);

        /// <summary>
        /// Returns all incoming edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those hyper edges (VertexTypeID, EdgeID, HyperEdge, Bool)</param>
        /// <returns>An IEnumerable of incoming hyper edges</returns>
        IEnumerable<Tuple<UInt64, UInt64, IHyperEdge>> GetAllIncomingEdges(
            Func<UInt64, UInt64, IHyperEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns a specified incoming edge
        /// </summary>
        /// <param name="myVertexTypeID">The id of the vertex type that defines the edge</param>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>The specified incoming edge or null</returns>
        IHyperEdge GetIncomingHyperEdge(UInt64 myVertexTypeID, UInt64 myEdgePropertyID);

        #endregion

        #region Outgoing

        /// <summary>
        /// Is there a specified outgoing edge?
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasOutgoingEdge(UInt64 myEdgePropertyID);

        /// <summary>
        /// Returns all outgoing edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges (EdgeID, IEdge, Bool)</param>
        /// <returns>An IEnumerable of all outgoing edges</returns>
        IEnumerable<Tuple<UInt64, IEdge>> GetAllOutgoingEdges(Func<UInt64, IEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges (EdgeID, IHyperEdge, Bool)</param>
        /// <returns>An IEnumerable of propertyID/hyper edge KVP</returns>
        IEnumerable<Tuple<UInt64, IHyperEdge>> GetAllOutgoingHyperEdges(
            Func<UInt64, IHyperEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges (EdgeID, ISingleEdge, Bool)</param>
        /// <returns>An IEnumerable of all single edges</returns>
        IEnumerable<Tuple<UInt64, ISingleEdge>> GetAllOutgoingSingleEdges(
            Func<UInt64, ISingleEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns a specified edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>An IEdge</returns>
        IEdge GetOutgoingEdge(UInt64 myEdgePropertyID);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A hyper edge</returns>
        IHyperEdge GetOutgoingHyperEdge(UInt64 myEdgePropertyID);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A single edge</returns>
        ISingleEdge GetOutgoingSingleEdge(UInt64 myEdgePropertyID);

        #endregion

        #endregion

        #region Binary data

        /// <summary>
        /// Returns a specified binary property
        /// </summary>
        /// <param name="myPropertyID">The property id of the specified binary</param>
        /// <returns>A stream</returns>
        Stream GetBinaryProperty(UInt64 myPropertyID);

        /// <summary>
        /// Returns all binary properties
        /// </summary>
        /// <param name="myFilterFunc">A function to filter the binary properties</param> 
        /// <returns>An IEnumerable of PropertyID/stream KVP</returns>
        IEnumerable<Tuple<UInt64, Stream>> GetAllBinaryProperties(Func<UInt64, Stream, bool> myFilterFunc = null);

        #endregion
    }
}