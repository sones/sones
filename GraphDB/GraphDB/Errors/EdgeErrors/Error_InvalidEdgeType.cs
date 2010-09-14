using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidEdgeType : GraphDBEdgeError
    {
        public Type[] ExpectedEdgeTypes { get; private set; }
        public Type CurrentEdgeType { get; private set; }

        public Error_InvalidEdgeType(params Type[] myExpectedEdgeTypes)
        {
            ExpectedEdgeTypes = myExpectedEdgeTypes;
        }

        public Error_InvalidEdgeType(Type myCurrentEdgeType, params Type[] myExpectedEdgeTypes)
        {
            CurrentEdgeType = myCurrentEdgeType;
            ExpectedEdgeTypes = myExpectedEdgeTypes;
        }

        public override string ToString()
        {
            if (CurrentEdgeType != null)
                return String.Format("The edge type \"{0}\" does not match the expected type: {1}", CurrentEdgeType,
                    ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
            else
                return String.Format("Invalid edge type! Use one of the following: {0}", ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }
    }
}
