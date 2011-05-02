using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace sones.GraphDB.Request.Delete
{
    public sealed class RequestDelete : IRequest
    {
        #region Data

        public readonly String              TypeName;
        public readonly IEnumerable<Int64>  ToBeDeletedVertices;
        public readonly IEnumerable<String> ToBeDeletedAttributes;
        public readonly IExpression         ToBeDeletedExpression;

        #endregion

        #region Constructor

        public RequestDelete(String myTypeName, IEnumerable<Int64> myToBeDeletedVertices, IEnumerable<String> myToBeDeletedAttributes = null)
        {
            TypeName = myTypeName;
            ToBeDeletedAttributes = myToBeDeletedAttributes;
            ToBeDeletedExpression = null;
        }

        public RequestDelete(IExpression myToBeDeletedExpression, IEnumerable<String> myToBeDeletedAttributes = null)
        {
            ToBeDeletedAttributes = myToBeDeletedAttributes;
            TypeName = null;
            ToBeDeletedVertices = null;
            ToBeDeletedExpression = myToBeDeletedExpression;
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
