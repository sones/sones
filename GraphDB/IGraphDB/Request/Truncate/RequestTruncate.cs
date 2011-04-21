using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB
{
    public sealed class RequestTruncate : IRequest
    {
        #region data

        public String TypeName;

        #endregion

        #region Constructor

        public RequestTruncate(String myTypeName)
        {
            TypeName = myTypeName;
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
