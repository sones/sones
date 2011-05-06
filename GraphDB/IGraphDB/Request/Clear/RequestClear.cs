namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for clearing the whole graphdb
    /// </summary>
    public sealed class RequestClear : IRequest
    {
        #region Constructor

        /// <summary>
        /// Returns a new clear request
        /// </summary>
        public RequestClear()
        { }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadWrite; }
        }

        #endregion
    }
}