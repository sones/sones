using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.ErrorHandling
{
    public class InvalidEdgeTypeException : AGraphQLEdgeException
    {
        public Type[] ExpectedEdgeTypes { get; private set; }
        public Type CurrentEdgeType { get; private set; }

        public InvalidEdgeTypeException(params Type[] myExpectedEdgeTypes)
        {
            ExpectedEdgeTypes = myExpectedEdgeTypes;
            _msg = String.Format("Invalid edge type! Use one of the following: {0}", ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));

        }

        public InvalidEdgeTypeException(Type myCurrentEdgeType, params Type[] myExpectedEdgeTypes)
        {
            CurrentEdgeType = myCurrentEdgeType;
            ExpectedEdgeTypes = myExpectedEdgeTypes;
            _msg = String.Format("The edge type \"{0}\" does not match the expected type: {1}", CurrentEdgeType,
                   ExpectedEdgeTypes.Aggregate<Type, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }

    }
}
