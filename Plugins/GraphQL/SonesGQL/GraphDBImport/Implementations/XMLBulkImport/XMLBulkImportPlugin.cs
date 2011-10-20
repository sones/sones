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
using System.Reflection;
using System.Xml;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.TypeSystem;
using sones.GraphFS;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.Library.DataStructures;
using sones.Library.LanguageExtensions;
using sones.Library.PropertyHyperGraph;
using sones.Library.SlimLogFramework;
using sones.Library.SlimLogFramework.Logger;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.DBImport;
using sones.Plugins.SonesGQL.DBImport.ErrorHandling;
using sones.Library.SystemInformation;

namespace sones.Plugins.SonesGQL.XMLBulkImport
{

    /// <summary>
    /// A class that represents a plugin for importing data as XML into IGraphDB
    /// </summary>
    public sealed class XMLBulkImportPlugin : IGraphDBImport
    {

        #region inner classes

        /// <summary>
        /// A class that holds the necessary information of the available vertex types.
        /// </summary>
        private class InternVertexType
        {
            public InternVertexType(String myName, long myID, IDictionary<String, long> myAttributes)
            {
                Name = myName;
                ID = myID;
                Attributes = myAttributes;
            }

            public String Name { get; private set; }
            public long ID { get; private set; }
            public IDictionary<String, long> Attributes { get; private set; }
        }

        /// <summary>
        /// A class that represents an vertex identification
        /// </summary>
        private class VertexID
        {
            public VertexID(String myVertexType, long myVertexID)
            {
                VertexType = myVertexType;
                ID = myVertexID;
            }

            public String VertexType { get; private set; }
            public long ID           { get; private set; }
        }

        /// <summary>
        /// A class that represents a multi link statement
        /// </summary>
        private class MultiLink
        {
            public MultiLink(String myKey, IEnumerable<Link> myLinks )
            {
                Key = myKey;
                Links = myLinks;
            }

            public String Key              { get; private set; }
            public IEnumerable<Link> Links { get; private set; }
        }

        /// <summary>
        /// A class that represents a single link statement
        /// </summary>
        private class SingleLink
        {
            public SingleLink(String myKey, Link myLink)
            {
                Key = myKey;
                Link = myLink;
            }

            public String Key { get; private set; }
            public Link Link  { get; private set; }
        }


        /// <summary>
        /// A class that represents one link.
        /// </summary>
        private class Link
        {
            public Link(VertexID myTargetID, IDictionary<String, String> myValues)
            {
                TargetID = myTargetID;
                Values = myValues;
            }

            public VertexID TargetID                   { get; private set; }
            public IDictionary<String, String> Values  { get; private set; }
        }

        #endregion

        #region Constants

        #region Plug-in Parameter

        public const string LogLocation = "Location"; 

        #endregion

        #region XML Schema

        public const string ImportTag              = "Import";
        public const string VertexIDAttribute      = "VertexID";
        public const string AttributeNameAttribute = "AttributeName";
        public const string PrePhaseTag            = "PrePhase";
        public const string PostPhaseTag           = "PostPhase";
        public const string InsertTag              = "Insert";
        public const string VertexTypeAttribute    = "VertexType";
        public const string SetValueTag            = "SetValue";
        public const string SingleLinkTag          = "SingleLink";
        public const string MultiLinkTag           = "MultiLink";
        public const string LinkTag                = "Link";
        public const string ValueAttribute         = "Value";

        #endregion

        #region Others

        private const string Comment = "Comment";
        private const string Edition = "Edition";
        private const string ModificationDate = "ModificationDate";
        private const string CreationDate = "CreationDate";
        
        #endregion

        #endregion

        #region Data
        /// <summary>
        /// The lock to avoid multiple concurrent calls to this plugin.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// The stream that contains the xml data.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The xml reader that reads the current stream.
        /// </summary>
        private XmlTextReader _reader;
        /// <summary>
        /// The IGraphDB instance that was given with the current call.
        /// </summary>
        private IGraphDB _db;

        /// <summary>
        /// The IGraphQL instance that was given with the current call.
        /// </summary>
        private IGraphQL _ql;

        /// <summary>
        /// The security token that was given with the current call.
        /// </summary>
        private SecurityToken _security;

        /// <summary>
        /// The transaktion token that was given with the current call.
        /// </summary>
        private Int64 _transaction;

        /// <summary>
        /// The offset of the current import.
        /// </summary>
        private ulong? _offset;

        /// <summary>
        /// The limit of the current import.
        /// </summary>
        private ulong? _limit;

        /// <summary>
        /// The IGraphFS instance that is to be filled.
        /// </summary>
        private IGraphFS _resultingFS;

        /// <summary>
        /// Stores the current vertex types indexed by their name.
        /// </summary>
        private IDictionary<string, InternVertexType> _vertextypes;

        /// <summary>
        /// Stores the result types of properties indexes by the property id.
        /// </summary>
        private IDictionary<long, Type> _propertyTypes;

        /// <summary>
        /// Stores the edge type of outgoing edges indexes by the property id.
        /// </summary>
        private IDictionary<long, long> _edgeTypes;

        /// <summary>
        /// Stores the property ids of the current edge types indexes by the property name.
        /// </summary>
        private IDictionary<string, long> _edgeProps;

        /// <summary>
        /// The ILogger instance, that can be used to log information.
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// Stores how many vertices were stored during the current import.
        /// </summary>
        private ulong _currentImport;

        /// <summary>
        /// The path to the log location.
        /// </summary>
        private string _logpath;

        /// <summary>
        /// Indicates if the current import was finished by some condition.
        /// </summary>
        private bool _closed;
        private IncomingEdgeSorter _sorter;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a plugin stub, needed for current plugin manager
        /// </summary>
        public XMLBulkImportPlugin()
        { }

        /// <summary>
        /// Creates a plugin
        /// </summary>
        /// <param name="myPath">The path the plugin will log to.</param>
        public XMLBulkImportPlugin(String myPath)
        {
            if (myPath == null)
                throw new ArgumentNullException("myPath");

            _logpath = myPath;
        }

        #endregion

        #region IGraphDBImport Members

