using System;
using System.Collections.Generic;
using System.IO;

namespace sones.PropertyHyperGraph
{
    /// <summary>
    /// The interface for edge properties
    /// </summary>
    public interface IEdgeProperties
    {
        #region ID

        /// <summary>
        /// The edge id
        /// </summary>
        EdgeID EdgeID { get; }

        #endregion
    }
}
