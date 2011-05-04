
namespace sones.GraphDB.Request.AlterType
{
    /// <summary>
    /// Request to alter a vertex type
    /// </summary>
    public sealed class RequestAlterType : IRequest
    {
        #region data

        //some data, which henning won't say me

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new alter type request 
        /// </summary>
        public RequestAlterType()
        { 
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion
    }
}
