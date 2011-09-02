using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Spatial
{
    /// <summary>
    /// Simple definition of a circle geometry.
    /// </summary>
    public interface ICircle : IGeometry
    {
        /// <summary>
        /// Center point
        /// </summary>
        IPoint Center { get; }

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        double Radius { get; }
    }
}
