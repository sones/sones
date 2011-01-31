using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Internal.Definitions
{
    /// <summary>
    /// The species of an edge
    /// </summary>
    public enum EdgeSpecies
    {
        /// <summary>
        /// 1-N relation
        /// </summary>
        HyperEdge,

        /// <summary>
        /// 1-1 relation
        /// </summary>
        SingleEdge
    }
}