        public QueryResult Import(String myLocation,
			IGraphDB myGraphDB,
			IGraphQL myGraphQL,
			SecurityToken mySecurityToken,
			Int64 myTransactionToken,
			UInt32 myParallelTasks = 1U,
			IEnumerable<string> myComments = null,
			UInt64? myOffset = null,
			UInt64? myLimit = null,
			VerbosityTypes myVerbosityType = VerbosityTypes.Silent,
			Dictionary<string, string> myOptions = null)
        {
            if (myGraphDB == null)
                throw new ArgumentNullException("myGraphDB");

            if (myGraphQL == null)
                throw new ArgumentNullException("myGraphQL");

            if (!typeof(SonesGraphDB).Equals(myGraphDB.GetType()))
                throw new ArgumentException("FASTIMPORT is designed for SonesGraphDB only.");

            lock (_lock)
            {
                //if verbositiy is silent, we do not configure the logger, so it is an empty logger.
                if (myVerbosityType != VerbosityTypes.Silent)
                {
                    Level logLevel = (myVerbosityType == VerbosityTypes.Full)
                        ? Level.FINE
                        : Level.INFO;
                    LogManager.Instance.ConfigureLogger("FastImport", new FileLogger(_logpath, logLevel));
                }

                //store some arguments as fields, because there is at most one execution at any time.
                _logger = LogManager.Instance.GetLogger("FastImport");
                _db = myGraphDB;
                _ql = myGraphQL;
                _security = mySecurityToken;
                _transaction = myTransactionToken;
                _offset = myOffset;
                _limit = myLimit;
                _currentImport = 0L;
                _closed = false;
                _sorter = new IncomingEdgeSorter(myOptions);

                try
                {
                    if (myLocation.ToLower().StartsWith(@"file:\\"))
                    {
                        stream = GetStreamFromFile(myLocation.Substring(@"file:\\".Length));
                    }
                    else
                    {
                        _logger.Log(Level.SEVERE, "Location does not start with file:\\\\.");
                        return new QueryResult("", PluginShortName, 0L, ResultType.Failed, GetResult(), new InvalidImportLocationException(myLocation, @"file:\\"));
                    }

                    #region Start import using the AGraphDBImport implementation and return the result

                    return Import();

                    #endregion
                }
                catch (Exception ex)
                {
                    //if something unexpected happens we log it and return a query result with failed.
                    _logger.Log(Level.SEVERE, "Exception thrown:\n", ex);
                    return new QueryResult("", PluginShortName, 0L, ResultType.Failed, GetResult(), new ImportFailedException(ex));
                }
                finally
                {
                    if (stream != null)
                    {
                        _logger.Log(Level.FINE, "Stream closed");
                        stream.Dispose();
                    }
                }
            }
        }

        #endregion

        #region private helper

        /// <summary>
        /// Reads a file
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private Stream GetStreamFromFile(String location)
        {
            _logger.Log(Level.FINE, "Try to open from: {0}", location);
            var filestream = File.Open(location, FileMode.Open, FileAccess.Read, FileShare.Read);

            return filestream;
        }

        private QueryResult Import()
        {
            Stopwatch sw = Stopwatch.StartNew();

            //The resulting FS via reflection from the graphfs
            _resultingFS = (IGraphFS) typeof(SonesGraphDB).GetField("_iGraphFS", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_db);

            //insert into both filesystems...
            FillFilesystems();
            _logger.Log(Level.INFO, "filesystems filled.");

            sw.Stop();

            _logger.Log(Level.INFO, "Import finished.");

            return new QueryResult("", PluginShortName, Convert.ToUInt64(sw.Elapsed.TotalMinutes), ResultType.Successful, GetResult());
        }

        private IEnumerable<VertexView> GetResult()
        {
            return new VertexView(new Dictionary<string, object> { { "Number of import statements:", _currentImport } }, null).SingleEnumerable();
        }

