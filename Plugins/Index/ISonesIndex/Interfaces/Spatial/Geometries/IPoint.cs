using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Spatial
{
    /// <summary>
    /// Simple definition of a 2D-point.
    /// </summary>
    public interface IPoint : IGeometry
    {
        /// <summary>
        /// Longitude (X-Value) of that point.
        /// </summary>
        double Longitude { get; }

        /// <summary>
        /// Latitude (Y-Value) of that point.
        /// </summary>
        double Latitude { get; }

        /// <summary>
        /// GeoHash of that point.
        /// </summary>
        string GeoHash { get; }
    }
}
