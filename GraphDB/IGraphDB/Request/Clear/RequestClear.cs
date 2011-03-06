namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for clearing the whole graphdb
    /// </summary>
    public sealed class RequestClear : IRequest
    {
        #region Constructor

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadWrite; }
        }

        #endregion
    }
}