using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.CLIrony.Compiler;

namespace sones.Lib.Frameworks.CLIrony.EditorServices
{

    public static class EditorUtilities
    {
        public static TokenList GetOpeningBraces(TokenList tokens)
        {
            TokenList braces = new TokenList();
            foreach (Token token in tokens)
                if (token.Term.IsSet(TermOptions.IsOpenBrace) && token.OtherBrace != null)
                    braces.Add(token);
            return braces;
        }
    }

}//namespace
