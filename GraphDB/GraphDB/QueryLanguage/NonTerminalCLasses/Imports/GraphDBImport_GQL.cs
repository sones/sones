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

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Imports
{
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

            Parallel.ForEach(lines, parallelOptions, (line, state) =>
            {

                if (!IsComment(line, comments))
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
                            queryResult.AddErrors(qresult.Errors);
                            queryResult.AddWarnings(qresult.Warnings);
                        }
                        state.Break();
                    }

                    #endregion

                }

            });

            return queryResult;

        }

        private QueryResult ExecuteAsSingleThread(IEnumerable<string> lines, IGraphDBSession graphDBSession, VerbosityTypes verbosityType, IEnumerable<string> comments = null)
        {

            var queryResult = new QueryResult();

            foreach (var line in lines)
            {

                #region Skip comments

                if (IsComment(line, comments))
                {
                    continue;
                }

                #endregion

                var qresult = ExecuteQuery(line, graphDBSession);

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

                if (qresult.ResultType != Structures.ResultType.Successful && verbosityType != VerbosityTypes.Silent)
                {
                    queryResult.AddErrors(qresult.Errors);
                    queryResult.AddWarnings(qresult.Warnings);
                    break;
                }

                #endregion

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
