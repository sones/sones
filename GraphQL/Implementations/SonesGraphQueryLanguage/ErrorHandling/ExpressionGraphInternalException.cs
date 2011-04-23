using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using System.Diagnostics;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class ExpressionGraphInternalException : AGraphQLException
    {
        public String Info { get; private set; }

        public ExpressionGraphInternalException(String myInfo)
        {
            Info = myInfo;
            _msg = String.Format("An internal ExpressionGraph error occurred: \"{0}\"\nStacktrace:\n{1}", Info, StackTrace);
        }
    }
}
