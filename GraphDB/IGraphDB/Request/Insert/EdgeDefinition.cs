using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of an IncomingEdge
    /// </summary>
    public sealed class EdgeDefinition
    {
        #region data

        /// <summary>
        /// The name of the IncomingEdge that should be filled
        /// </summary>
        public readonly String EdgeName;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new IncomingEdge definition to be able to define a connection towards other vertices 
        /// </summary>
        /// <param name="myEdgeName"></param>
        public EdgeDefinition(String myEdgeName)
        {
        }

        #endregion
    }
}