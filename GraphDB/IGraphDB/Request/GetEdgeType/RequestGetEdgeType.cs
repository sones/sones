namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get edge type request
    /// </summary>
    public sealed class RequestGetEdgeType : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the edge type that should be requested from the graphdb
        /// </summary>
        public readonly GetEdgeTypeDefinition GetEdgeTypeDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request gets a edge type from the Graphdb
        /// </summary>
        /// <param name="myGetEdgeTypeDefinition">The definition of the edge type that should be requested from the graphdb</param>
        public RequestGetEdgeType(GetEdgeTypeDefinition myGetEdgeTypeDefinition)
        {
            GetEdgeTypeDefinition = myGetEdgeTypeDefinition;
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
