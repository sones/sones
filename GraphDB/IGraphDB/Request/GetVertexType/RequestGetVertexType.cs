namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertex type request
    /// </summary>
    public sealed class RequestGetVertexType : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the vertex type that should be requested from the graphdb
        /// </summary>
        public readonly GetVertexTypeDefinition GetVertexTypeDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request gets a vertex type from the Graphdb
        /// </summary>
        /// <param name="myGetEdgeTypeDefinition">The definition of the vertex type that should be requested from the graphdb</param>
        public RequestGetVertexType(GetVertexTypeDefinition myGetVertexTypeDefinition)
        {
            GetVertexTypeDefinition = myGetVertexTypeDefinition;
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
