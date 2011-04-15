namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertex request
    /// </summary>
    public sealed class RequestGetVertex : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the vertex that should be requested from the graphdb
        /// </summary>
        public readonly GetVertexDefinition GetVertexDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request gets a vertex from the Graphdb
        /// </summary>
        /// <param name="myGetVertexDefinition">The definition of the vertex that should be requested from the graphdb</param>
        public RequestGetVertex(GetVertexDefinition myGetVertexDefinition)
        {
            GetVertexDefinition = myGetVertexDefinition;
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
