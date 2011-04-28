using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.Delete
{
    public sealed class RequestDelete : IRequest
    {
        #region Constructor


        public RequestDelete()
        {
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
