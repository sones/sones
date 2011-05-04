using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    public sealed class RequestUpdate : IRequest
    {

        #region data

        /// <summary>
        /// The interesting type on which the index is created
        /// </summary>
        public readonly String TypeName;

        #endregion

        #region constructor

        public RequestUpdate(String myTypeName)
        {
            TypeName = myTypeName;
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
