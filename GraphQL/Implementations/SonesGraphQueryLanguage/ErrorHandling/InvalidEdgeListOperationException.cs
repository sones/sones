using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.GraphQL.GQL.Manager.Select;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class InvalidEdgeListOperationException : AGraphQLException
    {
        private EdgeList _EdgeList1;
        private EdgeList _EdgeList2;
        private EdgeKey _EdgeKey;
        private string _Operation;

        public InvalidEdgeListOperationException(EdgeList myEdgeList, EdgeKey myEdgeKey, string myOperation)
        {
            _EdgeList1 = myEdgeList;
            this._EdgeKey = myEdgeKey;
            this._Operation = myOperation;
        }

        public InvalidEdgeListOperationException(EdgeList myEdgeList1, EdgeList myEdgeList2, string myOperation)
        {
            _EdgeList1 = myEdgeList1;
            _EdgeList2 = myEdgeList2;
            this._Operation = myOperation;
        }

        public override string ToString()
        {
            if (_EdgeKey != null)
            {
                return String.Format("EdgeList operation [{0}] is not valid for [{1}] and [{2}] ", _Operation, _EdgeList1, _EdgeKey);
            }
            else
            {
                return String.Format("EdgeList operation [{0}] is not valid for [{1}] and [{2}] ", _Operation, _EdgeList1, _EdgeList2);
            }
        }
    }
}
