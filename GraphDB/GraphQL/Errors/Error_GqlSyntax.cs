
#region Usings

using System;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.Errors
{

    public class Error_GqlSyntax : GraphDBError
    {

        public String Info { get; private set; }
        public Exception Exception { get; private set; }
        public SyntaxError SyntaxError { get; private set; }
        public String SyntaxErrorMessage
        {
            get
            {
                return (SyntaxError != null)?SyntaxError.Message:"";
            }
        }

        public Error_GqlSyntax(SyntaxError mySyntaxError, String myQuery)
        {
            SyntaxError = mySyntaxError;
            Info = myQuery;
        }

        public Error_GqlSyntax(String myInfo)
        {
            Info = myInfo;
        }

        public Error_GqlSyntax(String myInfo, Exception myException)
        {
            Info = myInfo;
            Exception = myException;
        }

        public override string ToString()
        {

            if (SyntaxError != null)
            {
                if (SyntaxError.Exception != null)
                    return String.Format("Syntax error in query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}\n\nException:{3}", Info, SyntaxError.Message, SyntaxError.Location.ToString(), SyntaxError.Exception.ToString());
                else
                    return String.Format("Syntax error in query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}", Info, SyntaxError.Message, SyntaxError.Location.ToString());
            }

            else if (Exception != null)
            {
                return Info + Environment.NewLine + Exception.Message;
            }

            else
            {
                return Info;
            }

        }
        
    }

}
