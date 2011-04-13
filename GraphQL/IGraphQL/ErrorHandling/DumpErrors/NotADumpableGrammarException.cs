using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The grammar is not dumpable
    /// </summary>
    public sealed class NotADumpableGrammarException : AGraphQLDumpException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Create a new NotADumpableGrammarException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public NotADumpableGrammarException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }
        
    }
}