        private void FillFilesystems()
        {
            _logger.Log(Level.FINE, "FillFilesystems started.");

            var createdVertexTypes = new List<IVertexType>();
            try
            {

                using (_reader = new XmlTextReader(stream))
                {
                    try
                    {
                        //we move to the first element in this file. 
                        _reader.MoveToContent();

                        //extract data from xml... (types, import)
                        ExecuteBulkInsert(_reader.ReadSubtree());
                    }
                    finally
                    {
                        if (!_closed)
                            _reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(Level.SEVERE, "Exception thrown: {0}", ex);
            }
            finally
            {
                _logger.Log(Level.FINE, "FillFilesystems finished.");
            }
        }

        private void ExecuteBulkInsert(XmlReader readerBulkInsert)
        {
            _logger.Log(Level.FINE, "ExecuteBulkInsert started.");

            //if no xml is available, we have nothing to do
            if (readerBulkInsert == null)
                return;

            int phase = 0;
            try
            {
                //move to the first Element
                readerBulkInsert.MoveToContent();

                //should be a BulkInsert element
                if (!CheckIsBulkInsertElement(readerBulkInsert))
                    //TODO: log something;
                    return;

                //now we are on the first node of bulk insert or at the end
                try
                {
                    while (!_closed && readerBulkInsert.Read())
                    {
                        if (CheckIsElement(readerBulkInsert))
                        {
                            var nextReader = readerBulkInsert.ReadSubtree();

                            switch (readerBulkInsert.Name)
                            {
                                case ImportTag:
                                    if (phase > 1)
                                        //TODO: log
                                        ;

                                    if (phase == 0)
                                        GetVertexTypes();

                                    phase = 1;
                                    ExecuteImport(nextReader);
                                    _logger.Log(Level.INFO, "Finished one Import tag.");
                                    break;
                                case PrePhaseTag:
                                    if (phase > 0)
                                        //TODO: log 
                                        ;

                                    phase = 0;
                                    ExecuteTypesPhase(nextReader);
                                    break;
                                case PostPhaseTag:
                                    if (phase == 1)
                                        InsertIncomingEdges();
                                    phase = 2;
                                    //TODO: implement post phase
                                    break;
                                default:
                                    //TODO: log something
                                    break;
                            }
                        }

                    }
                    if (phase == 1)
                        InsertIncomingEdges();
                }
                finally
                {
                    if (!_closed)
                        readerBulkInsert.Close();
                }

            }
            finally
            {
               _logger.Log(Level.FINE, "ExecuteBulkInsert finished.");
            }

        }

        private void InsertIncomingEdges()
        {
            var incomingEdges = _sorter.GetSorted().GetEnumerator();

            List<IncomingEdge> toAdd = new List<IncomingEdge>();
            var hasNext = incomingEdges.MoveNext();
            long count = 0L;

            while (hasNext)
            {
                toAdd.Add(incomingEdges.Current);

                while (hasNext)
                {
                    hasNext = incomingEdges.MoveNext();
                    if (hasNext &&
                        incomingEdges.Current.TargetVertexTypeID == toAdd[0].TargetVertexTypeID &&
                        incomingEdges.Current.TargetVertexID == toAdd[0].TargetVertexID)
                    {
                        toAdd.Add(incomingEdges.Current);
                    }
                    else
                    {
                        break;
                    }
                }
                try
                {
                    _resultingFS.UpdateVertex(_security, _transaction, toAdd[0].TargetVertexID, toAdd[0].TargetVertexTypeID, ConvertIncomingEdgesToUpdateDefinition(toAdd), false);
                }
                catch (Exception ex)
                {
#if DEBUG
                    for (var current = ex; current != null; current = current.InnerException)
                    {
                        _logger.Log(Level.SEVERE, "An exception was thrown:{0}\n{1}", current.GetType().FullName, current.StackTrace.ToString());
                    }
#else
                    _logger.Log(Level.SEVERE, "An exception was thrown:\n{0}", ex);
#endif

                }
                toAdd.Clear();
                count++;

                if (count > 0 && count % 500 == 0)
                {
                    _logger.Log(Level.INFO, "Edges added: {0}", count);
                }
            }
        }

        private VertexUpdateDefinition ConvertIncomingEdgesToUpdateDefinition(List<IncomingEdge> toAdd)
        {
            return new VertexUpdateDefinition(
                myToBeAddedIncomingEdges: toAdd.Select(_ => new IncomingEdgeAddDefinition(
                    _.SourceVertexTypeID, 
                    _.PropertyID, 
                    _.SourceVertexID.SingleEnumerable())));
        }

        /// <summary>
        /// gets the current type information and stores them in the coresponding fields.
        /// </summary>
        private void GetVertexTypes()
        {
            _logger.Log(Level.FINE, "GetVertexTypes started.");

            var info = _db.GetAllVertexTypes(_security, _transaction, new RequestGetAllVertexTypes(), ConvertVertexTypes);
            _edgeProps = _db.GetAllEdgeTypes(_security, _transaction, new RequestGetAllEdgeTypes(), ConvertEdgeTypes);

            _vertextypes = info.Item1;
            _propertyTypes = info.Item2;
            _edgeTypes = info.Item3;

            if (_vertextypes.Count == 0)
                throw new XMLBulkImportPluginException("There are no user defined non abstract vertex types that can be filled..");

            _logger.Log(Level.FINE, "GetVertexTypes finished.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerTypes"></param>
        private void ExecuteTypesPhase(XmlReader readerTypes)
        {
            _logger.Log(Level.FINE, "ExecuteTypesPhase started.");

            //if no xml is available, we have nothing to do
            if (readerTypes == null)
                return;

            try
            {
                //move to the first Element
                readerTypes.MoveToContent();

                //should be a BulkInsert element
                if (!CheckTypesElement(readerTypes))
                    //TODO: log something;
                    return;

                while (!_closed && readerTypes.Read())
                {
                    if (CheckIsElement(readerTypes))
                    {
                        var nextReader = readerTypes.ReadSubtree();

                        switch (readerTypes.Name)
                        {
                            /*                            case "GQL":
                                                            ExecuteGql(nextReader);
                                                            break;
                                                        case "CreateType":
                                                            //log not implemented yet
                                                            break;
                            */

                            default:
                                //TODO: log something
                                break;

                        }
                    }
                }
            }
            finally
            {
                if (!_closed)
                    readerTypes.Close();

                _logger.Log(Level.FINE, "ExecuteTypesPhase finished.");
            }
            
        }

        #region execution methods



        /// <summary>
        /// The entry point for the execution of an &lt;Import&gt; tag.
        /// </summary>
        /// <param name="myImportReader">An XmlReader, that is positioned before an &lt;Import&gt; tag.</param>
        private void ExecuteImport(XmlReader myImportReader)
        {
            _logger.Log(Level.FINE, "ExecuteImport started.");

            // if no xml is available, we have nothing to do.
            if (myImportReader == null)
                return;

            try
            {
                myImportReader.MoveToContent();

                if (!CheckIsImportElement(myImportReader))
                {
                    _logger.Log(Level.WARNING, "ExecuteImport expects 'Import' element, but was {0}.", myImportReader.LocalName);
                    return;
                }

                while (!_closed && myImportReader.Read())
                {
                    if (CheckIsElement(myImportReader))
                    {
                        var nextReader = myImportReader.ReadSubtree();

                        if (_offset.HasValue && _offset.Value > 0)
                        {
                            _logger.Log(Level.FINE, "Skipped one statement of import because offset is not reached.");

                            _offset = _offset.Value - 1;
                            nextReader.Close();
                        }
                        else
                        {
                            switch (myImportReader.LocalName)
                            {
                                case InsertTag:
                                    ExecuteInsert(nextReader);
                                    _currentImport++;
                                    break;
/*                                case "Gql":
                                    ExecuteGql(nextReader);
                                    _currentImport++;
                                    break;
*/
                                default:
                                    _logger.Log(Level.WARNING, "Unknown xml tag {0},", myImportReader.LocalName);
                                    break;
                            }
                        }
                        if (_currentImport > 0 && _currentImport % 500 == 0 )
                        {

                            _logger.Log(Level.INFO, "Imports executed: {0}", _currentImport);                            
                            
                            if (_resultingFS.IsPersistent)
                            {
                                var free = _resultingFS.GetNumberOfFreeBytes();
                                _logger.Log(Level.INFO, "Number of free bytes: {0}", free);

                                if (free < 1073741824L && free > 0) //1GB
                                {
                                    _logger.Log(Level.WARNING, String.Format("Finished import because file system has less than 1 GB free ({0}B).", free));
                                    _closed = true;
                                    return;
                                }
                                if (File.Exists("semaphore"))
                                {
                                    _logger.Log(Level.WARNING, "Finished import because semphore file was found.");
                                    _closed = true;
                                    return;
                                }
                            }

                        }
                        if (_limit.HasValue && _currentImport >= _limit.Value)
                        {
                            _logger.Log(Level.FINE, "Finished import because limit was exceeded.");
                            _closed = true;
                            return;
                        }
                    }
                }
            }
            finally
            {
                if (!_closed)
                    myImportReader.Close();
                _logger.Log(Level.FINE, "ExecuteImport finished.");
            }

        }

/*
        /// <summary>
        /// The entry point for the execution of an &lt;GQL&gt; tag.
        /// </summary>
        /// <param name="myGQLReader">An XmlReader, that is positioned before an &lt;GQL&gt; tag.</param>
        private void ExecuteGql(XmlReader myGQLReader)
        {
            _logger.Log(Level.FINE, "ExecuteGql started.");

            // if no xml is available, we have nothing to do.
            if (myGQLReader == null)
                //TODO: log something;
                return;

            try
            {
                myGQLReader.MoveToContent();

                if (!CheckIsGqlElement(myGQLReader))
                    return ;

                String gql = null;
                while (myGQLReader.MoveToNextAttribute())
                {
                    switch (myGQLReader.LocalName)
                    {
                        case "Query":
                            gql = myGQLReader.ReadContentAsString();
                            break;
                        default:
                            //TODO: log something
                            break;

                    }
                }

                if (gql == null)
                    //TODO log something;
                    return;

                var result = _ql.Query(_security, _transaction, gql);

            }
            finally
            {
                if (!_closed)
                    myGQLReader.Close();

                _logger.Log(Level.FINE, "ExecuteGql finished.");
            }
        }
  
 

        /// <summary>
        /// Logs the result of the import as Info.
        /// </summary>
        /// <param name="myResult"></param>
        private void logQueryResult(QueryResult myResult)
        {

            throw new NotImplementedException();
        }
        */

        /// <summary>
        /// The entry point for the execution of an &lt;Insert&gt; tag.
        /// </summary>
        /// <param name="myInsertReader">An XmlReader, that is positioned before an &lt;Insert&gt; tag.</param>
        private void ExecuteInsert(XmlReader myInsertReader)
        {
            _logger.Log(Level.FINE, "ExecuteInsert started.");

            // if no xml is available, we have nothing to do.
            if (myInsertReader == null)
                //TODO: log something;
                return;

            try
            {
                myInsertReader.MoveToContent();

                if (!CheckIsInsertElement(myInsertReader))
                    return;

                String vertexTypeName = null;
                long? id = null;
                while (myInsertReader.MoveToNextAttribute())
                {
                    switch (myInsertReader.LocalName)
                    {
                        case VertexTypeAttribute:
                            vertexTypeName = myInsertReader.ReadContentAsString();
                            break;
                        case VertexIDAttribute:
                            id = myInsertReader.ReadContentAsLong();
                            break;
                        default:
                            //TODO: log something
                            break;

                    }
                }

                if (vertexTypeName == null)
                    //TODO: log something;
                    return;

                if (!id.HasValue)
                    //TODO: log something;
                    return;

                IDictionary<String, String> values = new Dictionary<String, String>();
                List<SingleLink> singleLinks = new List<SingleLink>();
                List<MultiLink> multiLinks = new List<MultiLink>();

                while (!_closed && myInsertReader.Read())
                {
                    if (CheckIsElement(myInsertReader))
                    {
                        var nextReader = myInsertReader.ReadSubtree();

                        switch (myInsertReader.LocalName)
                        {
                            case SetValueTag:
                                var value = ExecuteSetValue(nextReader);
                                if (value.HasValue)
                                    values.Add(value.Value);

                                break;
                            case SingleLinkTag:
                                var singleLink = ExecuteSingleLink(nextReader);
                                if (singleLink != null)
                                    singleLinks.Add(singleLink);

                                break;
                            case MultiLinkTag:
                                var multiLink = ExecuteMultiLink(nextReader);
                                if (multiLink != null)
                                    multiLinks.Add(multiLink);

                                break;
                            default:
                                //TODO: log something
                                break;
                        }
                    }

                }

                var vertextype = RetrieveVertexType(vertexTypeName);

                var vertexTypeID = vertextype.ID;
                var vertexID = id.Value;

                var edition = ExtractEdition(values);
                var comment = ExtractComment(values);
                var creation = ExtractCreationDate(values);
                var modification = ExtractModificationDate(values);
                var structured = ConvertStructuredProperties(values, vertextype);
                var unstructured = ConvertUnstructuredProperties(values);

                var source = new VertexInformation(vertexTypeID, vertexID);
                var hyper = ConvertOutgoingEdges(multiLinks, vertextype, source);
                var single = ConvertSingleEdges(singleLinks, vertextype, source);

                var forResultingFS = new VertexAddDefinition(vertexID, vertexTypeID, edition, hyper, single, null, null, comment, creation, modification, structured, unstructured);

                AddEdgesToSorter(hyper, single);

                _resultingFS.AddVertex(_security, _transaction, forResultingFS, myCreateAutoIncomingEdges: false);

            }
            catch (Exception ex)
            {
                IXmlLineInfo info = myInsertReader as IXmlLineInfo;
                if (info != null && info.HasLineInfo())
                {
                    var line = info.LineNumber;
                    var pos = info.LinePosition;
                    _logger.Log(Level.INFO, "Exception thrown while reading line {0} position {1}.", line, pos);
                }

#if DEBUG
                for (var current = ex; current != null; current = current.InnerException)
                {
                    _logger.Log(Level.SEVERE, "An exception was thrown:{0}\n{1}", current.GetType().FullName, current.StackTrace.ToString());
                }

#else
                _logger.Log(Level.SEVERE, "An exception was thrown:\n{0}", ex);
#endif
            }
            finally
            {
                _logger.Log(Level.FINE, "ExecuteGql finished.");
                
                if (!_closed)
                    myInsertReader.Close();
            }

        }

        private void AddEdgesToSorter(IEnumerable<HyperEdgeAddDefinition> myHypers, IEnumerable<SingleEdgeAddDefinition> mySingles)
        {
            foreach (var hyper in myHypers)
            {
                foreach (var single in hyper.ContainedSingleEdges)
                {
                    _sorter.Add(single.SourceVertexInformation, single.TargetVertexInformation, hyper.PropertyID);
                }
            }

            foreach (var single in mySingles)
            {
                _sorter.Add(single.SourceVertexInformation, single.TargetVertexInformation, single.PropertyID);
            }
        }

        /// <summary>
        /// The entry point for the execution of an &lt;SingleLink&gt; tag.
        /// </summary>
        /// <param name="mySingleLinkReader">An XmlReader, that is positioned before an &lt;SingleLink&gt; tag.</param>
        /// <returns>
        /// An instance of SingleLink that contains the information of the  &lt;SingleLink&gt; tag or <c>Null</c> if the tag contains no information.
        /// </returns>
        private SingleLink ExecuteSingleLink(XmlReader mySingleLinkReader)
        {
            _logger.Log(Level.FINE, "ExecuteSingleLink started");

            // if no xml is available, we have nothing to do.
            if (mySingleLinkReader == null)
                return null;

            try
            {
                mySingleLinkReader.MoveToContent();

                if (!CheckIsSingleLink(mySingleLinkReader))
                    //TODO: log something;
                    return null;

                String key = null;
                while (mySingleLinkReader.MoveToNextAttribute())
                {
                    switch (mySingleLinkReader.LocalName)
                    {
                        case AttributeNameAttribute:
                            key = mySingleLinkReader.ReadContentAsString();
                            break;
                        default:
                            //TODO: log something
                            break;

                    }
                }

                if (key == null)
                    //TODO: log something;
                    return null;

                List<Link> links = new List<Link>();
                while (!_closed && mySingleLinkReader.Read())
                {
                    if (CheckIsElement(mySingleLinkReader))
                    {
                        var nextReader = mySingleLinkReader.ReadSubtree();

                        switch (mySingleLinkReader.LocalName)
                        {
                            case LinkTag:
                                var value = ExecuteLink(nextReader);
                                if (value != null)
                                    links.Add(value);

                                break;
                            default:
                                //TODO: log something
                                break;
                        }
                    }

                }

                if (links.Count > 0)
                {
                    if (links.Count > 1)
                        //TODO log something
                        ;

                    return new SingleLink(key, links.First());
                }

                return null;

            }
            finally
            {
                if (!_closed)
                    mySingleLinkReader.Close();
                _logger.Log(Level.FINE, "ExecuteSingleLink finished.");
            }

        }

        /// <summary>
        /// The entry point for the execution of an &lt;MultiLink&gt; tag.
        /// </summary>
        /// <param name="myMultiLinkReader">An XmlReader, that is positioned before an &lt;MultiLink&gt; tag.</param>
        /// <returns>
        /// An instance of MultiLink that contains the information of the  &lt;MultiLink&gt; tag or <c>Null</c> if the tag contains no information.
        /// </returns>
        private MultiLink ExecuteMultiLink(XmlReader myMultiLinkReader)
        {
            _logger.Log(Level.FINE, "ExecuteMultiLink started.");

            // if no xml is available, we have nothing to do.
            if (myMultiLinkReader == null)
                return null;

            try
            {
                myMultiLinkReader.MoveToContent();

                if (!CheckIsImportElement(myMultiLinkReader))
                    //TODO: log something;
                    return null;

                String key = null;
                while (myMultiLinkReader.MoveToNextAttribute())
                {
                    switch (myMultiLinkReader.LocalName)
                    {
                        case AttributeNameAttribute:
                            key = myMultiLinkReader.ReadContentAsString();
                            break;
                        default:
                            //TODO: log something
                            break;

                    }
                }

                if (key == null)
                    //TODO: log something;
                    return null;

                List<Link> links = new List<Link>();
                while (!_closed && myMultiLinkReader.Read())
                {
                    if (CheckIsElement(myMultiLinkReader))
                    {
                        var nextReader = myMultiLinkReader.ReadSubtree();

                        switch (myMultiLinkReader.LocalName)
                        {
                            case LinkTag:
                                var value = ExecuteLink(nextReader);
                                if (value != null)
                                    links.Add(value);

                                break;
                            default:
                                //TODO: log something
                                break;
                        }
                    }

                }


                return new MultiLink(key, links);
            }
            finally
            {
                if (!_closed)
                    myMultiLinkReader.Close();
                _logger.Log(Level.FINE, "ExecuteMultiLink finished.");

            }
        }

        /// <summary>
        /// The entry point for the execution of an &lt;Link&gt; tag.
        /// </summary>
        /// <param name="myLinkReader">An XmlReader, that is positioned before an &lt;Link&gt; tag.</param>
        /// <returns>
        /// An instance of Link that contains the information of the &lt;Link&gt; tag or <c>Null</c> if the tag contains no information.
        /// </returns>
        private Link ExecuteLink(XmlReader myLinkReader)
        {
            _logger.Log(Level.FINE, "ExecuteLink started.");

            // if no xml is available, we have nothing to do.
            if (myLinkReader == null)
                //TODO: log something;
                return null;

            try
            {
                myLinkReader.MoveToContent();

                if (!CheckIsLinkElement(myLinkReader))
                    return null;

                String vertexType = null;
                long? id = null;
                while (myLinkReader.MoveToNextAttribute())
                {
                    switch (myLinkReader.LocalName)
                    {
                        case VertexTypeAttribute:
                            vertexType = myLinkReader.ReadContentAsString();
                            break;
                        case VertexIDAttribute:
                            try
                            {
                                id = myLinkReader.ReadContentAsLong();
                                break;
                            }
                            catch (Exception e)
                            {
                                //TODO: log something;
                                return null;
                            }
                        default:
                            //TODO: log something
                            break;

                    }
                }

                if (vertexType == null)
                    //TODO: log something;
                    return null;
                if (!id.HasValue)
                    //TODO: log something;
                    return null;

                IDictionary<String, String> values = new Dictionary<String, String>();

                while (!_closed && myLinkReader.Read())
                {
                    if (CheckIsElement(myLinkReader))
                    {
                        var nextReader = myLinkReader.ReadSubtree();

                        switch (myLinkReader.LocalName)
                        {
                            case SetValueTag:
                                var value = ExecuteSetValue(nextReader);
                                if (value.HasValue)
                                    values.Add(value.Value);

                                break;
                            default:
                                //TODO: log something
                                break;
                        }
                    }

                }

                return new Link(new VertexID(vertexType, id.Value), values);

            }
            finally
            {
                if (!_closed)
                    myLinkReader.Close();
                _logger.Log(Level.FINE, "ExecuteLink finished.");
            }
        }

        /// <summary>
        /// The entry point for the execution of an &lt;SetValue&gt; tag.
        /// </summary>
        /// <param name="mySetValueReader">An XmlReader, that is positioned before an &lt;SetValue&gt; tag.</param>
        /// <returns>
        /// An instance of Link that contains the information of the &lt;SetValue&gt; tag or <c>Null</c> if the tag contains no information.
        /// </returns>
        private KeyValuePair<String, String>? ExecuteSetValue(XmlReader mySetValueReader)
        {
            _logger.Log(Level.FINE, "ExecuteSetValue started.");
            // if no xml is available, we have nothing to do.
            if (mySetValueReader == null)
                //TODO: log something;
                return null;

            try
            {
                mySetValueReader.MoveToContent();

                if (!CheckIsSetValueElement(mySetValueReader))
                    return null;

                String key = null;
                String value = null;
                while (mySetValueReader.MoveToNextAttribute())
                {
                    switch (mySetValueReader.LocalName)
                    {
                        case AttributeNameAttribute:
                            key = mySetValueReader.ReadContentAsString();
                            break;
                        case ValueAttribute:
                            value = DecodeBase64(mySetValueReader.ReadContentAsString());
                            break;
                        default:
                            //TODO: log something
                            break;

                    }
                }

                if (key == null)
                    //TODO: log something;
                    return null;
                if (value == null)
                    //TODO: log something;
                    return null;

                return new KeyValuePair<string, string>(key, value);
            }
            finally
            {
                if (!_closed)
                    mySetValueReader.Close();
                _logger.Log(Level.FINE, "ExecuteSetValue finished.");
            }

        }

        #endregion

        /// <summary>
        /// Converts an enumerable of MultiLink into an enumerable of HyperEdgeAddDefinition.
        /// </summary>
        /// <param name="myMultiLinks">The enumerable of MultiLink instances that will be converted.</param>
        /// <param name="myVertextype">The vertex type, that is used to get the property id.</param>
        /// <param name="mySource">The vertex information of the source vertex.</param>
        /// <returns>An enumerable of HyperEdgeAddDefinition one definition per MultiLink instance.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="myMultiLinks"/> or <paramref name="myVertextype"/> is <c>Null</c>.</exception>
        private IEnumerable<HyperEdgeAddDefinition> ConvertOutgoingEdges(List<MultiLink> myMultiLinks, InternVertexType myVertextype, VertexInformation mySource)
        {

            List<HyperEdgeAddDefinition> result = new List<HyperEdgeAddDefinition>();

            foreach (var multiLink in myMultiLinks)
            {

                var propID = myVertextype.Attributes[multiLink.Key];

                var date = DateTime.UtcNow.ToBinary();

                var add = new HyperEdgeAddDefinition(propID, _edgeTypes[propID], mySource, multiLink.Links.Select(_ => ConvertLink(_, propID, mySource)), null, date, date, null, null);
                result.Add(add);

            }

            return result;
        }

        /// <summary>
        /// Converts an enumerable of SingleLink into an enumerable of SingleEdgeAddDefinition.
        /// </summary>
        /// <param name="mySingleLinks">The enumerable of SingleLink instances to be converted.</param>
        /// <param name="myVertextype">The vertex type, that is used to get the property id.</param>
        /// <param name="mySource">The vertex information of the source vertex.</param>
        /// <returns>An enumerable of SingleEdgeAddDefinition one definition per SingleLink instance.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="mySingleLinks"/> or <paramref name="myVertextype"/> is <c>Null</c>.</exception>
        private IEnumerable<SingleEdgeAddDefinition> ConvertSingleEdges(List<SingleLink> mySingleLinks, InternVertexType myVertextype, VertexInformation mySource)
        {
            List<SingleEdgeAddDefinition> result = new List<SingleEdgeAddDefinition>();

            foreach (var singleLink in mySingleLinks)
            {
                
                var propID = myVertextype.Attributes[singleLink.Key];
                result.Add(ConvertLink(singleLink.Link, propID, mySource));

            }

            return result;
            
        }

        /// <summary>
        /// Converts an instance of Link into an instance of SingleEdgeAddDefinition.
        /// </summary>
        /// <param name="myLink">An instance of Link that will be converted.</param>
        /// <param name="myPropertyID">The property id of the outgoing edge attribute.</param>
        /// <param name="mySource">The vertex information of the source vertex.</param>
        /// <returns>An instance of SingleEdgeAddDefinition.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="myLink"/> is <c>Null</c>.</exception>
        private SingleEdgeAddDefinition ConvertLink(Link myLink, long myPropertyID, VertexInformation mySource)
        {

            var target = ConvertVertexID(myLink.TargetID);

            var comment = ExtractComment(myLink.Values);
            var creation = ExtractCreationDate(myLink.Values);
            var modification = ExtractModificationDate(myLink.Values);

            return new SingleEdgeAddDefinition(myPropertyID, _edgeTypes[myPropertyID], mySource, target, comment, creation, modification, null, null);
        }

        /// <summary>
        /// Converts a instance of VertexID into an instance of VertexInformation.
        /// </summary>
        /// <param name="myVertexID">An instance of VertexID that will be converted.</param>
        /// <returns>An instance of VertexInformation.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="myVertexID"/> is <c>Null</c>.</exception>
        private VertexInformation ConvertVertexID(VertexID myVertexID)
        {
            return new VertexInformation(RetrieveVertexType(myVertexID.VertexType).ID, myVertexID.ID);
        }

        /// <summary>
        /// Converts a dictionary string to string into string to object.
        /// </summary>
        /// <param name="myValues">An dictionary of string to string.</param>
        /// <returns>An dictionary of string to object.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="myValues"/> is <c>Null</c>.</exception>
        private IDictionary<String, object> ConvertUnstructuredProperties(IDictionary<string, string> myValues)
        {
            return myValues.ToDictionary(_=>_.Key, _=> (object)_.Value);
        }

        /// <summary>
        /// Converts a dictionary string to string into long to IComparable.
        /// </summary>
        /// <param name="myValues">An dictionary of string to string.</param>
        /// <param name="myVertexType">The vertex type, that is needed to get the attribute ids of properties.</param>
        /// <returns>An dictionary of long to IComparable.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="myValues"/> or <paramref name="myVertexType"/>is <c>Null</c>.</exception>
        private IDictionary<long, IComparable> ConvertStructuredProperties(IDictionary<string, string> myValues, InternVertexType myVertexType)
        {
            Dictionary<long, IComparable> result = new Dictionary<long, IComparable>();
            List<String> toBeDeleted = new List<String>();

            foreach (var value in myValues)
            {
                if (myVertexType.Attributes.ContainsKey(value.Key))
                {
                    var id = myVertexType.Attributes[value.Key];

                    IComparable newValue;

                    try
                    {
                        newValue = value.Value.ConvertToIComparable(_propertyTypes[id]);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(Level.SEVERE, "Exception thrown: {0}", e);
                        _logger.Log(Level.SEVERE, "Key: " + value.Key);
                        _logger.Log(Level.SEVERE, "Value: " + value.Value);
                        _logger.Log(Level.WARNING, String.Format("Could not convert {0} into {1} of vertex type {2} and property {3}", value.Value, _propertyTypes[id].Name, myVertexType.Name, value.Key));
                        continue;
                    }

                    result.Add(id, newValue);

                    toBeDeleted.Add(value.Key);
                    
                }
            }

            foreach (var delete in toBeDeleted)
            {
                myValues.Remove(delete);
            }

            return result;
        }

        /// <summary>
        /// Extracts the modification date from a dictionary of key-values.
        /// </summary>
        /// <param name="myValues">An dictionary of string to string. If this dictionary contains a modification date, the entry will be removed from this dictionary.</param>
        /// <returns>The modification date that was part of <paramref name="myValues"/> or <c>DateTime.UtcNow.ToBinary()</c>.</returns>
        /// <remarks>If <paramref name="myValues"/> contains a key that equals ModificationDate, this entry will be removed.</remarks>
        /// <exception cref="NullReferenceException">If <paramref name="myValues"/> is <c>Null</c>.</exception>
        private long ExtractModificationDate(IDictionary<string, string> myValues)
        {
            long result;
            if (myValues.ContainsKey(ModificationDate))
            {
                result = long.Parse(myValues[ModificationDate]);
                myValues.Remove(ModificationDate);
            }
            else
                result = DateTime.UtcNow.ToBinary();

            return result;
        }

        /// <summary>
        /// Extracts the creation date from a dictionary of key-values.
        /// </summary>
        /// <param name="myValues">An dictionary of string to string. If this dictionary contains a creation date, the entry will be removed from this dictionary.</param>
        /// <returns>The creation date that was part of <paramref name="myValues"/> or <c>DateTime.UtcNow.ToBinary()</c>.</returns>
        /// <remarks>If <paramref name="myValues"/> contains a key that equals CreationDate, this entry will be removed.</remarks>
        /// <exception cref="NullReferenceException">If <paramref name="myValues"/> is <c>Null</c>.</exception>
        private long ExtractCreationDate(IDictionary<string, string> myValues)
        {
            long result;
            if (myValues.ContainsKey(CreationDate))
            {
                result = long.Parse(myValues[CreationDate]);
                myValues.Remove(CreationDate);
            }
            else
                result =DateTime.UtcNow.ToBinary();

            return result;
        }

        /// <summary>
        /// Extracts the comment from a dictionary of key-values.
        /// </summary>
        /// <param name="myValues">An dictionary of string to string. If this dictionary contains a comment, the entry will be removed from this dictionary.</param>
        /// <returns>The comment that was part of <paramref name="myValues"/> or <c>Null</c>, if not.</returns>
        /// <remarks>If <paramref name="myValues"/> contains a key that equals Comment, this entry will be removed.</remarks>
        /// <exception cref="NullReferenceException">If <paramref name="myValues"/> is <c>Null</c>.</exception>
        private String ExtractComment(IDictionary<string, string> myValues)
        {
            String result = null;
            if (myValues.ContainsKey(Comment))
            {
                result = myValues[Comment];
                myValues.Remove(Comment);
            }

            return result;
        }

        /// <summary>
        /// Extracts the edition from a dictionary of key-values.
        /// </summary>
        /// <param name="myValues">An dictionary of string to string. If this dictionary contains a edition, the entry will be removed from this dictionary.</param>
        /// <returns>The edition that was part of <paramref name="myValues"/> or <c>Null</c>, if not.</returns>
        /// <remarks>If <paramref name="myValues"/> contains a key that equals Edition, this entry will be removed.</remarks>
        /// <exception cref="NullReferenceException">If <paramref name="myValues"/> is <c>Null</c>.</exception>
        private String ExtractEdition(IDictionary<string, string> myValues)
        {
            String result = null;
            if (myValues.ContainsKey(Edition))
            {
                result = myValues[Edition];
                myValues.Remove(Edition);
            }

            return result;
        }

        /// <summary>
        /// Converts a string into its base64 representation.
        /// </summary>
        /// <param name="myData">Any string.</param>
        /// <returns>The base64 representation of the given string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="myData"/> is <c>Null</c>.</exception>
        private string DecodeBase64(string myData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();

            byte[] todecode_byte = Convert.FromBase64String(myData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        /// <summary>
        /// Gets the InternVertexType instance that has the given name.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type, that should be returned.</param>
        /// <returns>An instance of InternVertexType, that contains the information for this vertex type.</returns>
        /// <exception cref="XMLBulkImportPluginException">If <paramref name="myVertexTypeName"/> id not part of _vertextypes. This means the vertex type was not created before.</exception>
        private InternVertexType RetrieveVertexType(string myVertexTypeName)
        {
            if (_vertextypes.ContainsKey(myVertexTypeName))
            {
                return _vertextypes[myVertexTypeName];
            }
            else
            {
                throw new XMLBulkImportPluginException(string.Format("The vertex type {0} is unknown.", myVertexTypeName));
            }
        }

        /// <summary>
        /// Creates a new instance of VertexUpdateDefinition.
        /// </summary>
        /// <param name="myIncomingEdges">The incoming edges that will be added.</param>
        /// <param name="myOutgoingHyperEdges">The outgoing hyperedges, that will be added.</param>
        /// <param name="myOutgoingSingleEdges">The outgoing singleedges, that will be added.</param>
        /// <returns>A new instance of VertexUpdateDefinition, that contains the given parameters.</returns>
        private VertexUpdateDefinition GenerateUpdateDefinition(IEnumerable<IncomingEdgeAddDefinition> myIncomingEdges, IDictionary<long, HyperEdgeUpdateDefinition> myOutgoingHyperEdges, IDictionary<long, SingleEdgeUpdateDefinition> myOutgoingSingleEdges)
        {
            return new VertexUpdateDefinition(mySingleEdgeUpdate: new SingleEdgeUpdate(myOutgoingSingleEdges), myHyperEdgeUpdate: new HyperEdgeUpdate(myOutgoingHyperEdges), myToBeAddedIncomingEdges: myIncomingEdges);
        }

        /// <summary>
        /// Converts ISingleEdge instances into SingleEdgeUpdateDefinitions.
        /// </summary>
        /// <param name="mySingleEdges"></param>
        /// <returns></returns>
        private IDictionary<Int64, SingleEdgeUpdateDefinition> ConvertSingleEdges(IEnumerable<Tuple<long, ISingleEdge>> mySingleEdges)
        {
            var result = new Dictionary<long, SingleEdgeUpdateDefinition>();

            if (mySingleEdges != null && mySingleEdges.Count() != 0)
            {
                foreach (var aSingleEdge in mySingleEdges)
                {
                    result.Add(aSingleEdge.Item1, CreateSingleEdgeUpdateDefinition(aSingleEdge.Item2));
                }
            }

            return result;
        }

        private SingleEdgeUpdateDefinition CreateSingleEdgeUpdateDefinition(ISingleEdge iSingleEdge)
        {
            var sourceVertex = iSingleEdge.GetSourceVertex();
            var targetVertex = iSingleEdge.GetTargetVertex();
            var structuredProperties = iSingleEdge.GetAllProperties();
            var unstructuredProperties = iSingleEdge.GetAllUnstructuredProperties();

            return new SingleEdgeUpdateDefinition(
                new VertexInformation(sourceVertex.VertexTypeID, sourceVertex.VertexID, sourceVertex.VertexRevisionID, sourceVertex.EditionName),
                new VertexInformation(targetVertex.VertexTypeID, targetVertex.VertexID, targetVertex.VertexRevisionID, targetVertex.EditionName),
                iSingleEdge.EdgeTypeID,
                iSingleEdge.Comment,
                new StructuredPropertiesUpdate(
                    structuredProperties != null && structuredProperties.Count() > 0 ? structuredProperties.ToDictionary(key => key.Item1, value => value.Item2) : null),
                new UnstructuredPropertiesUpdate(
                    unstructuredProperties != null && unstructuredProperties.Count() > 0 ? unstructuredProperties.ToDictionary(key => key.Item1, value => value.Item2) : null));
        }

        private HyperEdgeUpdateDefinition CreateHyperEdgeUpdateDefinition(IHyperEdge iHyperEdge)
        {
            var containedEdges = iHyperEdge.GetAllEdges();
            var structuredProperties = iHyperEdge.GetAllProperties();
            var unstructuredProperties = iHyperEdge.GetAllUnstructuredProperties();

            return new HyperEdgeUpdateDefinition(iHyperEdge.EdgeTypeID, iHyperEdge.Comment,
                new StructuredPropertiesUpdate(
                    structuredProperties != null && structuredProperties.Count() > 0 ? structuredProperties.ToDictionary(key => key.Item1, value => value.Item2) : null),
                new UnstructuredPropertiesUpdate(
                    unstructuredProperties != null && unstructuredProperties.Count() > 0 ? unstructuredProperties.ToDictionary(key => key.Item1, value => value.Item2) : null),
                null,
                containedEdges.Select(_ => CreateSingleEdgeUpdateDefinition(_)));
        }

        private IDictionary<Int64, HyperEdgeUpdateDefinition> GenerateHyperEdges(IEnumerable<Tuple<long, IHyperEdge>> myHyperEdges)
        {
            var result = new Dictionary<long, HyperEdgeUpdateDefinition>();

            if (myHyperEdges != null && myHyperEdges.Count() != 0)
            {
                foreach (var aHyperEdge in myHyperEdges)
                {
                    result.Add(aHyperEdge.Item1, CreateHyperEdgeUpdateDefinition(aHyperEdge.Item2));
                }
            }

            return result;
        }

        private IEnumerable<IncomingEdgeAddDefinition> GenerateIncomingEdgeDefinitions(IEnumerable<Tuple<long, long, IEnumerable<IVertex>>> myIncomingVertices)
        {
            if (myIncomingVertices != null && myIncomingVertices.Count() != 0)
            {
                List<IncomingEdgeAddDefinition> result = new List<IncomingEdgeAddDefinition>();

                foreach (var aIncomingEdge in myIncomingVertices)
                {
                    result.Add(new IncomingEdgeAddDefinition(aIncomingEdge.Item1, aIncomingEdge.Item2, aIncomingEdge.Item3.Select(_ => _.VertexID)));
                }

                return result;
            }
            else
            {
                return Enumerable.Empty<IncomingEdgeAddDefinition>();
            }
        }

        #endregion

        #region Converts

        private IDictionary<String, long> ConvertEdgeTypes(IRequestStatistics myRequestStatistics, IEnumerable<IEdgeType> myEdgeTypes)
        {
            return myEdgeTypes.SelectMany(_ => _.GetAttributeDefinitions(true)).Distinct().ToDictionary(_ => _.Name, _ => _.ID);
        }

        private Tuple<
            Dictionary<string, InternVertexType>,
            Dictionary<long, Type>,
            Dictionary<long, long>>
            ConvertVertexTypes(IRequestStatistics myRequestStatistics, IEnumerable<IVertexType> myVertexTypes)
        {
            var vertextypes = myVertexTypes
                .Where(_ => _.IsUserDefined && !_.IsAbstract)
                .Select(_ => new InternVertexType(
                    _.Name,
                    _.ID,
                    _.GetAttributeDefinitions(true)
                        .ToDictionary(myAttr => myAttr.Name, myAttr => myAttr.ID)))
                .ToDictionary(_ => _.Name);

            var attributetypes = myVertexTypes.SelectMany(_ => _.GetPropertyDefinitions(true)).Distinct().ToDictionary(_ => _.ID, _ => _.BaseType);

            var edgetypes = myVertexTypes
                .SelectMany(_ => _.GetOutgoingEdgeDefinitions(true))
                .Distinct()
                .ToDictionary(
                    _ => _.ID,
                    _ => (_.Multiplicity == EdgeMultiplicity.SingleEdge)
                            ? _.EdgeType.ID
                            : _.InnerEdgeType.ID);
            return Tuple.Create(vertextypes, attributetypes, edgetypes);
        }

        #endregion        

        #region Checks

        bool CheckIsElement(XmlReader myReader)
        {
            return myReader.NodeType == XmlNodeType.Element;
        }

        private bool CheckIsSingleLink(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsMultiLink(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }
/*
        private bool CheckIsGqlElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }
*/
        private bool CheckTypesElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsSetValueElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsLinkElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsInsertElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsBulkInsertElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsImportElement(XmlReader myReader)
        {
            return CheckIsElement(myReader);
        }

        private bool CheckIsBulkInsertElement(XmlTextReader myReader)
        {
            return CheckIsElement(myReader);
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.fastimport"; }
        }

        public string PluginShortName
        {
            get { return "FASTIMPORT"; }
        }

        public string PluginDescription
        {
            get { return "A class that represents a plugin for importing data as XML into IGraphDB"; }
        }
        
        public PluginParameters<Type> SetableParameters
        {
            get 
            {
                return new PluginParameters<Type>() { 
                    { LogLocation, typeof(string) } 
                };
            }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var path = "";
            if (myParameters != null && myParameters.ContainsKey(LogLocation))
                path = (string)myParameters[LogLocation];
            
            path = Path.Combine(path, ".fastimport_" + myUniqueString + ".log");
            
            return new XMLBulkImportPlugin(path);
        }

        public void Dispose()
        { }

        #endregion


    }
}
