using System;
using System.Collections.Generic;
using System.IO;

namespace sones.GraphInfrastructure.Element
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertex : IGraphElement, IVertexStatistics, IGraphPartitionInformation
    {
        #region Properties

        /// <summary>
        /// A vertex is identified by the name of its vertex type and Guid
        /// </summary>
        /// <returns>A vertex id</returns>
        VertexID GetVertexID();

        /// <summary>
        /// Returns the revision id of this vertex
        /// </summary>
        /// <returns></returns>
        VertexRevisionID GetVertexRevisionID();

        /// <summary>
        /// Returns the name of the edition of this vertex
        /// </summary>
        /// <returns></returns>
        String GetEditionName();

        #endregion

        #region Edges

        #region Incoming
        //Incoming edges are allways hyper edges

        /// <summary>
        /// Is there a specified incoming edge?
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasIncomingEdge(PropertyID myEdgePropertyID);

        /// <summary>
        /// Returns all incoming edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those hyper edges</param>
        /// <returns>An IEnumerable of propertyID/incoming edge KVP</returns>
        IEnumerable<KeyValuePair<PropertyID, IHyperEdge>> GetAllIncomingEdges(Func<PropertyID, IHyperEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns a specified incoming edge
        /// </summary>
        /// <param name="myEdgePropertyID"></param>
        /// <returns></returns>
        IHyperEdge GetIncomingHyperEdge(PropertyID myEdgePropertyID);

        #endregion

        #region Outgoing

        /// <summary>
        /// Is there a specified outgoing edge?
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the interesting edge</param>
        /// <returns>True if there is a specified edge, otherwise false</returns>
        Boolean HasOutgoingEdge(PropertyID myEdgePropertyID);

        /// <summary>
        /// Returns all outgoing edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges</param>
        /// <returns>An IEnumerable of propertyID/outgoing edges</returns>
        IEnumerable<KeyValuePair<PropertyID, IEdge>> GetAllOutgoingEdges(Func<PropertyID, IEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges</param>
        /// <returns>An IEnumerable of propertyID/hyper edge KVP</returns>
        IEnumerable<KeyValuePair<PropertyID, IHyperEdge>> GetAllOutgoingHyperEdges(Func<PropertyID, IHyperEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <param name="myFilterFunc">A function to filter those edges</param>
        /// <returns>An IEnumerable of PropertyID/single edge KVP</returns>
        IEnumerable<KeyValuePair<PropertyID, ISingleEdge>> GetAllOutgoingSingleEdges(Func<PropertyID, ISingleEdge, bool> myFilterFunc = null);

        /// <summary>
        /// Returns a specified edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>An IEdge</returns>
        IEdge GetOutgoingEdge(PropertyID myEdgePropertyID);

        /// <summary>
        /// Returns a specified hyper edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A hyper edge</returns>
        IHyperEdge GetOutgoingHyperEdge(PropertyID myEdgePropertyID);

        /// <summary>
        /// Get a specified single edge
        /// </summary>
        /// <param name="myEdgePropertyID">The property id of the specified edge</param>
        /// <returns>A single edge</returns>
        ISingleEdge GetOutgoingSingleEdge(PropertyID myEdgePropertyID);

        #endregion

        #endregion

        #region Binary data

        /// <summary>
        /// Returns a specified binary property
        /// </summary>
        /// <param name="myPropertyID">The property id of the specified binary</param>
        /// <returns>A stream</returns>
        Stream GetBinaryProperty(PropertyID myPropertyID);

        /// <summary>
        /// Returns all binary properties
        /// </summary>
        /// <param name="myFilterFunc">A function to filter the binary properties</param> 
        /// <returns>An IEnumerable of PropertyID/stream KVP</returns>
        IEnumerable<KeyValuePair<PropertyID,Stream>> GetAllBinaryProperties(Func<PropertyID, bool> myFilterFunc = null);

        #endregion
    }
}
