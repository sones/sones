using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    public sealed class RequestDropType : IRequest
    {
        #region data

        /// <summary>
        /// The name of the type
        /// </summary>
        public String TypeName;

        #endregion
        
        #region constructor

        public RequestDropType(String myTypeName)
        {
            TypeName = myTypeName;
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
