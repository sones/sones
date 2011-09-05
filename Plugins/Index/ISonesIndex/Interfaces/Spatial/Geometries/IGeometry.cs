using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Spatial
{
    /// <summary>
    /// Parent interface for all geometries used by spatial indexes.
    /// </summary>
    public interface IGeometry : IComparable
    {
        /// <summary>
        /// Returns the minimum bounding rectangle (MBR)
        /// of that geometry.
        /// 
        /// <see cref="http://en.wikipedia.org/wiki/Minimum_bounding_rectangle"/>
        /// </summary>
        /// <returns>MBR of that geometry</returns>
        IRectangle GetMBR();
    }
}
