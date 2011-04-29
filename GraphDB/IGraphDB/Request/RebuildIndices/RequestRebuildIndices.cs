using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.RebuildIndices
{
    public sealed class RequestRebuildIndices : IRequest
    {
        #region data

        /// <summary>
        /// The types that should be rebuild
        /// </summary>
        public IEnumerable<String> Types;

        #endregion

        #region constructor

        public RequestRebuildIndices(IEnumerable<String> myTypes)
        {
            Types = myTypes;
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
