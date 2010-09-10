/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* 
 * GraphDBImport_GQL
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using sones.GraphDB.GraphQL;


using sones.Lib;
using sones.GraphDBInterface.Result;

#endregion

namespace sones.GraphDB.ImportExport
{

    /// <summary>
    /// An import implementation for GQL
    /// </summary>
    public class GraphDBImport_GQL : AGraphDBImport
    {

        public override string ImportFormat
        {
            get { return "GQL"; }
        }

        public override QueryResult Import(IEnumerable<String> myLines, IGraphDBSession myIGraphDBSession, DBContext myDBContext, UInt32 parallelTasks = 1, IEnumerable<String> comments = null, ulong? offset = null, ulong? limit = null, VerbosityTypes verbosityTypes = VerbosityTypes.Errors)
        {

            var gqlQuery = new GraphQLQuery(myDBContext.DBPluginManager);

            #region Evaluate Limit and Offset

            if (offset != null)
            {
                myLines = myLines.SkipULong(offset.Value);
            }
            if (limit != null)
            {
                myLines = myLines.TakeULong(limit.Value);
            }

            #endregion

            var queryResult = new QueryResult();

            #region Import queries

            if (parallelTasks > 1)
            {
                queryResult = ExecuteAsParallel(myLines, myIGraphDBSession, gqlQuery, verbosityTypes, parallelTasks, comments);
            }
            else
            {
                queryResult = ExecuteAsSingleThread(myLines, myIGraphDBSession, gqlQuery, verbosityTypes, comments);
            }

            #endregion

            return queryResult;

        }


        private QueryResult ExecuteAsParallel(IEnumerable<String> myLines, IGraphDBSession myIGraphDBSession, GraphQLQuery myGQLQuery, VerbosityTypes verbosityTypes, UInt32 parallelTasks = 1, IEnumerable<String> comments = null)
        {

            var queryResult = new QueryResult();
            List<IEnumerable<DBObjectReadout>> aggregatedResults = new List<IEnumerable<DBObjectReadout>>();

            #region Create parallel options

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = (int)parallelTasks
            };

            #endregion

            Int64 numberOfLine = 0;

            Parallel.ForEach(myLines, parallelOptions, (line, state) =>
            {

                if (!IsComment(line, comments))
                {

                    System.Threading.Interlocked.Add(ref numberOfLine, 1L);

                    if (!IsComment(line, comments)) // Skip comments
                    {

                        var qresult = ExecuteQuery(line, myIGraphDBSession, myGQLQuery);

                        #region VerbosityTypes.Full: Add result

                        if (verbosityTypes == VerbosityTypes.Full)
                        {
                            lock (aggregatedResults)
                            {
                                aggregatedResults.Add(qresult.Results.Objects);
                            }
                        }

                        #endregion

                        #region !VerbosityTypes.Silent: Add errors and break execution

                        if (qresult.ResultType != ResultType.Successful && verbosityTypes != VerbosityTypes.Silent)
                        {
                            lock (queryResult)
                            {
                                queryResult.AddErrors(new[] { new Errors.Error_ImportFailed(line, numberOfLine) });
                                queryResult.AddErrors(qresult.Errors);
                                queryResult.AddWarnings(qresult.Warnings);
                            }
                            state.Break();
                        }

                        #endregion
                    
                    }

                }

            });


            //add the results of each query into the queryResult
            queryResult.SetResult(new SelectionResultSet(AggregateListOfReadouts(aggregatedResults)));

            return queryResult;

        }

        private QueryResult ExecuteAsSingleThread(IEnumerable<String> myLines, IGraphDBSession myIGraphDBSession, GraphQLQuery myGQLQuery, VerbosityTypes verbosityTypes, IEnumerable<String> comments = null)
        {

            var queryResult = new QueryResult();
            Int64 numberOfLine = 0;
            String query = String.Empty;
            List<IEnumerable<DBObjectReadout>> aggregatedResults = new List<IEnumerable<DBObjectReadout>>();

            foreach (var _Line in myLines)
            {

                numberOfLine++;

                #region Skip comments

                if (IsComment(_Line, comments))
                {
                    continue;
                }

                #endregion

                query += _Line;

                var qresult = ExecuteQuery(query, myIGraphDBSession, myGQLQuery);

                #region VerbosityTypes.Full: Add result

                if (verbosityTypes == VerbosityTypes.Full)
                {
                    aggregatedResults.Add(qresult.Results.Objects);
                }

                #endregion

                #region !VerbosityTypes.Silent: Add errors and break execution

                if (qresult.ResultType == ResultType.Failed)
                {

                    if (qresult.Errors.Any(e => (e is Errors.Error_GqlSyntax) && (e as Errors.Error_GqlSyntax).SyntaxErrorMessage.Equals("Mal-formed  string literal - cannot find termination symbol.")))
                    {
                        System.Diagnostics.Debug.WriteLine("Query at line [" + numberOfLine + "] [" + query + "] failed with " + qresult.GetErrorsAsString() + " add next line...");
                        continue;
                    }
                    
                    if (verbosityTypes != VerbosityTypes.Silent)
                    {
                        queryResult.AddErrors(new[] { new Errors.Error_ImportFailed(query, numberOfLine) });
                        queryResult.AddErrors(qresult.Errors);
                        queryResult.AddWarnings(qresult.Warnings);
                    }

                    break;
                }
                else if (qresult.ResultType == ResultType.PartialSuccessful && verbosityTypes != VerbosityTypes.Silent)
                {

                    queryResult.AddWarning(new Warnings.Warning_ImportWarning(query, numberOfLine));
                    queryResult.AddWarnings(qresult.Warnings);

                }

                #endregion

                query = String.Empty;

            }

            //add the results of each query into the queryResult
            queryResult.SetResult(new SelectionResultSet(AggregateListOfReadouts(aggregatedResults)));

            return queryResult;

        }

        /// <summary>
        /// Aggregates different enumerations of readout objects
        /// </summary>
        /// <param name="listOfReadouts"></param>
        /// <returns></returns>
        private IEnumerable<DBObjectReadout> AggregateListOfReadouts(List<IEnumerable<DBObjectReadout>> listOfReadouts)
        {
            foreach (var aReadoutEnumerable in listOfReadouts)
            {
                foreach (var aDBReadout in aReadoutEnumerable)
                {
                    yield return aDBReadout;
                }
            }

            yield break;
        }

        private Boolean IsComment(String myQuery, IEnumerable<String> comments = null)
        {

            if (comments.IsNullOrEmpty())
                return false;

            return comments.Any(c => myQuery.StartsWith(c));

        }

        private QueryResult ExecuteQuery(String myQuery, IGraphDBSession myIGraphDBSession, GraphQLQuery myGQLQuery)
        {
            return myGQLQuery.Query(myQuery, myIGraphDBSession);
        }

    }

}
