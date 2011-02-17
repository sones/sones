namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for creating a new vertex
    /// </summary>
    public sealed class RequestInsertVertex : IRequest
    {
        #region data

        /// <summary>
        /// The definition of the vertex that is going to be inserted
        /// </summary>
        public readonly VertexInsert VertexInsertDefinition;

        /// <summary>
        /// The stats of the request
        /// </summary>
        private RequestStatistics _stats;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        /// <param name="myVertexInsertDefinition">Describes the vertex that is going to be inserted</param>
        public RequestInsertVertex(VertexInsert myVertexInsertDefinition)
        {
            VertexInsertDefinition = myVertexInsertDefinition;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadWrite; }
        }

        public void SetStatistics(IRequestStatistics myRequestStatistics)
        {
            _stats = myRequestStatistics as RequestStatistics;
        }

        #endregion
    }
}