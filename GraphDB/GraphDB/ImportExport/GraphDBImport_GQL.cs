/* 
 * GraphDBImport_GQL
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.Lib;
using System.Threading.Tasks;
using sones.GraphDB.QueryLanguage.Result;
using System.Collections.Concurrent;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Import;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements;

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

        public override QueryResult Import(IEnumerable<string> lines, IGraphDBSession graphDBSession, uint parallelTasks = 1, IEnumerable<string> comments = null, ulong? offset = null, ulong? limit = null, VerbosityTypes verbosityType = VerbosityTypes.Errors)
        {

            #region Evaluate Limit and Offset

            if (offset != null)
            {
                lines = lines.SkipULong(offset.Value);
            }
            if (limit != null)
            {
                lines = lines.TakeULong(limit.Value);
            }

            #endregion

            var queryResult = new QueryResult();

            #region Import queries

            if (parallelTasks > 1)
            {
                queryResult = ExecuteAsParallel(lines, graphDBSession, verbosityType, parallelTasks, comments);
            }
            else
            {
                queryResult = ExecuteAsSingleThread(lines, graphDBSession, verbosityType, comments);
            }

            #endregion

            return queryResult;
        }


        private QueryResult ExecuteAsParallel(IEnumerable<string> lines, IGraphDBSession graphDBSession, VerbosityTypes verbosityType, uint parallelTasks = 1, IEnumerable<string> comments = null)
        {

            var queryResult = new QueryResult();

            #region Create parallel options

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = (int)parallelTasks
            };

            #endregion

            Int64 numberOfLine = 0;

            Parallel.ForEach(lines, parallelOptions, (line, state) =>
            {

                if (!IsComment(line, comments))
                {

                    System.Threading.Interlocked.Add(ref numberOfLine, 1L);

                    if (!IsComment(line, comments)) // Skip comments
                    {

                        var qresult = ExecuteQuery(line, graphDBSession);

                        #region VerbosityTypes.Full: Add result

                        if (verbosityType == VerbosityTypes.Full)
                        {
                            lock (queryResult)
                            {
                                foreach (var resultSet in qresult.Results)
                                {
                                    queryResult.AddResult(resultSet);
                                }
                            }
                        }

                        #endregion

                        #region !VerbosityTypes.Silent: Add errors and break execution

                        if (qresult.ResultType != Structures.ResultType.Successful && verbosityType != VerbosityTypes.Silent)
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

            return queryResult;

        }

        private QueryResult ExecuteAsSingleThread(IEnumerable<string> lines, IGraphDBSession graphDBSession, VerbosityTypes verbosityType, IEnumerable<string> comments = null)
        {

            var queryResult = new QueryResult();
            Int64 numberOfLine = 0;
            String query = String.Empty;
            foreach (var line in lines)
            {

                numberOfLine++;

                #region Skip comments

                if (IsComment(line, comments))
                {
                    continue;
                }

                #endregion

                query += line;

                var qresult = ExecuteQuery(query, graphDBSession);

                #region VerbosityTypes.Full: Add result

                if (verbosityType == VerbosityTypes.Full)
                {
                    foreach (var resultSet in qresult.Results)
                    {
                        queryResult.AddResult(resultSet);
                    }
                }

                #endregion

                #region !VerbosityTypes.Silent: Add errors and break execution

                if (qresult.ResultType == Structures.ResultType.Failed)
                {

                    if (qresult.Errors.Any(e => (e is Errors.Error_GqlSyntax) && (e as Errors.Error_GqlSyntax).SyntaxError.Message.Equals("Mal-formed  string literal - cannot find termination symbol.")))
                    {
                        System.Diagnostics.Debug.WriteLine("Query at line [" + numberOfLine + "] [" + query + "] failed with " + qresult.GetErrorsAsString() + " add next line...");
                        continue;
                    }
                    
                    if (verbosityType != VerbosityTypes.Silent)
                    {
                        queryResult.AddErrors(new[] { new Errors.Error_ImportFailed(query, numberOfLine) });
                        queryResult.AddErrors(qresult.Errors);
                        queryResult.AddWarnings(qresult.Warnings);
                    }

                    break;
                }
                else if (qresult.ResultType == Structures.ResultType.PartialSuccessful && verbosityType != VerbosityTypes.Silent)
                {

                    queryResult.AddWarning(new Warnings.Warning_ImportWarning(query, numberOfLine));
                    queryResult.AddWarnings(qresult.Warnings);

                }

                #endregion

                query = String.Empty;

            }

            return queryResult;

        }

        private Boolean IsComment(String query, IEnumerable<string> comments = null)
        {
            if (comments.IsNullOrEmpty())
            {
                return false;
            }

            return comments.Any(c => query.StartsWith(c));
        }

        private QueryResult ExecuteQuery(String query, IGraphDBSession graphDBSession)
        {

            return graphDBSession.Query(query);
        }

    }
}
