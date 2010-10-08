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

        public abstract QueryResult Import(IEnumerable<String> lines, IGraphDBSession graphDBSession, DBContext myDBContext, UInt32 parallelTasks = 1, IEnumerable<string> comments = null, UInt64? offset = null, UInt64? limit = null, VerbosityTypes verbosityType = VerbosityTypes.Errors);

        public QueryResult Import(String location, IGraphDBSession graphDBSession, DBContext myDBContext, UInt32 parallelTasks = 1, IEnumerable<string> comments = null, UInt64? offset = null, UInt64? limit = null, VerbosityTypes verbosityType = VerbosityTypes.Errors)
        {

            IEnumerable<String> lines = null;

            #region Read querie lines from location

            try
            {
                if (location.ToLower().StartsWith(@"file:\\"))
                {
                    lines = ReadFile(location.Substring(@"file:\\".Length));
                }
                else if (location.ToLower().StartsWith("http://"))
                {
                    lines = ReadHttpResource(location);
                }
                else
                {
                    return new QueryResult(new Exceptional(new Error_InvalidImportLocation(location, @"file:\\", "http://")));
                }
            }
            catch (Exception ex)
            {
                return new QueryResult(new Exceptional(new Error_ImportFailed(ex)));
            }

            #endregion

            #region Start import using the AGraphDBImport implementation

            return Import(lines, graphDBSession, myDBContext, parallelTasks, comments, offset, limit, verbosityType);

            #endregion

        }

        /// <summary>
        /// Reads a file, just let all exceptions thrown, they are too much to pack them into a graphDBException.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private IEnumerable<String> ReadFile(String location)
        {
            return File.ReadAllLines(location);
        }


        /// <summary>
        /// Reads a http ressource, just let all exceptions thrown, they are too much to pack them into a graphDBException.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private IEnumerable<String> ReadHttpResource(String location)
        {
            var request = (HttpWebRequest)WebRequest.Create(location);
            var response = request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());
            
            while (!stream.EndOfStream)
            {
                yield return stream.ReadLine();
            }
        }

    }
}
