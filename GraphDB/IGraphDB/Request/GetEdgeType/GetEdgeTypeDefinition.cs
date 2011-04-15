using System;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of what kind of edge type should be requested from the graphdb
    /// </summary>
    public sealed class GetEdgeTypeDefinition
    {
        #region Data

        /// <summary>
        /// The interesting edge type name
        /// </summary>
        public readonly String EdgeTypeName;

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
        /// Creates a new get edge type definition
        /// </summary>
        /// <param name="myEdgeTypeName">The interesting edge type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public GetEdgeTypeDefinition(String myEdgeTypeName, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            EdgeTypeName = myEdgeTypeName;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        #endregion
    }
}