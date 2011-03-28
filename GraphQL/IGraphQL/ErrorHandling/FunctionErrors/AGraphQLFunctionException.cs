using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    public abstract class AGraphQLFunctionException : AGraphQLException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
