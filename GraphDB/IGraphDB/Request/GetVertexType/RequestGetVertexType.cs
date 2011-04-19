using System;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertex type request
    /// </summary>
    public sealed class RequestGetVertexType : IRequest
    {
        #region data

        /// <summary>
        /// The interesting vertex type id
        /// </summary>
        public readonly Int64 VertexTypeID;

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
        /// Creates a new request gets a vertex type from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeName">The interesting vertex type name</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetVertexType(String myVertexTypeName, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeName = myVertexTypeName;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
        }

        /// <summary>
        /// Creates a new request gets a vertex type from the Graphdb
        /// </summary>
        /// <param name="myVertexTypeId">The interesting vertex type id</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public RequestGetVertexType(Int64 myVertexTypeId, String myEdition = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            VertexTypeID = myVertexTypeId;
            Edition = myEdition;
            Timespan = myTimeSpanDefinition;
            VertexTypeName = null;
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
