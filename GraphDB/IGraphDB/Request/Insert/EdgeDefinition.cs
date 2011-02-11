using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of an edge
    /// </summary>
    public sealed class EdgeDefinition
    {
        #region data

        /// <summary>
        /// The name of the edge that should be filled
        /// </summary>
        public readonly String EdgeName;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new edge definition to be able to define a connection towards other vertices 
        /// </summary>
        /// <param name="myEdgeName"></param>
        public EdgeDefinition(String myEdgeName)
        {

        }

        #endregion
    }
}
