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
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasIncomingHyperEdge(UInt64 myEdgePropertyID);

        /// <summary>
        /// Returns all incoming edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those hyper edges</param>
        /// <returns>An IEnumerable of propertyID/incoming edge KVP</returns>
        IEnumerable<KeyValuePair<UInt64, IHyperEdge>> GetAllIncomingHyperEdges(Func<UInt64, IHyperEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns a specified incoming edge
        /// </summary>
        /// <param name="myEdgePropertyID"></param>
        /// <returns></returns>
        IHyperEdge GetIncomingHyperEdge(UInt64 myEdgePropertyID);

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
        /// <param name="myFilterFunc">A function to filter those edges</param>
        /// <returns>An IEnumerable of propertyID/outgoing edges</returns>
        IEnumerable<KeyValuePair<UInt64, IEdge>> GetAllOutgoingEdges(Func<UInt64, IEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges</param>
        /// <returns>An IEnumerable of propertyID/hyper edge KVP</returns>
        IEnumerable<KeyValuePair<UInt64, IHyperEdge>> GetAllOutgoingHyperEdges(Func<UInt64, IHyperEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges</param>
        /// <returns>An IEnumerable of PropertyID/single edge KVP</returns>
        IEnumerable<KeyValuePair<UInt64, ISingleEdge>> GetAllOutgoingSingleEdges(Func<UInt64, ISingleEdge, bool> myFilterFunc = null);

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
        IEnumerable<KeyValuePair<UInt64, Stream>> GetAllBinaryProperties(Func<UInt64, bool> myFilterFunc = null);

        #endregion
    }
}
