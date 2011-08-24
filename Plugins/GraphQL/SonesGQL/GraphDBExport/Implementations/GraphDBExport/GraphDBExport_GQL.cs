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
using System.IO;
using System.Linq;
using System.Net;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager;
using sones.Library.LanguageExtensions;
using System.Text;
using sones.Library.CollectionWrapper;

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

        public string ExporterName
        {
            get { return "GQLEXPORT"; }
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

            if(error != null)
                return new QueryResult("", ExportFormat, 0L, ResultType.Failed, result.Vertices, error);
            else
                return new QueryResult("", ExportFormat, 0L, result.TypeOfResult, result.Vertices);
        }

        private QueryResult Export(IDumpable myGrammar, IGraphDB myGraphDB, IGraphQL myGraphQL, SecurityToken mySecurityToken, TransactionToken myTransactionToken, IEnumerable<IVertexType> myTypes, DumpTypes myDumpType)
        {
            var dumpReadout = new Dictionary<String, Object>();
            ASonesException error = null;

            #region dump gddl
            if ((myDumpType & DumpTypes.GDDL) == DumpTypes.GDDL)
            {

                var graphDDL = myGrammar.ExportGraphDDL(DumpFormats.GQL, myTypes);

                if (graphDDL == null)
                {
                    throw new ExportFailedException(myDumpType.ToString(), "");

                }

                dumpReadout.Add("GDDL", new ListCollectionWrapper(graphDDL));

                try
                {
                    Write(DumpTypes.GDDL, graphDDL);
                }
                catch (ASonesException e)
                {
                    error = e;
                }

            } 
            #endregion

            #region dump gdml
            if ((myDumpType & DumpTypes.GDML) == DumpTypes.GDML)
            {

                var graphDML = myGrammar.ExportGraphDML(DumpFormats.GQL, myTypes, mySecurityToken, myTransactionToken);

                if (graphDML == null)
                {
                    throw new ExportFailedException(myDumpType.ToString(), "");
                }

                dumpReadout.Add("GDML", new ListCollectionWrapper(graphDML));

                try
                {
                    Write(DumpTypes.GDML, graphDML);
                }
                catch (ASonesException e)
                {
                    error = e;
                }

            } 
            #endregion

            return new QueryResult("", ExportFormat, 0L, ResultType.Successful, new List<IVertexView> { new VertexView(dumpReadout, null) }, error);
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.gqlexport"; }
        }

        public string PluginShortName
        {
            get { return "gqlexport"; }
        }

        public string PluginDescription
        {
            get { return "This class realizes GQL code export to a file."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
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
                throw new ArgumentException(myDumpType.ToString() + " already added");
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
                    if (!destination.EndsWith(".gql"))
                        destination = new StringBuilder(destination).Append(".gql").ToString();

                    _Stream = new StreamWriter(File.Create(destination.Substring(@"file:\\".Length).TrimStart('\\')));
                }
                catch (Exception ex)
                {
                    throw new StreamWriterException("System.IO.StreamWriter", "Error create File.", ex);
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
                    throw new StreamWriterException("System.IO.StreamWriter", "Error on HttpWebRequest.", ex);
                }

                #endregion
            }
            else
            {
                throw new InvalidDumpLocationException(destination, @"file:\\", "http://", "");
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
                                throw new StreamReaderException("System.IO.StreamReader", "", null);
                            }

                        }

                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                throw new StreamReaderException("System.IO.StreamReader", "", ex);
            }

        }

        #endregion

        #region GetTypes to dump

        private IEnumerable<IVertexType> GetTypes(ref IGraphDB myGraphDB, ref SecurityToken mySecurityToken, ref TransactionToken myTransactionToken, IEnumerable<String> myTypes)
        {

            #region GetTypeToDump

            List<IVertexType> typesToDump = new List<IVertexType>();

            if (myTypes.IsNullOrEmpty())
            {
                foreach (var type in myGraphDB.GetAllVertexTypes(mySecurityToken, myTransactionToken, new RequestGetAllVertexTypes(), (stats, vertexTypes) => vertexTypes))
                { 
                    if(type.IsUserDefined)
                    {
                        typesToDump.Add(type);
                    }
                }
            }
            else
            {
                var typesToDumpHash = new HashSet<IVertexType>();
                foreach (var stringType in myTypes)
                {
                    var type = myGraphDB.GetVertexType(mySecurityToken, myTransactionToken, new RequestGetVertexType(stringType), (stats, vertexType) => vertexType);
                    
                    if (type == null)
                    {
                        throw new TypeDoesNotExistException(stringType, "");
                    }

                    //typesToDumpHash.UnionWith(myDBContext.DBTypeManager.GetAllParentTypes(type, true, false));
                    AddTypeAndAttributesRecursivly(ref myGraphDB, ref mySecurityToken, ref myTransactionToken, type, ref typesToDumpHash);

                }

                typesToDump = typesToDumpHash.ToList();
            }

            #endregion

            return typesToDump;

        }

        private void AddTypeAndAttributesRecursivly(ref IGraphDB myGraphDB, ref SecurityToken mySecurityToken, ref TransactionToken myTransactionToken, IVertexType type, ref HashSet<IVertexType> types)
        {
            if (!type.IsUserDefined) return;

            if (type.HasParentType)
            {
                if (!types.Contains(type.ParentVertexType))
                {
                    types.Add(type.ParentVertexType);
                    foreach (var attr in (type.GetAttributeDefinitions(false)).Where(attrDef => attrDef.Kind == AttributeType.Property))
                    {
                        var attrType = myGraphDB.GetVertexType<IVertexType>(mySecurityToken, myTransactionToken, new RequestGetVertexType(attr.ID), (stats, vertex) => vertex);
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
