using System;
using System.Collections.Generic;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for clearing the whole graphdb
    /// </summary>
    public sealed class RequestClear : IRequest
    {

        #region data

        /// <summary>
        /// The request stats
        /// </summary>
        private RequestStatistics _stats = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        public RequestClear()
        {
        }

        #endregion

        #region IRequest<TResult> Members

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
