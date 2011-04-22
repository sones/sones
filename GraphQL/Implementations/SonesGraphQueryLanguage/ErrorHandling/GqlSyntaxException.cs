using System;
using Irony.Parsing;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A GQL syntax error has occurred
    /// </summary>
    public class GqlSyntaxException : AGraphQLException
    {

        #region data
        
        public String Info { get; private set; }
        public Exception Exception { get; private set; }
        public ParserMessage SyntaxError { get; private set; }
        public String SyntaxErrorMessage
        {
            get
            {
                return (SyntaxError != null) ? SyntaxError.Message : "";
            }
        }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new GqlSyntaxException exception
        /// </summary>
        /// <param name="mySyntaxError">The parser message from Irony (contains a message from kind of info, warning or error)</param>
        /// <param name="myQuery">The given query</param>
        public GqlSyntaxException(ParserMessage mySyntaxError, String myQuery)
        {
            SyntaxError = mySyntaxError; 
            Info = myQuery;

            if (SyntaxError != null)
            {
                if (SyntaxError.Exception != null)
                    _msg = String.Format("Syntax error in query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}\n\nException:{3}", Info, SyntaxError.Message, SyntaxError.Location.ToString(), SyntaxError.Exception.ToString());
                else
                    _msg = String.Format("Syntax error in query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}", Info, SyntaxError.Message, SyntaxError.Location.ToString());
            }
        }

        /// <summary>
        /// Creates a new GqlSyntaxException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public GqlSyntaxException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }

        /// <summary>
        /// Creates a new GqlSyntaxException exception
        /// </summary>
        /// <param name="myInfo">An information</param>
        /// <param name="myException">The occurred exception</param>
        public GqlSyntaxException(String myInfo, Exception myException)
        {
            Info = myInfo;
            Exception = myException;

            if (myException != null)
                _msg = Info + Environment.NewLine + Exception.Message;
        }

        #endregion

    }

}

