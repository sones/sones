using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for creating a new edge type
    /// </summary>
    public sealed class RequestCreateEdgeType : IRequest
    {
        #region Constructor

        public RequestCreateEdgeType()
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
