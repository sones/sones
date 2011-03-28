using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    public abstract class AGraphQLAttributeAssignmentException : AGraphQLException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
