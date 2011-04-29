using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.StatementNodes;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class GQLStatementNodeExecutionException : AGraphQLException
    {
        #region data
        
        public String Info { get; private set; }
        public AStatement Statement { get; private set; }
        public String Query { get; private set; }
        ASonesException InnerException;

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new GqlSyntaxException exception
        /// </summary>
        /// <param name="mySyntaxError">The parser message from Irony (contains a message from kind of info, warning or error)</param>
        /// <param name="myQuery">The given query</param>
        public GQLStatementNodeExecutionException(String myQuery, AStatement myStatement, String myInfo, ASonesException myInnerException = null)
        {
            Query = myQuery;
            Statement = myStatement;
            Info = myInfo;
            InnerException = myInnerException;

            if(InnerException != null)
                _msg = String.Format("Error during execute statement: [{0}]\n\n in query: [{1}]\n\n{2}", Statement.StatementName, Query, Info);
            else
                _msg = String.Format("Error during execute statement: [{0}]\n\n in query: [{1}]\n\n{2}\n\n{3}", Statement.StatementName, Query, Info, InnerException.Message);
        }

        #endregion
    }
}
