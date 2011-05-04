using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace sones.GraphDB.Request
{
    public sealed class RequestDelete : IRequest
    {
        #region Data

        public readonly RequestGetVertices  GetVerticesRequest;
        public readonly IEnumerable<String> ToBeDeletedAttributes;

        #endregion

        #region Constructor

        public RequestDelete(RequestGetVertices myGetVerticesRequest, IEnumerable<String> myToBeDeletedAttributes = null)
        {
            GetVerticesRequest = myGetVerticesRequest;

            ToBeDeletedAttributes = myToBeDeletedAttributes;
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
