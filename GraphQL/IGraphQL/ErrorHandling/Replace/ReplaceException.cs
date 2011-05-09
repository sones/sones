using System;

namespace sones.GraphQL.ErrorHandling
{
    public sealed class ReplaceException : AGraphQLException
    {
        public ReplaceException(String myInfo)
        {
            _msg = myInfo;
        }
               
    }
}
