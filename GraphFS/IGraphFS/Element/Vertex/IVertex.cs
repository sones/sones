using System;
using System.Collections.Generic;
using sones.Library.Internal.Definitions;

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

        Boolean HasIncomingEdge(PropertyID myEdgePropertyID);

        IEnumerable<IEdge> GetIncomingEdges();

        IEnumerable<IHyperEdge> GetIncomingHyperEdges();

        IEnumerable<ISingleEdge> GetIncomingSingleEdges();

        IEdge GetIncomingEdge(PropertyID myEdgePropertyID);

        IHyperEdge GetIncomingHyperEdge(PropertyID myEdgePropertyID);

        ISingleEdge GetIncomingSingleEdge(PropertyID myEdgePropertyID);

        #endregion

        #region Outgoing

        Boolean HasOutgoingEdge(PropertyID myEdgePropertyID);

        IEnumerable<IEdge> GetOutgoingEdges();

        IEnumerable<IHyperEdge> GetOutgoingHyperEdges();

        IEnumerable<ISingleEdge> GetOutgoingSingleEdges();

        IEdge GetOutgoingEdge(PropertyID myEdgePropertyID);

        IHyperEdge GetOutgoingHyperEdge(PropertyID myEdgePropertyID);

        ISingleEdge GetOutgoingSingleEdge(PropertyID myEdgePropertyID);

        #endregion

        #endregion

    }
}
