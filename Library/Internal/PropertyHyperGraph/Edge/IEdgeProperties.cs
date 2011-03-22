using System;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for edge properties
    /// </summary>
    public interface IEdgeProperties
    {
        #region EdgeTypeID

        /// <summary>
        /// The id of the edge type
        /// </summary>
        Int64 EdgeTypeID { get; }

        #endregion

        #region Statistics

        /// <summary>
        /// The statistics of an edge
        /// </summary>
        IEdgeStatistics Statistics { get; }

        #endregion
    }
}