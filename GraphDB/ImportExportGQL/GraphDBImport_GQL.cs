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
using sones.GraphDB.Result;
using System.Diagnostics;
using sones.GraphDB.Errors;
using sones.GraphDB.Warnings;
using System.Threading;
using sones.GraphDB.NewAPI;
using System.IO;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement.BasicTypes;

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

        public override QueryResult Import(System.IO.Stream myInputStream, IGraphDBSession myIGraphDBSession, DBContext myDBContext, UInt32 myParallelTasks = 1, IEnumerable<String> myComments = null, ulong? myOffset = null, ulong? myLimit = null, VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            var gqlQuery = new GraphQLQuery(myDBContext.DBPluginManager);

            var lines = ReadLinesFromStream(myInputStream);

            #region Evaluate Limit and Offset

            if (myOffset != null)
            {
                lines = lines.SkipULong(myOffset.Value);
            }
            if (myLimit != null)
            {
                lines = lines.TakeULong(myLimit.Value);
            }

            #endregion

            var queryResult = new QueryResult();

            #region Import queries

            if (myParallelTasks > 1)
            {
                queryResult = ExecuteAsParallel(lines, myIGraphDBSession, gqlQuery, myVerbosityType, myParallelTasks, myComments);
            }
            else
            {
                queryResult = ExecuteAsSingleThread(lines, myIGraphDBSession, gqlQuery, myVerbosityType, myComments);
            }

            #endregion

            return queryResult;

        }

        private QueryResult ExecuteAsParallel(IEnumerable<String> myLines, IGraphDBSession myIGraphDBSession, GraphQLQuery myGQLQuery, VerbosityTypes verbosityTypes, UInt32 parallelTasks = 1, IEnumerable<String> comments = null)
        {

            var queryResult       = new QueryResult();
            var aggregatedResults = new List<IEnumerable<IVertex>>();

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

                    Interlocked.Add(ref numberOfLine, 1L);

                    if (!IsComment(line, comments)) // Skip comments
                    {

                        var qresult = ExecuteQuery(line, myIGraphDBSession, myGQLQuery);

                        #region VerbosityTypes.Full: Add result

                        if (verbosityTypes == VerbosityTypes.Full)
                        {
                            lock (aggregatedResults)
                            {
                                aggregatedResults.Add(qresult.Vertices);
                            }
                        }

                        #endregion

                        #region !VerbosityTypes.Silent: Add errors and break execution

                        if (qresult.ResultType != ResultType.Successful && verbosityTypes != VerbosityTypes.Silent)
                        {
                            lock (queryResult)
                            {
                                queryResult.PushIErrors(new[] { new Errors.Error_ImportFailed(line, numberOfLine) });
                                queryResult.PushIErrors(qresult.Errors);
                                queryResult.PushIWarnings(qresult.Warnings);
                            }
                            state.Break();
                        }

                        #endregion
                    
                    }

                }

            });


            //add the results of each query into the queryResult
            queryResult.Vertices = AggregateListOfListOfVertices(aggregatedResults);

            return queryResult;

        }

        private QueryResult ExecuteAsSingleThread(IEnumerable<String> myLines, IGraphDBSession myIGraphDBSession, GraphQLQuery myGQLQuery, VerbosityTypes verbosityTypes, IEnumerable<String> comments = null)
        {

            var queryResult1 = new QueryResult();
            Int64 numberOfLine = 0;
            var query = String.Empty;
            var aggregatedResults = new List<IEnumerable<IVertex>>();

            foreach (var _Line in myLines)
            {

                numberOfLine++;

                if (String.IsNullOrWhiteSpace(_Line))
                {
                    continue;
                }

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
                    aggregatedResults.Add(qresult.Vertices);
                }

                #endregion

                #region !VerbosityTypes.Silent: Add errors and break execution

                if (qresult.ResultType == ResultType.Failed)
                {

                    if (qresult.Errors.Any(e => (e is Error_GqlSyntax) && (e as Error_GqlSyntax).SyntaxErrorMessage.Equals("Mal-formed  string literal - cannot find termination symbol.")))
                    {
                        Debug.WriteLine("Query at line [" + numberOfLine + "] [" + query + "] failed with " + qresult.GetIErrorsAsString() + " add next line...");
                        continue;
                    }
                    
                    if (verbosityTypes != VerbosityTypes.Silent)
                    {
                        queryResult1.PushIError(new Error_ImportFailed(query, numberOfLine));
                        queryResult1.PushIErrors(qresult.Errors);
                        queryResult1.PushIWarnings(qresult.Warnings);
                    }

                    break;

                }
                else if (qresult.ResultType == ResultType.PartialSuccessful && verbosityTypes != VerbosityTypes.Silent)
                {
                    queryResult1.PushIWarning(new Warning_ImportWarning(query, numberOfLine));
                    queryResult1.PushIWarnings(qresult.Warnings);
                }

                #endregion

                query = String.Empty;

            }

            //add the results of each query into the queryResult
            queryResult1.Vertices = AggregateListOfListOfVertices(aggregatedResults);

            return queryResult1;

        }

        /// <summary>
        /// Aggregates different enumerations of readout objects
        /// </summary>
        /// <param name="myListOfListOfVertices"></param>
        /// <returns></returns>
        private IEnumerable<IVertex> AggregateListOfListOfVertices(List<IEnumerable<IVertex>> myListOfListOfVertices)
        {

            foreach (var _ListOfVertices in myListOfListOfVertices)
            {
                foreach (var _Vertex in _ListOfVertices)
                {
                    yield return _Vertex;
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
