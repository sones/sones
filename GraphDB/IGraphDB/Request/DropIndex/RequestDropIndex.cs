using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    public sealed class RequestDropIndex : IRequest
    {
        #region data

        /// <summary>
        /// Name of the Type on which the index is created
        /// </summary>
        public String TypeName;
        /// <summary>
        /// Name of the index
        /// </summary>
        public String IndexName;
        /// <summary>
        /// Edition of the index
        /// </summary>
        public String Edition;

        #endregion

        #region constructor

        public RequestDropIndex(String myTypeName, String myIndexName, String myEdition)
        {
            TypeName = myTypeName;
            IndexName = myIndexName;
            Edition = myEdition;
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
