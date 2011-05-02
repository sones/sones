using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.ErrorHandling
{
    public sealed class NotADumpableGrammarException : AGraphQLException
    {
        #region data

        public String Info { get; private set; }
        public String Grammar { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new NotADumpableGrammarException exception
        /// </summary>
        public NotADumpableGrammarException(String myGrammar, String myInfo)
        {
            Info = myInfo;
            Grammar = myGrammar;

            _msg = String.Format("Error the given Grammar: [{0}] is not dumpable.\n\n{1}", Grammar, myInfo);
        }

        #endregion
    }
}
