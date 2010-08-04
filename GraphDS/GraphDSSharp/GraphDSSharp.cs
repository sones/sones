/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/*
 * GraphDSSharp
 * Achim Friedland, 2009 - 2010 
 */

#region Usings

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

using sones.GraphFS;
using sones.GraphFS.Caches;
using sones.GraphFS.Session;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Transactions;
using sones.GraphFS.DataStructures;

using sones.GraphDB;
using sones.GraphDB.Errors;
using sones.GraphDB.Transactions;
using sones.GraphDB.QueryLanguage.Result;

using sones.GraphDS.API.CSharp.Reflection;

using sones.Lib;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;

using sones.Notifications;
using System.Xml.Linq;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public class GraphDSSharp : AGraphDSSharp
    {

        public delegate void ShutdownEventHandler(Object Sender, EventArgs e);


        #region Data

        //protected String                    _GraphDB            = null;
        protected IGraphFS                  _IGraphFS           = null;
        private   Dictionary<String, Type>  _AssemblyTypes;
        private   String                    _RestURL            = null;

        public event ShutdownEventHandler ShutdownEvent;


        #endregion

        #region Properties

        public String GraphFSImplementation { get; set; }
        public String DatabaseName          { get; set; }
        public String Username              { get; set; }
        public String Password              { get; set; }
        public UInt64 FileSystemSize        { get; set; }
        
        public ObjectCacheSettings    ObjectCacheSettings    { get; set; }        
        public NotificationSettings   NotificationSettings   { get; set; }
        public NotificationDispatcher NotificationDispatcher { get; set; }

        #region StorageLocation

        public String StorageLocation
        {
            set
            {

                if (value == null)
                    throw new ArgumentNullException();

                _StorageLocations = new HashSet<String> { value };

            }
        }

        #endregion

        #region StorageLocations

        public HashSet<String> _StorageLocations; 

        public HashSet<String> StorageLocations
        {

            get
            {
                return _StorageLocations;
            }

            set
            {
                
                if (value == null)
                    throw new ArgumentNullException();

                _StorageLocations = value;

            }

        }

        #endregion

        #region IGraphFSSession

        protected IGraphDBSession _IGraphDBSession = null;

        public IGraphFSSession IGraphFSSession
        {
            get
            {
                return _IGraphFSSession;
            }
            set
            {
                _IGraphFSSession = value;
            }
        }

        #endregion

        #region IGraphDBSession

        protected IGraphFSSession _IGraphFSSession = null;

        public IGraphDBSession IGraphDBSession
        {
            get
            {
                return _IGraphDBSession;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region GraphDSSharp()

        public GraphDSSharp( )
        {
        }

        #endregion

        #region GraphDSSharp(myIGraphDBSession)

        public GraphDSSharp(IGraphDBSession myIGraphDBSession)
        {
            Connect(myIGraphDBSession);
        }

        public GraphDSSharp(IGraphDBSession myIGraphDBSession, String myAssembly)
            :this(myAssembly)
        {
            Connect(myIGraphDBSession);
        }

        public GraphDSSharp(String myAssembly)
        {

            _AssemblyTypes = new Dictionary<String, Type>();

            var assembly = Assembly.LoadFile(myAssembly);

            foreach (var _Type in assembly.GetTypes())
            {
                _AssemblyTypes.Add(_Type.Name, _Type);
            }

        }

        #endregion

        #endregion

        internal DBObject GetType(String myTypeName)
        {

            if (_AssemblyTypes.ContainsKey(myTypeName))
                return Activator.CreateInstance(_AssemblyTypes[myTypeName]) as DBVertex;

            return null;

        }

        #region Connection Management

        #region Connect(myIGraphDBSession)

        public Boolean Connect(IGraphDBSession myIGraphDBSession)
        {

            if (myIGraphDBSession == null)
                throw new ArgumentNullException();

            _IGraphDBSession = myIGraphDBSession;

            return true;

        }

        #endregion

        #region CreateDatabase(myOverwritte)

        /// <summary>
        /// Create a new database and overwritte a existing one
        /// </summary>
        /// <param name="myOverwritte"></param>
        /// <returns></returns>
        public Boolean CreateDatabase(Boolean myOverwritte)
        {
            return CreateDatabase(myOverwritte, null);
        }

        #endregion

        #region CreateDatabase(myOverwritte, myAction)

        public Boolean CreateDatabase(Boolean myOverwritte, Action<Double> myAction)
        {

            #region Create and open all StorageEngines

            //var _MKFSBufferSize = 1 * 1024 * 1024u;

            try
            {

                //var _IStorageEngines        = new List<IStorageEngine>();
                //var _StorageEnginesFactory  = new StorageEngineFactory();

                //foreach (var _StorageLocation in _StorageLocations)
                //{

                //    var _StorageEngine = _StorageEnginesFactory.CreateStorage(_StorageLocation, FileSystemSize, _MKFSBufferSize, myOverwritte, myAction);
                //    _StorageEngine.SealStorage();
                //    _StorageEngine.DetachStorage();

                //    _IStorageEngines.Add(_StorageEnginesFactory.OpenStorage(_StorageLocation));

                //}

            }

            catch
            {
                return false;
            }

            #endregion

            #region Make and mount the GraphFS

            try
            {

                #region If there are no StorageEngines => use TmpFS!

                if (_StorageLocations == null || _StorageLocations.Count == 0)
                {

                    _IGraphFS = new GraphFS1
                    {
                        ObjectCacheSettings    = ObjectCacheSettings,
                        NotificationSettings   = NotificationSettings,
                        NotificationDispatcher = NotificationDispatcher
                    };

                    _IGraphFSSession = new GraphFSSession(_IGraphFS, Username);

                    _IGraphFSSession.MountFileSystem("TmpFS", AccessModeTypes.rw);

                }

                #endregion

                else
                {

                    #region Initialize the requested GraphFS implementation

                    GraphFSFactory.Instance.ActivateIGraphFS(GraphFSImplementation).
                        SuccessAction<IGraphFS>(e => _IGraphFS = e.Value).
                        FailedAction<IGraphFS>(e => { throw new GraphFSException(e.Errors.First().Message); });
    
                    //switch (GraphFSImplementation)
                    //{

                    //    case "GraphFS3":
                            
                            //_IGraphFS = new OnDiscFS3()
                            //                {
                            //                    ObjectCacheSettings    = ObjectCacheSettings,
                            //                    NotificationSettings   = NotificationSettings,
                            //                    NotificationDispatcher = NotificationDispatcher
                            //                };
                    //        break;


                    //    default:
                    //        _IGraphFS = new OnDiscFS3()
                    //                        {
                    //                            ObjectCacheSettings    = ObjectCacheSettings,
                    //                            NotificationSettings   = NotificationSettings,
                    //                            NotificationDispatcher = NotificationDispatcher
                    //                        };
                    //        break;

                    //}

                    #endregion

                    //if (GraphFSImplementation == "GraphFS3")
                    //    _IGraphFS = new GraphFS3()
                    //    {
                    //        ObjectCacheSettings  = ObjectCacheSettings,
                    //        NotificationSettings = NotificationSettings
                    //    };

                    //else
                    //    _IGraphFS = new GraphFS2()
                    //    {
                    //        ObjectCacheSettings  = ObjectCacheSettings,
                    //        NotificationSettings = NotificationSettings
                    //    };

                    _IGraphFSSession = new GraphFSSession(_IGraphFS, Username);

                    if (myOverwritte)
                        _IGraphFSSession.MakeFileSystem(_StorageLocations.OrderBy(x => x).ToList()[0], "GraphFS for database " + DatabaseName, FileSystemSize, true, myAction);

                    _IGraphFSSession.MountFileSystem(_StorageLocations.OrderBy(x => x).ToList()[0], AccessModeTypes.rw);
                
                }

            }

            catch (Exception e)
            {
                throw new GraphDSSharpException(e.Message);
            }

            #endregion

            try
            {
                _IGraphDBSession = new GraphDBSession(new GraphDB2(new UUID(), new ObjectLocation(DatabaseName), _IGraphFSSession, true), Username);
            }

            catch (Exception e)
            {
                throw new GraphDSSharpException(e.Message);
            }

            return true;

        }

        #endregion

        #region OpenDatabase()

        public Boolean OpenDatabase()
        {

            try
            {

                #region If there are no StorageEngines => use TmpFS!

                if (_StorageLocations == null || _StorageLocations.Count == 0)
                {

                    _IGraphFS = new GraphFS1
                    {
                        ObjectCacheSettings  = ObjectCacheSettings,
                        NotificationSettings = NotificationSettings
                    };

                    _IGraphFSSession = new GraphFSSession(_IGraphFS, Username);

                    _IGraphFSSession.MountFileSystem("TmpFS", AccessModeTypes.rw);

                }

                #endregion

                else
                {

                    var fsException = GraphFSFactory.Instance.ActivateIGraphFS();

                    if (!fsException.Success)
                        throw new GraphDSSharpException(fsException.GetErrorsAsString());

                    _IGraphFS = fsException.Value;

                    _IGraphFS.ObjectCacheSettings = ObjectCacheSettings;
                    _IGraphFS.NotificationSettings = NotificationSettings;

                    _IGraphFSSession = new GraphFSSession(_IGraphFS, Username);

                    _IGraphFSSession.MountFileSystem(_StorageLocations.OrderBy(x => x).ToList()[0], AccessModeTypes.rw);


                    try
                    {
                        
                        _IGraphDBSession = new GraphDBSession(new GraphDB2(new UUID(), new ObjectLocation(DatabaseName), _IGraphFSSession, true), Username);

                    }

                    catch (Exception e)
                    {
                        throw new GraphDSSharpException(e.Message);
                    }

                }

            }

            catch (Exception e)
            {
                return false;
            }

            return true;

        }

        #endregion

        #region Shutdown()

        public Boolean Shutdown()
        {

            try
            {

                if (ShutdownEvent != null)
                    ShutdownEvent(this, EventArgs.Empty);

                _IGraphDBSession.Shutdown();
                _IGraphFSSession.UnmountAllFileSystems();

                _IGraphDBSession = null;
                _IGraphFSSession = null;

                //_IGraphFSSession.GetNotificationDispatcher().Dispose();

            }

            catch (Exception e)
            {
                return false;
            }

            return true;

        }

        #endregion

        #endregion



        #region Query(myQuery, myAction = null, mySuccessAction = null, myPartialSuccessAction = null, myFailureAction = null)

        public override QueryResult Query(String myQuery, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null)
        {

            QueryResult _QueryResult = null;

            // Use embedded reference
            if (_IGraphDBSession != null)
            {
                _QueryResult = _IGraphDBSession.Query(myQuery);
            }

            else if (!String.IsNullOrEmpty(_RestURL))
            {

                var request = (HttpWebRequest) WebRequest.Create(_RestURL + myQuery.ToBase64());
                var stream = new StreamReader(request.GetResponse().GetResponseStream());
                var result = stream.ReadToEnd();
                
                if (result.StartsWith("<string>"))
                    result = result.Substring("<string>".Length);

                if (result.EndsWith("</string>"))
                    result = result.Substring(0, result.Length - "</string>".Length);

                var readout = new DBObjectReadout();
                //readout.Attributes.Add("query", QueryXml(myQuery));
                readout.Attributes.Add("query", Query(myQuery));

                _QueryResult = new QueryResult(new SelectionResultSet(new List<DBObjectReadout>() { readout }));

            }

            else
                _QueryResult = new QueryResult(new Error_NotImplemented(new StackTrace(true)));

            QueryResultAction(_QueryResult, myAction, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion


        public override SelectToObjectGraph QuerySelect(String myQuery)
        {
            return new SelectToObjectGraph(Query(myQuery));
        }


        #region BeginTransaction(myDistributed = false, myLongRunning = false, myIsolationLevel = IsolationLevel.Serializable, myName = "", myCreated = null)

        public override DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null)
        {
            return _IGraphDBSession.BeginTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, myCreated);
        }

        #endregion

    }

}
