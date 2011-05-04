namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for altering a edge type
    /// </summary>
    public sealed class RequestAlterEdgeType : IRequest
    {
        #region Constructor

        public RequestAlterEdgeType()
        {

        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadWrite; }
        }

        #endregion
    }
}