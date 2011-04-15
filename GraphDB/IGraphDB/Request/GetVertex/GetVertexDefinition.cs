using System;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of what kind of vertex should be requested from the graphdb
    /// </summary>
    public sealed class GetVertexDefinition
    {
        #region Data

        /// <summary>
        /// The vertex type name of the requested vertex
        /// </summary>
        public readonly String VertexTypeName;

        /// <summary>
        /// The id of the requested vertex
        /// </summary>
        public readonly Int64 VertexID;

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
        /// Creates a new get vertex definition
        /// </summary>
        /// <param name="myVertexTypeName">The vertex type name of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public GetVertexDefinition(String myVertexTypeName, Int64 myVertexID, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeName = myVertexTypeName;
            VertexID = myVertexID;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        #endregion
    }
}