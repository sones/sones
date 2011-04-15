namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertices request
    /// </summary>
    public sealed class RequestGetVertices : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the vertices that should be requested from the graphdb
        /// </summary>
        public readonly GetVerticesDefinition GetVerticesDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that gets vertices from the Graphdb
        /// </summary>
        /// <param name="myGetVerticesDefinition">The definition of the vertices that should be requested from the graphdb</param>
        public RequestGetVertices(GetVerticesDefinition myGetVerticesDefinition)
        {
            GetVerticesDefinition = myGetVerticesDefinition;
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
