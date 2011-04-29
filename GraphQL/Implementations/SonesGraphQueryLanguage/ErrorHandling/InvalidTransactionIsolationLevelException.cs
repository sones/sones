using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class InvalidTransactionIsolationLevelException : AGraphQLException
    {
        public String Isolation;
        public String Info;

        public InvalidTransactionIsolationLevelException(String myIsolationLevel, String myInfo)
        {
            Isolation = myIsolationLevel;
            Info = myInfo;

            _msg = String.Format("Given transaction isolation level is invalid: [{0}]\n\n{1}", Isolation, Info);
        }
    }
}
