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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using sones.GraphDB;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.DBImport.ErrorHandling;
using sones.Library.DataStructures;
using sones.Plugins.SonesGQL.DBImport;
using System.Threading.Tasks;
using System.Threading;

namespace sones.Plugins.SonesGQL
{
    public sealed class GraphDBImport_GQL : IGraphDBImport, IPluginable
    {
        #region constructor

        public GraphDBImport_GQL()
        { }

        #endregion

        #region IGraphDBImport Members

        public string ImportFormat
        {
            get { return "GQL"; }
        }

        public string ImporterName
        {
            get { return "GQLIMPORT"; }
        }

        public QueryResult Import(String myLocation, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, UInt32 myParallelTasks = 1U, IEnumerable<string> myComments = null, UInt64? myOffset = null, UInt64? myLimit = null, VerbosityTypes myVerbosityType = VerbosityTypes.Silent)
        {
            ASonesException error;
            Stream stream = null;
            QueryResult result;

            #region Read querie lines from location

            try
            {
                #region file
                if (myLocation.ToLower().StartsWith(@"file:\\"))
                {
                    //lines = ReadFile(location.Substring(@"file:\\".Length));
                    stream = GetStreamFromFile(myLocation.Substring(@"file:\\".Length));
                } 
                #endregion
                #region http
                else if (myLocation.ToLower().StartsWith("http://"))
                {
                    stream = GetStreamFromHttp(myLocation);
                } 
                #endregion
                else
                {
                    error = new InvalidImportLocationException(myLocation, @"file:\\", "http://");
                    result = new QueryResult("", ImportFormat, 0L, ResultType.Failed, null, error);
                    return result;
                }

                #region Start import using the AGraphDBImport implementation and return the result

                return Import(stream, myGraphDB, myGraphQL, mySecurityToken, myTransactionToken, myParallelTasks, myComments, myOffset, myLimit, myVerbosityType);

                #endregion
            }
            catch (Exception ex)
            {
                #region throw new exception
                error = new ImportFailedException(ex);
                result = new QueryResult("", ImportFormat, 0L, ResultType.Failed, null, error);
                return result; 
                #endregion
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            #endregion
        }
        
        private QueryResult Import(Stream myInputStream, IGraphDB myIGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, UInt32 myParallelTasks = 1U, IEnumerable<string> myComments = null, ulong? myOffset = null, ulong? myLimit = null, VerbosityTypes myVerbosityType = VerbosityTypes.Silent)
        {
            var lines = ReadLinesFromStream(myInputStream);

            #region Evaluate Limit and Offset

            if (myOffset != null)
            {
                if(lines != null)
                    lines = lines.Skip<String>(Convert.ToInt32(myOffset.Value));
            }
            if (myLimit != null)
            {
                if(lines != null)
                    lines = lines.Take<String>(Convert.ToInt32(myLimit.Value));
            }

            #endregion

            #region Import queries

            QueryResult queryResult;
            if (myParallelTasks > 1)
            {
                queryResult = ExecuteAsParallel(lines, myGraphQL, mySecurityToken, myTransactionToken, myVerbosityType, myParallelTasks, myComments);
            }
            else
            {
                queryResult = ExecuteAsSingleThread(lines, myGraphQL, mySecurityToken, myTransactionToken, myVerbosityType, myComments);
            }

            #endregion

            return queryResult;
        }



        private QueryResult ExecuteAsParallel(IEnumerable<String> myLines, IGraphQL myIGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, VerbosityTypes myVerbosityType, UInt32 myParallelTasks = 1U, IEnumerable<String> comments = null)
        {
            #region data
            QueryResult queryResult = new QueryResult(myLines.ToString(), ImportFormat, 0L, ResultType.Successful);
            Int64 numberOfLine = 0;
            var query = String.Empty;
            var aggregatedResults = new List<IEnumerable<IVertexView>>();
            Stopwatch StopWatchLine = new Stopwatch();
            Stopwatch StopWatchLines = new Stopwatch();
            #endregion

            #region Create parallel options

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = (int)myParallelTasks
            };

            #endregion
            StopWatchLines.Start();
            Parallel.ForEach(myLines, parallelOptions, (line, state) =>
            {

                if (!IsComment(line, comments))
                {

                    Interlocked.Add(ref numberOfLine, 1L);

                    if (!IsComment(line, comments)) // Skip comments
                    {

                        var qresult = ExecuteQuery(line, myIGraphQL, mySecurityToken, myTransactionToken);

                        #region VerbosityTypes.Full: Add result

                        if (myVerbosityType == VerbosityTypes.Full)
                        {
                            lock (aggregatedResults)
                            {
                                aggregatedResults.Add(qresult.Vertices);
                            }
                        }

                        #endregion

                        #region !VerbosityTypes.Silent: Add errors and break execution

                        if (qresult.TypeOfResult != ResultType.Successful && myVerbosityType != VerbosityTypes.Silent)
                        {
                            lock (queryResult)
                            {
                                queryResult = new QueryResult(line, ImportFormat, Convert.ToUInt64(StopWatchLine.ElapsedMilliseconds), ResultType.Failed, qresult.Vertices, qresult.Error);
                            }
                            state.Break();
                        }

                        #endregion

                    }

                }

            });
            StopWatchLines.Stop();

            //add the results of each query into the queryResult
            if (queryResult != null)
                queryResult = new QueryResult(myLines.ToString(), ImportFormat, Convert.ToUInt64(StopWatchLines.ElapsedMilliseconds), queryResult.TypeOfResult, AggregateListOfListOfVertices(aggregatedResults), queryResult.Error);
            else
                queryResult = new QueryResult(myLines.ToString(), ImportFormat, Convert.ToUInt64(StopWatchLines.ElapsedMilliseconds), ResultType.Successful, AggregateListOfListOfVertices(aggregatedResults));

            return queryResult;

        }

