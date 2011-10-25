/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
        
        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new GqlSyntaxException exception
        /// </summary>
        /// <param name="mySyntaxError">The parser message from Irony (contains a message from kind of info, warning or error)</param>
        /// <param name="myQuery">The given query</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public GQLStatementNodeExecutionException(String myQuery, AStatement myStatement, String myInfo, Exception innerException = null) : base(innerException)
        {
            Query = myQuery;
            Statement = myStatement;
            Info = myInfo;
            
            if(InnerException != null)
                _msg = String.Format("Error during execute statement: [{0}]\n\n in query: [{1}]\n\n{2}", Statement.StatementName, Query, Info);
            else
                _msg = String.Format("Error during execute statement: [{0}]\n\n in query: [{1}]\n\n{2}\n\n{3}", Statement.StatementName, Query, Info, InnerException.Message);
        }

        #endregion
    }
}
