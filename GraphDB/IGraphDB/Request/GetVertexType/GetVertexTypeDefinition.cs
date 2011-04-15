using System;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of what kind of vertex type should be requested from the graphdb
    /// </summary>
    public sealed class GetVertexTypeDefinition
    {
        #region Data

        /// <summary>
        /// The interesting vertex type name
        /// </summary>
        public readonly String VertexTypeName;

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new get vertex type definition
        /// </summary>
        /// <param name="myEdgeTypeName">The interesting vertex type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public GetVertexTypeDefinition(String myVertexTypeName, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeName = myVertexTypeName;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        #endregion
    }
}