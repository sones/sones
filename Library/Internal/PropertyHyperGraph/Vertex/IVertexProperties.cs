using System;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for vertices
    /// </summary>
    public interface IVertexProperties
    {
        #region ID / Edition / Revision

        /// <summary>
        /// The id of the vertex type
        /// </summary>
        Int64 VertexTypeID { get; }

        /// <summary>
        /// The id of the vertex
        /// </summary>
        Int64 VertexID { get; }

        /// <summary>
        /// Returns the revision id of this vertex
        /// </summary>
        VertexRevisionID VertexRevisionID { get; }

        /// <summary>
        /// Returns the name of the edition of this vertex
        /// </summary>
        String EditionName { get; }

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