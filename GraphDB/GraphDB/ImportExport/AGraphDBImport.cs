/* 
 * AGraphDBImport
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using sones.GraphDB.Errors;
using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;


#endregion

namespace sones.GraphDB.ImportExport
{

    /// <summary>
    /// The base import class. Implement the Import method to do what you want with the input lines.
    /// </summary>
    public abstract class AGraphDBImport
    {

        public abstract string ImportFormat { get; }

        public abstract QueryResult Import(Stream myInputStream, IGraphDBSession myGraphDBSession, DBContext myDBContext, UInt32 myParallelTasks = 1, IEnumerable<string> myComments = null, UInt64? myOffset = null, UInt64? myLimit = null, VerbosityTypes myVerbosityType = VerbosityTypes.Errors);

        public QueryResult Import(String location, IGraphDBSession graphDBSession, DBContext myDBContext, UInt32 parallelTasks = 1, IEnumerable<string> comments = null, UInt64? offset = null, UInt64? limit = null, VerbosityTypes verbosityType = VerbosityTypes.Errors)
        {

            QueryResult result = new QueryResult();
            Stream stream = null;

            #region Read querie lines from location

            try
            {
                if (location.ToLower().StartsWith(@"file:\\"))
                {
                    //lines = ReadFile(location.Substring(@"file:\\".Length));
                    stream = GetStreamFromFile(location.Substring(@"file:\\".Length));
                }
                else if (location.ToLower().StartsWith("http://"))
                {
                    stream = GetStreamFromHttp(location);
                }
                else
                {
                    return new QueryResult(new Exceptional(new Error_InvalidImportLocation(location, @"file:\\", "http://")));
                }

                #region Start import using the AGraphDBImport implementation and return the result
          
                return Import(stream, graphDBSession, myDBContext, parallelTasks, comments, offset, limit, verbosityType);

                #endregion

            }
            catch (Exception ex)
            {
                return new QueryResult(new Exceptional(new Error_ImportFailed(ex)));
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
        protected IEnumerable<String> ReadLinesFromStream(Stream myInputStream)
        {
            var streamReader = new StreamReader(myInputStream);
            while (!streamReader.EndOfStream)
            {
                yield return streamReader.ReadLine();
            }
        }

        #endregion

    }
}
