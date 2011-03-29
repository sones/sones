using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.ErrorHandling
{
    public abstract class AGraphQLEdgeException : AGraphQLException
    {
        public override ushort ErrorCode
        {
            get { throw new NotImplementedException(); }
        }
    }
}
