namespace sones.GraphDB.Request
{
    /// <summary>
    /// The traverse vertex request
    /// </summary>
    public sealed class RequestTraverseVertex : IRequest
    {
        #region data

        /// <summary>
        /// The definition / start node wich should be requested from the graphdb
        /// </summary>
        public readonly TraverseVertexDefinition TraverseVertexDefinition;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that traverses verticies
        /// </summary>
        /// <param name="myGetVerticesDefinition">The definition of the vertices that should be requests from the graphdb</param>
        public RequestTraverseVertex(TraverseVertexDefinition myTraverseVertexDefinition)
        {
            TraverseVertexDefinition = myTraverseVertexDefinition;
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
