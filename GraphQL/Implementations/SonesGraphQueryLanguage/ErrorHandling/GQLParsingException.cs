using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using Irony.Parsing;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class GQLParsingException : AGraphQLException
    {
        #region data
        
        public String Info { get; private set; }
        public Exception Exception { get; private set; }
        public ParserMessage ParserError { get; private set; }

        public String ParserErrorMessage
        {
            get
            {
                return (ParserError != null) ? ParserError.Message : "";
            }
        }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new GQLParsingException exception
        /// </summary>
        /// <param name="myParserError">The parser message from Irony (contains a message from kind of info, warning or error)</param>
        /// <param name="myQuery">The given query</param>
        public GQLParsingException(ParserMessage myParserError, String myQuery)
        {
            ParserError = myParserError; 
            Info = myQuery;

            if (ParserError != null)
            {
                if (ParserError.Exception != null)
                    _msg = String.Format("Parser error during query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}\n\nException:{3}", Info, ParserError.Message, ParserError.Location.ToString(), ParserError.Exception.ToString());
                else
                    _msg = String.Format("Parser error during query: [{0}]\n\n gql: [{1}]\n\nAt position: {2}", Info, ParserError.Message, ParserError.Location.ToString());
            }
        }

        /// <summary>
        /// Creates a new GQLParsingException exception
        /// </summary>
        /// <param name="myInfo">An information</param>
        /// <param name="myException">The occurred exception</param>
        public GQLParsingException(String myInfo, Exception myException)
        {
            Info = myInfo;
            Exception = myException;

            if (myException != null)
                _msg = Info + Environment.NewLine + Exception.Message;
        }

        #endregion
    }
}
