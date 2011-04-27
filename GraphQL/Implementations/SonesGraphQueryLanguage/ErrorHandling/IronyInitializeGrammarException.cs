using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using Irony.Parsing;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class IronyInitializeGrammarException : AGraphQLException
    {
        public String Info { get; private set; }

        private GrammarErrorList Errors;

        public IronyInitializeGrammarException(GrammarErrorList myErrors, String myInfo)
        {
            Info = myInfo;

            Errors = myErrors;

            StringBuilder msg = new StringBuilder();

            msg.AppendLine("An error occurred during initializing the grammar: ");
            
            foreach (var error in myErrors)
            {
                msg.AppendLine(String.Format("{0} {1} {2}", error.Level.ToString(), error.Message, error.State.Name));
            }

            msg.AppendLine(Info);

            _msg = msg.ToString();
        }
    }
}
