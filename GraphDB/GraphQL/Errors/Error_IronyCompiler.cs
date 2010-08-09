
#region Usings

using System;
using System.Linq;
using System.Text;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.Errors
{

    public class Error_IronyCompiler : GraphDBError
    {

        public GrammarErrorList GrammarErrorList { get; private set; }

        public Error_IronyCompiler(GrammarErrorList myGrammarErrorList)
        {
            GrammarErrorList = myGrammarErrorList;
        }

        public override string ToString()
        {
            return String.Format("Invalid grammar: {0}",
                    GrammarErrorList.Aggregate<GrammarError, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem.Message); return result; }));
        }

    }

}
