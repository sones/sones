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

/* <id name="sones GraphDB – Main Database Code" />
 * <copyright file="PandoraDatabase.cs"
 *            company="sones GmbH">
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using sones.GraphFS.Session;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;

using sones.GraphDB.Query;
using sones.GraphDB.Errors;
using sones.GraphDB.Settings;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.DataStructures.Settings;

using sones.Notifications;
using sones.Lib.DataStructures;

using sones.Lib.NewFastSerializer;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.Lib.Session;
using sones.GraphDB.Transactions;
using sones.GraphFS.Transactions;
using GraphFSInterface.Transactions;
using sones.GraphDB.Managers;


#endregion

namespace sones.GraphDB
{
    /// <summary>
    /// This is the main class for the Pandora Database Library and one instance of the Pandora Database.
    /// </summary>
    public class GraphDB2 
    {

        #region Data

        private DBInstanceSettingsManager<InstanceSettings> _InstanceSettingsManager = null;
        //private ListOfStringsObject _ListOfObjectSchemes = null;

        //private DBContext _DBContext = null;
        private ObjectLocation _DatabaseRootPath = null;
        private IGraphFSSession _IGraphFSSession = null;

        private UUID _internalDatabaseUUID = null;
        private QueryManager _QueryManager = null;
        private EntityUUID _internalUserID = null;
        private Dictionary<string, ADBSettingsBase> _DBSettings = null;

        DBTransactionManager _DBTransactionManager;

        #endregion

        #region Accessors

        public ObjectLocation DatabaseRootPath
        {
            get { return _DatabaseRootPath; }
        }

        public Dictionary<string, ADBSettingsBase> DBSettings
        {
            get { return _DBSettings; }
        }

        //public ListOfStringsObject ObjectSchemes
        //{
        //    get { return _ListOfObjectSchemes; }
        //}

        public IGraphFSSession GraphFSSession
        {
            get { return _IGraphFSSession; }
        }
        #endregion

        public GraphDB2(UUID myDatabaseInstanceUUID, ObjectLocation myDatabaseRootPath, IGraphFSSession myPandoraFS, Boolean myCreateNewIfNotExisting)
            : this(myDatabaseInstanceUUID, myDatabaseRootPath, myPandoraFS, myCreateNewIfNotExisting, true)
        {
        }

        /// <summary>
        /// Constructor for the Pandora Database Instance Manager
        /// </summary>
        /// <param name="myDatabaseInstanceUUID">unique database id</param>
        /// <param name="DatabaseRootPath">where is the database - a path inside the Pandora Filessystem, including the name of the database like "/database1"</param>
        /// <param name="PandoraVFSInstance">Pandora Virtual myIPandoraFS Instance</param>
        /// <param name="CreateNewIfNotExisting">if true an empty database scheme and settings structure will be established at the given root path</param>
        public GraphDB2(UUID myDatabaseInstanceUUID, ObjectLocation myDatabaseRootPath, IGraphFSSession myPandoraFS, Boolean myCreateNewIfNotExisting, Boolean myRebuildIndices)
        {

            #region Data

            this._DatabaseRootPath = myDatabaseRootPath;
            this._IGraphFSSession = myPandoraFS;
            this._internalDatabaseUUID = myDatabaseInstanceUUID;
            this._internalUserID = new EntityUUID();
            this._internalUserID.Generate();

            #endregion

            #region Check if there's a RootPath already

            var isDirectExcept = this._IGraphFSSession.isIDirectoryObject(this._DatabaseRootPath);

            if (isDirectExcept.Failed)
                throw new GraphDBException(isDirectExcept.Errors);

            if (isDirectExcept.Value != Trinary.TRUE)
            {
                if (myCreateNewIfNotExisting)
                {
                    // it's not there and we should create it...
                    var createDirExcept = this._IGraphFSSession.CreateDirectoryObject(this._DatabaseRootPath);

                    if (createDirExcept.Failed)
                        throw new GraphDBException(createDirExcept.Errors);
                }
                else
                {
                    throw new GraphDBException(new Error_DatabaseNotFound(this._DatabaseRootPath));
                }
            }

            #endregion

            #region Read the Database Instance Metadata

            _DBSettings                 = new Dictionary<string, ADBSettingsBase>();
            _InstanceSettingsManager    = new DBInstanceSettingsManager<InstanceSettings>(_DatabaseRootPath, _IGraphFSSession, myCreateNewIfNotExisting);
            //var listOfExceptions            = _IGraphFSSession.GetOrCreateObject<ListOfStringsObject>(new ObjectLocation(myDatabaseRootPath, DBConstants.DBTypeLocations), FSConstants.LISTOF_STRINGS, null, null, 0, false);

            //if (listOfExceptions.Failed)
            //    throw new GraphDBException(listOfExceptions.Errors);

            //_ListOfObjectSchemes        = listOfExceptions.Value;
            
            //_DBContext                = new DBContext(_IGraphFSSession, _DatabaseRootPath, _ListOfObjectSchemes, _internalUserID, _DBSettings, myRebuildIndices);

            _QueryManager               = new QueryManager();

            #endregion

            #region Notification System Startup

            _NotificationSettings = new NotificationSettings();
            StartDefaultNotificationDispatcher(this._InstanceSettingsManager.Content.Identifier);

            #endregion

            _DBTransactionManager = new DBTransactionManager();
        }

        /// <summary>
        /// Initiates the Shutdown of this Database Instance Manager
        /// </summary>
        public void Shutdown(SessionToken mySessionToken)
        {

            // Shutdown the notification dispatcher
            if (_NotificationDispatcher != null)
                _NotificationDispatcher.Dispose();

        }


        #region Public Methods

        #region Query(QueryScript, mySessionToken)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QueryScript"></param>
        /// <param name="graphDBSession">Needed for BeginTransaction inside any AStatementNode</param>
        /// <returns></returns>
        public QueryResult Query(String QueryScript, GraphDBSession graphDBSession)
        {

            #region Data

            QueryResult _QueryResult;

            #endregion

            /// As soon as the GetContent method of each statement needs no DBContext this methods should be removed and replaced by checking directly the parsed statement.
            /// But currently, we need to start a transaction for each query - but NOT for transaction related queries.
            if (isNonTransactionalQuery(QueryScript))
            {
                _QueryResult = this._QueryManager.ExecuteQuery(QueryScript, graphDBSession.GetDBContext(), graphDBSession);
            }
            else
            {

                // create a inner transaction for each statement
                // This is mandatory because the GetContent of each node need to read inside a transaction
                // This should be changed sometimes: 
                    //1) no query is allowed without a valid transaction
                    //2) GetContent does not get any DBContext, so we can create the transaction just for the execute and with that we know we need either a ReadOnly or Serializeable

                if (graphDBSession.GetLatestTransaction().IsRunning())
                {

                    #region Create a new transaction based in the existing transaction

                    var existingTransaction = graphDBSession.GetLatestTransaction();
                    using (var transaction = graphDBSession.BeginTransaction(existingTransaction.Distributed, existingTransaction.LongRunning, existingTransaction.IsolationLevel))
                    {
                        _QueryResult = this._QueryManager.ExecuteQuery(QueryScript, transaction.GetDBContext(), graphDBSession);
                        _QueryResult.AddErrorsAndWarnings(transaction.Commit());
                    }

                    #endregion

                }
                else
                {
                    using (var transaction = graphDBSession.BeginTransaction(myIsolationLevel: IsolationLevel.Serializable))
                    {
                        _QueryResult = this._QueryManager.ExecuteQuery(QueryScript, transaction.GetDBContext(), graphDBSession);
                        _QueryResult.AddErrorsAndWarnings(transaction.Commit());
                    }
                }
            }

            _QueryResult.Query = QueryScript;

            return _QueryResult;

        }

        /// <summary>
        /// As soon as the GetContent method of each statement needs no DBContext this methods should be removed and replaced by checking directly the parsed statement.
        /// But currently, we need to start a transaction for each query - but NOT for transaction related queries.
        /// </summary>
        /// <param name="queryScript"></param>
        /// <returns></returns>
        private bool isNonTransactionalQuery(string queryScript)
        {
            var qs = queryScript.ToUpper();
            return qs.Contains("TRANSACTION") && (qs.StartsWith("BEGIN") || qs.StartsWith("COMMIT") || qs.StartsWith("ROLLBACK"));
        }

        #endregion

        public UUID GetDatabaseUniqueID()
        {
            return this._internalDatabaseUUID;
        }

        public EntityUUID GetDatabaseUserID()
        {
            return this._internalUserID;
        }

        #region MapAndReduce(myDBTypeName, myMap, myReduce)

        public Exceptional<Object> MapAndReduce(DBContext dbContext, String myDBTypeName, Func<DBObjectMR, Object> myMap, Func<Object, Object> myReduce)
        {

            var _ListOfDBObjectMRs = new List<Object>();

            var myDBTypeStream = dbContext.DBTypeManager.GetTypeByName(myDBTypeName);

            var objectLocation = new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation, FSPathConstants.PathDelimiter, DBConstants.DBObjectsLocation));
            var allDBOLocations = _IGraphFSSession.GetFilteredDirectoryListing(objectLocation, null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null, null, null, null, null, null);

            if (allDBOLocations.Failed)
                return new Exceptional<Object>(allDBOLocations.Errors);

            Exceptional<DBObjectStream> _DBObjectExceptional = null;

            foreach (var loc in allDBOLocations.Value)
            {

                _DBObjectExceptional = dbContext.DBObjectManager.LoadDBObject(new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation, FSPathConstants.PathDelimiter, DBConstants.DBObjectsLocation, FSPathConstants.PathDelimiter, loc)));

                if (_DBObjectExceptional.Failed)
                    return new Exceptional<Object>(_DBObjectExceptional);

                if (myMap != null)
                    _ListOfDBObjectMRs.Add(myMap(new DBObjectMR(_DBObjectExceptional.Value, myDBTypeStream, dbContext)));
                else
                    _ListOfDBObjectMRs.Add(new DBObjectMR(_DBObjectExceptional.Value, myDBTypeStream, dbContext));

            }

            if (myReduce != null)
                return new Exceptional<object>(myReduce(_ListOfDBObjectMRs));
            else
                return new Exceptional<object>(_ListOfDBObjectMRs);

        }

        #endregion

        #region FilterMapReduce(myDBTypeName, myFilter, myMap, myReduce)

        public IEnumerable<T2> FilterMapReduce<T1, T2>(DBContext dbContext, String myDBTypeName, Func<DBObjectMR, Boolean> myFilter, Func<DBObjectMR, T1> myMap, Func<IEnumerable<T1>, IEnumerable<T2>> myReduce)
        {

            var myDBTypeStream = dbContext.DBTypeManager.GetTypeByName(myDBTypeName);

            var objectLocation = new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation, FSPathConstants.PathDelimiter, DBConstants.DBObjectsLocation));
            var allDBOLocations = _IGraphFSSession.GetFilteredDirectoryListing(objectLocation, null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null, null, null, null, null, null);

            var _DBObjectMRs = from loc in allDBOLocations.Value select new DBObjectMR(dbContext.DBObjectManager.LoadDBObject(new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation,
                                                                            FSPathConstants.PathDelimiter,
                                                                            DBConstants.DBObjectsLocation,
                                                                            FSPathConstants.PathDelimiter, loc))).Value,
                                                                            myDBTypeStream, dbContext);

            try
            {

                IEnumerable<T1> aa = null;

                if (myFilter != null && myMap != null)
                    aa = (from _DBObjectMR in _DBObjectMRs where myFilter(_DBObjectMR) select myMap(_DBObjectMR)).Cast<T1>();

                else if (myFilter == null && myMap != null)
                    aa = (from _DBObjectMR in _DBObjectMRs select myMap(_DBObjectMR)).Cast<T1>();

                else if (myFilter != null && myMap == null)
                    aa = (from _DBObjectMR in _DBObjectMRs where myFilter(_DBObjectMR) select (Object)_DBObjectMR).Cast<T1>();

                else
                    aa = (from _DBObjectMR in _DBObjectMRs select (Object) _DBObjectMR).Cast<T1>();


                if (myReduce != null)
                    return myReduce(aa);

                else
                    return aa.Cast<T2>();


            }

            catch
            {
                return new List<T2>();
            }


            //var _FilteredAndMappedddd = _FilteredAndMapped.ToList();


            //DBObjectMR newDBObjectMR = null;
            //var _ListOfDBObjectMRs = new List<Object>();
            //Exceptional<DBObjectStream> _DBObject = null;

            //foreach (var loc in allDBOLocations.Value)
            //{

            //    _DBObject = _TypeManager.LoadDBObject(new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation, FSPathConstants.PathDelimiter, DBConstants.DBObjectsLocation, FSPathConstants.PathDelimiter, loc))).Value;
                
            //    newDBObjectMR = new DBObjectMR(_DBObject.Value, myDBTypeStream, _TypeManager);

            //    if (myFilter != null)
            //        if (!myFilter(newDBObjectMR))
            //            continue;

            //    if (myMap != null)
            //        _ListOfDBObjectMRs.Add(myMap(newDBObjectMR));
            //    else
            //        _ListOfDBObjectMRs.Add(newDBObjectMR);

            //}


            //if (myReduce != null)
            //    return myReduce(_ListOfDBObjectMRs);

            //else
            //    return _ListOfDBObjectMRs;

        }

        #endregion

        #endregion

        #region NotificationDispatcher

        // The NotificationDispatcher handles all kind of notification between system parts or other dispatchers.
        // Use register to get notified as recipient.
        // Use SendNotification to send a notification to all subscribed recipients.

        private NotificationDispatcher  _NotificationDispatcher;
        private NotificationSettings    _NotificationSettings;


        #region GetNotificationDispatcher(SessionToken)

        /// <summary>
        /// Returns the NotificationDispatcher of this file system.
        /// </summary>
        /// <returns>The NotificationDispatcher of this file system</returns>
        public NotificationDispatcher GetNotificationDispatcher(SessionToken mySessionToken)
        {
            return _NotificationDispatcher;
        }

        #endregion

        #region GetNotificationSettings()

        /// <summary>
        /// Returns the NotificationDispatcher settings of this file system
        /// </summary>
        /// <returns>The NotificationDispatcher settings of this file system</returns>
        public NotificationSettings GetNotificationSettings(SessionToken mySessionToken)
        {
            return _NotificationSettings;
        }

        #endregion

        #region SetNotificationDispatcher(myNotificationDispatcher)

        /// <summary>
        /// Sets the NotificationDispatcher of this file system.
        /// </summary>
        /// <param name="myNotificationDispatcher">A NotificationDispatcher object</param>
        public void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher, SessionToken mySessionToken)
        {
            _NotificationDispatcher = myNotificationDispatcher;
        }

        #endregion

        #region SetNotificationSettings(myNotificationSettings)

        /// <summary>
        /// Sets the NotificationDispatcher settings of this file system
        /// </summary>
        /// <param name="myNotificationSettings">A NotificationSettings object</param>
        public void SetNotificationSettings(NotificationSettings myNotificationSettings, SessionToken mySessionToken)
        {
            _NotificationSettings = myNotificationSettings;
        }

        #endregion

        #region (private) StartDefaultNotificationDispatcher()

        private void StartDefaultNotificationDispatcher(String DatabaseIdentificationString)
        {

            if (_NotificationDispatcher == null)
                _NotificationDispatcher = new NotificationDispatcher(new UUID(DatabaseIdentificationString), _NotificationSettings);

        }

        #endregion

        #endregion

        #region Transactions

        #region BeginTransaction

        public DBTransaction BeginTransaction(DBContext dbContext, SessionToken mySessionToken, Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {
            var fsTransaction = _IGraphFSSession.BeginTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);

            DBTransaction _Transaction = null;
            var currentTransaction = DBTransaction.GetLatestTransaction(mySessionToken.SessionInfo.SessionUUID);
            if (currentTransaction.IsRunning())
            {
                _Transaction = currentTransaction.BeginNestedTransaction(fsTransaction);
            }
            else
            {
                _Transaction = new DBTransaction(dbContext, mySessionToken.SessionInfo.SessionUUID, fsTransaction);
                DBTransaction.SetTransaction(mySessionToken.SessionInfo.SessionUUID, _Transaction);
            }

            return _Transaction;

        }

        #endregion

        #region CommitTransaction

        internal DBTransaction CommitTransaction(SessionToken sessionToken, bool async)
        {
            var curTransaction = DBTransaction.GetLatestTransaction(sessionToken.SessionInfo.SessionUUID);

            if (!curTransaction.IsRunning())
            {
                return new DBTransaction(new Errors.Error_NoTransaction());
            }
            curTransaction.Push(curTransaction.Commit(async));
            //curTransaction = null;

            return curTransaction;
        }

        #endregion

        #region RollbackTransaction

        internal DBTransaction RollbackTransaction(SessionToken sessionToken, bool async)
        {
            var curTransaction = DBTransaction.GetLatestTransaction(sessionToken.SessionInfo.SessionUUID);

            if (!curTransaction.IsRunning())
            {
                return new DBTransaction(new Errors.Error_NoTransaction());
            }
            curTransaction.Push(curTransaction.Rollback(async));
            //curTransaction = null;

            return curTransaction;
        }
        
        #endregion

        #endregion


        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] Serialize()
        {

            var _SerializationWriter = new SerializationWriter();

            _SerializationWriter.WriteObject((UInt32)_DBSettings.Count);
            foreach (var pSetting in _DBSettings)
                _SerializationWriter.WriteObject(pSetting.Value);

            return _SerializationWriter.ToArray();

        }

        public void Deserialize(byte[] mySerializedData)
        {

            var _SerializationReader = new SerializationReader(mySerializedData);
            UInt32 _Capacity;

            _Capacity = (UInt32)_SerializationReader.ReadObject();
            if(_DBSettings != null)
                _DBSettings.Clear();

            for (UInt32 i = 0; i < _Capacity; i++)
            {
                 var pSetting = (ADBSettingsBase)Activator.CreateInstance(typeof(ADBSettingsBase), new Object[1] { (Byte[])_SerializationReader.ReadObject() });
                _DBSettings.Add(pSetting.Name, pSetting);
            }

        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
