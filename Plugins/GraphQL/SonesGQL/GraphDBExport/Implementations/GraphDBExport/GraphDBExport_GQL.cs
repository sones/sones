using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.TypeSystem;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Library.ErrorHandling;
using sones.Library.LanguageExtensions;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.DBExport
{
    enum TypeOfOutputDestination
    {
        QueryResult,
        File,
        Http
    }

    public sealed class GraphDBExport_GQL : IGraphDBExport, IPluginable
    {
        #region Data

        private TypeOfOutputDestination _TypeOfOutputDestination;
        private StreamWriter _Stream;
        private HttpWebRequest _HttpWebRequest;
        private Dictionary<String, Object> _DumpReadout = new Dictionary<String, Object>();
        private String _Destination;

        #endregion

        #region constructor

        public GraphDBExport_GQL()
        { }

        #endregion

        #region IGraphDBExport Member

        public string ExportFormat
        {
            get { return "sones.gql"; }
        }

        public QueryResult Export(string destination, IDumpable myGrammar, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IEnumerable<String> myTypes, DumpTypes myDumpType)
        {
            _Destination = destination;
            ASonesException error = null;

            #region Open destination

            try
            {
                OpenStream(destination);
            }
            catch (ASonesException e)
            {
                error = e;
            }

            #endregion

            #region Start export using the AGraphDBExport implementation

            var result = Export(myGrammar, myGraphDB, myGraphQL, mySecurityToken, myTransactionToken, GetTypes(ref myGraphDB, ref mySecurityToken, ref myTransactionToken, myTypes), myDumpType);

            #endregion

            #region Close destination

            try
            {
                CloseStream();
            }
            catch (ASonesException e)
            {
                error = e;
            }
            
            #endregion

            return new QueryResult("", ExportFormat, 0L, result.TypeOfResult, result.Vertices, error);
        }

        private QueryResult Export(IDumpable myGrammar, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IEnumerable<IVertexType> myTypes, DumpTypes myDumpType)
        {
            var dumpReadout = new Dictionary<String, Object>();
            ASonesException error = null;

            if ((myDumpType & DumpTypes.GDDL) == DumpTypes.GDDL)
            {
                
                var graphDDL = myGrammar.ExportGraphDDL(DumpFormats.GQL, myTypes);

                if (graphDDL == null)
                {
                    //throw Exception
                    //return new Exceptional(graphDDL);
                }

                dumpReadout.Add("GDDL", graphDDL);

                try
                {
                    Write(DumpTypes.GDDL, graphDDL);
                }
                catch (ASonesException e)
                {
                    error = e;
                }

            }

            if ((myDumpType & DumpTypes.GDML) == DumpTypes.GDML)
            {

                var graphDML = myGrammar.ExportGraphDML(DumpFormats.GQL, myTypes);

                if (graphDML == null)
                {
                    //throw Exception
                    //return new Exceptional(graphDML);
                }

                dumpReadout.Add("GDML", graphDML);

                try
                {
                    Write(DumpTypes.GDML, graphDML);
                }
                catch (ASonesException e)
                {
                    error = e;
                }

            }

            return new QueryResult("", ExportFormat, 0L, ResultType.Successful, new List<IVertexView> { new VertexView(dumpReadout, null) }, error);
        }

        #endregion

        #region IPluginable Member

        public string PluginName
        {
            get { return "sones.gqlexport"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new GraphDBExport_GQL();
        }

        #endregion

        #region Helper

        #region Protected write

        /// <summary>
        /// This will write the output lines to the destination. Only one output for each <paramref name="myDumpType"/> is allowed.
        /// </summary>
        /// <param name="myDumpType"></param>
        /// <param name="lines"></param>
        public void Write(DumpTypes myDumpType, IEnumerable<String> lines)
        {

            if (_DumpReadout.ContainsKey(myDumpType.ToString()))
            {
                //throw Exception
                //return new Exceptional(new Error_ArgumentException(myDumpType.ToString() + " already added"));
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

        }

        #endregion


        #region Output handling

        private void OpenStream(String destination)
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
                    //throw Esxcception
                    //return new Exceptional(new Error_UnknownDBError(ex));
                }

                #endregion
            }
            else if (destination.ToLower().StartsWith("http://"))
            {
                #region http

                _TypeOfOutputDestination = TypeOfOutputDestination.Http;
                try
                {
                    _HttpWebRequest = (HttpWebRequest)WebRequest.Create(destination);
                    _HttpWebRequest.Method = "PUT";
                    _HttpWebRequest.Timeout = 1000;
                    _Stream = new StreamWriter(_HttpWebRequest.GetRequestStream());
                }
                catch (Exception ex)
                {
                    //thrwo Exception
                    //return new Exceptional(new Error_UnknownDBError(ex));
                }

                #endregion
            }
            else
            {
                //throw Exception
                //return new Exceptional(new Error_InvalidDumpLocation(destination, @"file:\\", "http://"));
            }

        }


        private void CloseStream()
        {

            if (_Stream == null)
            {
                return;
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

                        var response = _HttpWebRequest.GetResponse();
                        using (var stream = new StreamReader(response.GetResponseStream()))
                        {

                            var errors = stream.ReadToEnd();

                            if (!String.IsNullOrEmpty(errors))
                            {
                                //thrwo Exception
                                //return new Exceptional(new Error_UnknownDBError(errors));
                            }

                        }

                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                //thrwo Exception
                //return new Exceptional(new Error_UnknownDBError(ex));
            }

        }

        #endregion


        #region GetTypes to dump

        private IEnumerable<IVertexType> GetTypes(ref IGraphDB myGraphDB, ref SecurityToken mySecurityToken, ref TransactionToken myTransactionToken, IEnumerable<String> myTypes)
        {

            #region GetTypeToDump

            IEnumerable<IVertexType> typesToDump;

            if (myTypes.IsNullOrEmpty())
            {
                typesToDump = myGraphDB.GetAllVertexTypes(mySecurityToken, myTransactionToken, new RequestGetAllVertexTypes(), (stats, vertexTypes) => vertexTypes);
            }
            else
            {
                var typesToDumpHash = new HashSet<IVertexType>();
                foreach (var stringType in myTypes)
                {
                    var type = myGraphDB.GetVertexType(mySecurityToken, myTransactionToken, new RequestGetVertexType(stringType), (stats, vertexType) => vertexType);
                    
                    if (type == null)
                    {
                        //thrwo new Exception
                        //throw new GraphDBException(new Errors.Error_TypeDoesNotExist(stringType));
                    }

                    //typesToDumpHash.UnionWith(myDBContext.DBTypeManager.GetAllParentTypes(type, true, false));
                    AddTypeAndAttributesRecursivly(ref myGraphDB, ref mySecurityToken, ref myTransactionToken, type, ref typesToDumpHash);

                }
                typesToDump = typesToDumpHash;
            }

            #endregion

            return typesToDump;

        }

        private void AddTypeAndAttributesRecursivly(ref IGraphDB myGraphDB, ref SecurityToken mySecurityToken, ref TransactionToken myTransactionToken, IVertexType type, ref HashSet<IVertexType> types)
        {
            if (!type.IsUserDefined) return;

            if (type.HasParentType)
            {
                if (!types.Contains(type.GetParentVertexType))
                {
                    types.Add(type.GetParentVertexType);
                    foreach (var attr in (type.GetAttributeDefinitions(false)).Where(attrDef => attrDef.Kind == AttributeType.Property))
                    {
                        var attrType = myGraphDB.GetVertexType<IVertexType>(mySecurityToken, myTransactionToken, new RequestGetVertexType(attr.AttributeID), (stats, vertex) => vertex);
                        AddTypeAndAttributesRecursivly(ref myGraphDB, ref mySecurityToken, ref myTransactionToken, attrType, ref types);
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

        #endregion
    }
}
