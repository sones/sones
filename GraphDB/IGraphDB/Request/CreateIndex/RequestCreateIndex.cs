using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request.CreateVertexTypes;

namespace sones.GraphDB.Request.CreateIndex
{
    public sealed class RequestCreateIndex : IRequest
    {
        #region data

        /// <summary>
        /// Name of the type on which the index is created
        /// </summary>
        public IndexPredefinition IndexDefinition;

        #endregion

        #region constructor

        public RequestCreateIndex(IndexPredefinition myIndexDefinition)
        {
            IndexDefinition = myIndexDefinition;
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
