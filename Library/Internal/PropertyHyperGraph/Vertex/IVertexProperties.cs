using System;
using System.Collections.Generic;
using System.IO;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertexProperties
    {
        #region ID / Edition / Revision

        /// <summary>
        /// A vertex is identified by the name of its vertex type and Guid
        /// </summary>
        VertexID VertexID { get; }

        /// <summary>
        /// Returns the revision id of this vertex
        /// </summary>
        VertexRevisionID VertexRevisionID { get; }

        /// <summary>
        /// Returns the name of the edition of this vertex
        /// </summary>
        String EditionName { get; }

        #endregion

        #region Degree

        /// <summary>
        /// For a vertex, the number of incoming edges is called the indegree 
        /// </summary>
        UInt64 InDegree { get; }

        /// <summary>
        /// For a vertex, the number outgoing edges is called the outdegree 
        /// </summary>
        UInt64 OutDegree { get; }

        /// <summary>
        /// For a vertex the number of incoming plus the number of outgoing edges is called the degree
        /// </summary>
        UInt64 Degree { get; }

        #endregion

        #region Statistics

        /// <summary>
        /// Statistics concerning the vertex
        /// </summary>
        IVertexStatistics Statistics { get; }

        #endregion

        #region PartitionInformation

        /// <summary>
        /// Informations concerning the current partition
        /// </summary>
        IGraphPartitionInformation PartitionInformation { get; }

        #endregion
    }
}
