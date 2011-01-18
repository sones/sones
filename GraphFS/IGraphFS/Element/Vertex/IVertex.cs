using System;
using System.Collections.Generic;
using sones.Library.Internal.Definitions;
using System.IO;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertex : IGraphElement, IVertexStatistics
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
        /// <returns>An IEnumerable of edges</returns>
        IEnumerable<IHyperEdge> GetAllIncomingEdges();

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
        /// <returns>An IEnumerable of outgoing edges</returns>
        IEnumerable<IEdge> GetAllOutgoingEdges();

        /// <summary>
        /// Returns all outgoing hyper edges
        /// </summary>
        /// <returns>An IEnumerable of hyper edges</returns>
        IEnumerable<IHyperEdge> GetAllOutgoingHyperEdges();

        /// <summary>
        /// Returns all outgoing single edges
        /// </summary>
        /// <returns>An IEnumerable of single edges</returns>
        IEnumerable<ISingleEdge> GetAllOutgoingSingleEdges();

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
        /// <returns>An IEnumerable of streams</returns>
        IEnumerable<Stream> GetAllBinaryProperties();

        #endregion
    }
}
