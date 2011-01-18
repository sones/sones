using System;
using System.Collections.Generic;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The interface for all edge species
    /// </summary>
    public interface IEdge : IGraphElement
    {
        #region Source

        IVertex GetSourceVertex();

        #endregion

        #region Targets

        IEnumerable<IVertex> GetTargetVertices();

        #endregion
    }
}