        private QueryResult ExecuteAsSingleThread(IEnumerable<String> myLines, IGraphQL myIGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, VerbosityTypes myVerbosityType, IEnumerable<String> comments = null)
        {

            #region data

            QueryResult queryResult = null;
            Int64 numberOfLine = 0;
            var aggregatedResults = new List<IEnumerable<IVertexView>>(); 
            Stopwatch StopWatchLines = new Stopwatch();
            
            #endregion

            #region check lines and execute query
            
            StopWatchLines.Reset();
            StopWatchLines.Start();
            
            foreach (var _Line in myLines)
            {
                numberOfLine++;

                if (String.IsNullOrWhiteSpace(_Line))
                {
                    continue;
                }

                #region Skip comments

                if (IsComment(_Line, comments))
                    continue;

                #endregion

                #region execute query

                var tempResult = myIGraphQL.Query(mySecurityToken, myTransactionToken, _Line);
                
                #endregion

                #region Add errors and break execution

                if (tempResult.TypeOfResult == ResultType.Failed)
                {
                    if (tempResult.Error.Message.Equals("Mal-formed  string literal - cannot find termination symbol."))
                        Debug.WriteLine("Query at line [" + numberOfLine + "] [" + _Line + "] failed with " + tempResult.Error.ToString() + " add next line...");

                    if (myVerbosityType == VerbosityTypes.Errors)
                    {
                        queryResult = new QueryResult(_Line, ImportFormat, 0L, ResultType.Failed, tempResult.Vertices, tempResult.Error);

                        break;
                    }
                }

                aggregatedResults.Add(tempResult.Vertices);

                #endregion
            } 
            
            StopWatchLines.Stop();
            
            #endregion

            //add the results of each query into the queryResult
            if(queryResult != null)
                queryResult = new QueryResult(myLines.ToString(), ImportFormat, Convert.ToUInt64(StopWatchLines.ElapsedMilliseconds), queryResult.TypeOfResult, AggregateListOfListOfVertices(aggregatedResults), queryResult.Error);
            else
                queryResult = new QueryResult(myLines.ToString(), ImportFormat, Convert.ToUInt64(StopWatchLines.ElapsedMilliseconds), ResultType.Successful, AggregateListOfListOfVertices(aggregatedResults));

            return queryResult;

        }

        /// <summary>
        /// Aggregates different enumerations of readout objects
        /// </summary>
        /// <param name="myListOfListOfVertices"></param>
        /// <returns></returns>
        private IEnumerable<IVertexView> AggregateListOfListOfVertices(List<IEnumerable<IVertexView>> myListOfListOfVertices)
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
            if (comments == null || comments.Count() == 0)
                return false;

            return comments.Any(c => myQuery.StartsWith(c));
        }

        private QueryResult ExecuteQuery(String myQuery, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return myGraphQL.Query(mySecurityToken, myTransactionToken, myQuery);
        }

        #endregion

        #region Get streams

        /// <summary>
        /// Reads a file, just let all exceptions thrown, they are too much to pack them into a graphDBException.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private Stream GetStreamFromFile(String location)
        {
            return File.OpenRead(location);
        }

        /// <summary>
        /// Reads a http ressource, just let all exceptions thrown, they are too much to pack them into a graphDBException.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private Stream GetStreamFromHttp(String location)
        {
            var request = (HttpWebRequest)WebRequest.Create(location);
            var response = request.GetResponse();
            return response.GetResponseStream();
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Will read all lines of the <paramref name="myInputStream"/>. 
        /// Keep in mind, that this can be done ony 1 time. You need to seek the stream to 0 to reread all lines again.
        /// </summary>
        /// <param name="myInputStream"></param>
        /// <returns></returns>
        private IEnumerable<String> ReadLinesFromStream(Stream myInputStream)
        {
            var streamReader = new StreamReader(myInputStream);
            while (!streamReader.EndOfStream)
            {
                yield return streamReader.ReadLine();
            }
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.gqlimport"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new GraphDBImport_GQL();
        }

        #endregion
    }
}
