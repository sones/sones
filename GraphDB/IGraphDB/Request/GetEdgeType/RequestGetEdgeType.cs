using System;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get edge type request
    /// </summary>
    public sealed class RequestGetEdgeType : IRequest
    {
        #region data

        /// <summary>
        /// The interesting edge type id
        /// </summary>
        public readonly Int64 EdgeTypeID;

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
        /// Creates a new request gets a edge type from the Graphdb
        /// </summary>
        /// <param name="myEdgeTypeName">The interesting edge type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetEdgeType(String myEdgeTypeName, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            EdgeTypeName = myEdgeTypeName;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        /// <summary>
        /// Creates a new request gets a edge type from the Graphdb
        /// </summary>
        /// <param name="myEdgeTypeID">The interesting edge type id</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetEdgeType(Int64 myEdgeTypeID, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            EdgeTypeName = null;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
            EdgeTypeID = myEdgeTypeID;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadOnly; }
        }

        #endregion
    }
}
