/* 
 * AGraphDBExport
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Interfaces;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.ImportExport
{

    enum TypeOfOutputDestination
    {
        QueryResult,
        File,
        Http
    }

    /// <summary>
    /// The base class for a graph DB export. Implement Export method and use the "Write" method to export some lines.
    /// </summary>
    public abstract class AGraphDBExport : IDisposable
    {

        #region Data

        public abstract string ExportFormat { get; }

        private TypeOfOutputDestination _TypeOfOutputDestination;
        private StreamWriter _Stream;
        private HttpWebRequest request;
        private Dictionary<String, Object> _DumpReadout = new Dictionary<String, Object>();
        private String _Destination;

        #endregion

        #region Export

        public abstract Exceptional Export(DBContext myDBContext, IDumpable myGrammar, IEnumerable<GraphDBType> myTypes, DumpTypes myDumpType, VerbosityTypes verbosityType = VerbosityTypes.Errors);

        public QueryResult Export(String destination, DBContext myDBContext, IDumpable myGrammar, IEnumerable<String> myTypes, DumpTypes myDumpType, VerbosityTypes verbosityType = VerbosityTypes.Errors)
        {

            _Destination = destination;

            #region Open destination

            var openStreamResult = OpenStream(destination);
            if (!openStreamResult.Success())
            {
                return new QueryResult(openStreamResult);
            }

            #endregion

            #region Start export using the AGraphDBImport implementation

            var result = Export(myDBContext, myGrammar, GetTypes(myDBContext, myTypes), myDumpType, verbosityType);

            #endregion

            #region Close destination

            var closeStreamResult = CloseStream();
            if (!closeStreamResult.Success())
            {
                return new QueryResult(openStreamResult);
            }

            #endregion

            if (!result.Success())
            {
                return new QueryResult(result);
            }

            return new QueryResult(new List<Vertex>() { new Vertex(_DumpReadout) });

        }

        #endregion

        #region Protected write

        /// <summary>
        /// This will write the output lines to the destination. Only one output for each <paramref name="myDumpType"/> is allowed.
        /// </summary>
        /// <param name="myDumpType"></param>
        /// <param name="lines"></param>
        protected Exceptional Write(DumpTypes myDumpType, IEnumerable<String> lines)
        {

            if (_DumpReadout.ContainsKey(myDumpType.ToString()))
            {
                return new Exceptional(new Error_ArgumentException(myDumpType.ToString() + " already added"));
            }

            switch (_TypeOfOutputDestination)
            {

                case TypeOfOutputDestination.QueryResult:
                    _DumpReadout.Add(myDumpType.ToString(), lines);

                    break;

                default:
                    _DumpReadout.Add(myDumpType.ToString(), _Destination);

                    foreach (var line in lines)
                    {
                        _Stream.WriteLine(line);
                    }
                    break;
            }

            return Exceptional.OK;

        }
        
        #endregion


        #region Output handling

        private Exceptional OpenStream(String destination)
        {
            if (String.IsNullOrEmpty(destination))
            {
                _TypeOfOutputDestination = TypeOfOutputDestination.QueryResult;
            }
            else if (destination.ToLower().StartsWith(@"file:\\"))
            {
                #region file:

                _TypeOfOutputDestination = TypeOfOutputDestination.File;
                try
                {
                    _Stream = new StreamWriter(File.Create(destination.Substring(@"file:\\".Length).TrimStart('\\')));
                }
                catch (Exception ex)
                {
                    return new Exceptional(new Error_UnknownDBError(ex));
                }
                
                #endregion
            }
            else if (destination.ToLower().StartsWith("http://"))
            {
                #region http

                _TypeOfOutputDestination = TypeOfOutputDestination.Http;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(destination);
                    request.Method = "PUT";
                    request.Timeout = 1000;
                    _Stream = new StreamWriter(request.GetRequestStream());
                }
                catch (Exception ex)
                {
                    return new Exceptional(new Error_UnknownDBError(ex));
                }

                #endregion
            }
            else
            {
                return new Exceptional(new Error_InvalidDumpLocation(destination, @"file:\\", "http://"));
            }

            return Exceptional.OK;
        }


        private Exceptional CloseStream()
        {

            if (_Stream == null)
            {
                return Exceptional.OK;
            }

            try
            {

                switch (_TypeOfOutputDestination)
                {
                    case TypeOfOutputDestination.File:
                        _Stream.Close();
                        _Stream.Dispose();
                        break;

                    case TypeOfOutputDestination.Http:
                        _Stream.Dispose();

                        var response = request.GetResponse();
                        using (var stream = new StreamReader(response.GetResponseStream()))
                        {

                            var errors = stream.ReadToEnd();

                            if (!String.IsNullOrEmpty(errors))
                            {
                                return new Exceptional(new Error_UnknownDBError(errors));
                            }

                        }

                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                return new Exceptional(new Error_UnknownDBError(ex));
            }

            return Exceptional.OK;

        }

        #endregion


        #region GetTypes to dump

        private IEnumerable<GraphDBType> GetTypes(DBContext myDBContext, IEnumerable<String> myTypes)
        {

            #region GetTypeToDump

            IEnumerable<GraphDBType> typesToDump;
            if (myTypes.IsNullOrEmpty())
            {
                typesToDump = myDBContext.DBTypeManager.GetAllTypes(false);
            }
            else
            {
                var typesToDumpHash = new HashSet<GraphDBType>();
                foreach (var stringType in myTypes)
                {
                    var type = myDBContext.DBTypeManager.GetTypeByName(stringType);
                    if (type == null)
                    {
                        throw new GraphDBException(new Errors.Error_TypeDoesNotExist(stringType));
                    }

                    //typesToDumpHash.UnionWith(myDBContext.DBTypeManager.GetAllParentTypes(type, true, false));
                    AddTypeAndAttributesRecursivly(myDBContext, type, ref typesToDumpHash);

                }
                typesToDump = typesToDumpHash;
            }

            #endregion

            return typesToDump;

        }

        private void AddTypeAndAttributesRecursivly(DBContext myDBContext, GraphDBType type, ref HashSet<GraphDBType> types)
        {
            if (!type.IsUserDefined) return;

            foreach (var ptype in myDBContext.DBTypeManager.GetAllParentTypes(type, true, false))
            {
                if (!types.Contains(ptype))
                {
                    types.Add(ptype);
                    foreach (var attr in type.GetAllAttributes(ta => !ta.IsBackwardEdge, myDBContext, false))
                    {
                        AddTypeAndAttributesRecursivly(myDBContext, attr.GetDBType(myDBContext.DBTypeManager), ref types);
                    }
                }

                //types.UnionWith(myDBContext.DBTypeManager.GetAllParentTypes(type, true, false));

            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (_Stream != null)
            {
                CloseStream();
            }
        }

        #endregion


    }
}
