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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// This class hold all the data that comes out of the database after a query is run
    /// </summary>
    public sealed class QueryResult : IQueryResult
    {
        #region Creators

        public static QueryResult Success(String myQuery, String myQLName, IEnumerable<IVertexView> myVertices, UInt64 myDuration = 0UL)
        {
            return new QueryResult(myQuery, myQLName, myDuration, ResultType.Successful, myVertices, null);
        }

        public static QueryResult Failure(String myQuery, String myQLName, ASonesException myError, IEnumerable<IVertexView> myVertices = null, UInt64 myDuration = 0UL)
        {
            return new QueryResult(myQuery, myQLName, myDuration, ResultType.Failed, myVertices, myError);
        }

        #endregion

        #region Data

        /// <summary>
        /// An error that occured during the query process
        /// </summary>
        public ASonesException Error { get; private set; }

        /// <summary>
        /// The vertices that are contained in this IQueryResult
        /// </summary>
        public IEnumerable<IVertexView> Vertices { get; private set; }

        /// <summary>
        /// The query that has been executed
        /// </summary>
        public String Query { get; private set; }

        /// <summary>
        /// The name of the query language that has been executed
        /// </summary>
        public String NameOfQuerylanguage { get; private set; }

        /// <summary>
        /// The time that was spent on executing the query
        /// </summary>
        public UInt64 Duration { get; private set; }

        /// <summary>
        /// The ReasultType of the executed query
        /// </summary>
        public ResultType TypeOfResult { get; private set; }

        /// <summary>
        /// The number of affected vertices
        /// </summary>
        public UInt64 NumberOfAffectedVertices
        {
            get
            {
                var numberOfAffectedVertices = 0UL;

                if (Vertices != null)
                    numberOfAffectedVertices = (UInt64)Vertices.Count();

                return numberOfAffectedVertices;
            }
        }

        #endregion

        #region Constructor(s)
        
        /// <summary>
        /// Creates a new query result
        /// </summary>
        /// <param name="myQuery">The query that has been executed</param>
        /// <param name="myQLName">The name of the query language that has been executed</param>
        /// <param name="myDuration">The time that was spent on executing the query</param>
        /// <param name="myVertices">The vertices that should be available within the query result</param>
        /// <param name="myError">The error which occured during execution</param>
        public QueryResult(String myQuery, 
                            String myQLName, 
                            UInt64 myDuration, 
                            ResultType myResultType, 
                            IEnumerable<IVertexView> myVertices, 
                            ASonesException myError)
        {
            TypeOfResult = myResultType;
            Vertices = myVertices ?? new List<IVertexView>();
            Query = myQuery;
            NameOfQuerylanguage = myQLName;
            Duration = myDuration;
            Error = myError;
        }

        #endregion

        #region ToString()

        #region ToString()

        public override String ToString()
        {
            return String.Format("Query: {0}{1}Count of affected vertices: {2}", Query, Environment.NewLine, NumberOfAffectedVertices);
        }

        #endregion

        #endregion
    }
}