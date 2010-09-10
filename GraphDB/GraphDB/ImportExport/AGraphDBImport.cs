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
 * AGraphDBImport
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using sones.GraphDB.Errors;
using sones.GraphDBInterface.Result;
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
