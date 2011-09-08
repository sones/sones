using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Spatial
{
    /// <summary>
    /// Simple definition of a rectangle geometry.
    /// </summary>
    public interface IRectangle : IGeometry
    {
        /// <summary>
        /// Lower left point of that rectangle.
        /// </summary>
        IPoint LowerLeft { get; }

        /// <summary>
        /// Upper right point of that rectangle.
        /// </summary>
        IPoint UpperRight { get; }

        /// <summary>
        /// Calculates the intersect with another reactangle.
        /// </summary>
        /// <param name="myRectangle"></param>
        /// <returns></returns>
        IRectangle Intersect(IRectangle myRectangle);
    }
}
