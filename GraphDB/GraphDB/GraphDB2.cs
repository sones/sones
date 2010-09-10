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

/* <id name="GraphDB – Main Database Code" />
 * <copyright file="GraphDatabase.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphDB.DataStructures.Settings;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ImportExport;
using sones.GraphDB.Interfaces;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Select;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.Managers.Structures.Setting;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.Transactions;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.Transactions;
using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Notifications;
using sones.GraphDB.NewAPI;
using System.Threading;

#endregion

namespace sones.GraphDB
{

    /// <summary>
    /// This is the main class for the Graph Database Library and one instance of the Graph Database.
    /// </summary>
    public class GraphDB2 : IGraphDB
    {

        #region Data

        private DBInstanceSettingsManager<InstanceSettings> _InstanceSettingsManager = null;

        private ObjectLocation  _DatabaseRootPath = null;
        private IGraphFSSession _IGraphFSSession = null;

        private UUID            _InternalDatabaseUUID = null;
        private EntityUUID      _InternalUserID = null;
        private Dictionary<string, ADBSettingsBase> _DBSettings = null;

        #endregion

        #region Properties

        public ObjectLocation DatabaseRootPath
        {
            get { return _DatabaseRootPath; }
        }

        public Dictionary<String, ADBSettingsBase> DBSettings
        {
            get { return _DBSettings; }
        }

        public IGraphFSSession GraphFSSession
        {
            get { return _IGraphFSSession; }
        }

        public UUID GetDatabaseUUID()
        {
            return this._InternalDatabaseUUID;
        }

        public EntityUUID GetDatabaseUserID()
        {
            return this._InternalUserID;
        }

        #endregion

        #region Constructor(s)

        public GraphDB2(UUID myDatabaseInstanceUUID, ObjectLocation myDatabaseRootPath, IGraphFSSession myIGraphFSSession, Boolean myCreateNewIfNotExisting)
            : this(myDatabaseInstanceUUID, myDatabaseRootPath, myIGraphFSSession, myCreateNewIfNotExisting, true)
        {
        }

        /// <summary>
        /// Constructor for the Graph Database Instance Manager
        /// </summary>
        /// <param name="myDatabaseInstanceUUID">unique database id</param>
        /// <param name="DatabaseRootPath">where is the database - a path inside the Graph Filessystem, including the name of the database like "/database1"</param>
        /// <param name="GraphVFSInstance">Graph Virtual myIGraphFS Instance</param>
        /// <param name="CreateNewIfNotExisting">if true an empty database scheme and settings structure will be established at the given root path</param>
        public GraphDB2(UUID myDatabaseInstanceUUID, ObjectLocation myDatabaseRootPath, IGraphFSSession myIGraphFSSession, Boolean myCreateNewIfNotExisting, Boolean myRebuildIndices)
        {

            #region Data

            _DatabaseRootPath = myDatabaseRootPath;
            _IGraphFSSession = myIGraphFSSession;
            _InternalDatabaseUUID = myDatabaseInstanceUUID;
            _InternalUserID = new EntityUUID();
            _InternalUserID.Generate();

            #endregion

            #region Check if there's a RootPath already

            var isDirectExcept = this._IGraphFSSession.isIDirectoryObject(this._DatabaseRootPath);

            if (isDirectExcept.Failed())
                throw new GraphDBException(isDirectExcept.Errors);

            if (isDirectExcept.Value != Trinary.TRUE)
            {
                if (myCreateNewIfNotExisting)
                {
                    // it's not there and we should create it...
                    var createDirExcept = this._IGraphFSSession.CreateDirectoryObject(this._DatabaseRootPath);

                    if (createDirExcept.Failed())
                        throw new GraphDBException(createDirExcept.Errors);
                }
                else
                {
                    throw new GraphDBException(new Error_DatabaseNotFound(_DatabaseRootPath));
                }
            }

            #endregion

            #region Read the Database Instance Metadata

            _DBSettings = new Dictionary<String, ADBSettingsBase>();
            _InstanceSettingsManager = new DBInstanceSettingsManager<InstanceSettings>(_DatabaseRootPath, _IGraphFSSession, myCreateNewIfNotExisting);

            #endregion

            #region Notification System Startup

            _NotificationSettings = new NotificationSettings();
            StartDefaultNotificationDispatcher(this._InstanceSettingsManager.Content.Identifier);

            #endregion

        }

        #endregion

        #region Public Methods

        #region Query(QueryScript, myGraphDBSession, myQueryManager)
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myQueryScript"></param>
        /// <param name="myGraphDBSession">Needed for BeginTransaction inside any AStatementNode</param>
        /// <returns></returns>
        public QueryResult Query(String myQueryScript, GraphDBSession myGraphDBSession, QueryManager myQueryManager)
        {

            #region Data

            QueryResult _QueryResult;

            #endregion

            /// As soon as the GetContent method of each statement needs no DBContext this methods should be removed and replaced by checking directly the parsed statement.
            /// But currently, we need to start a transaction for each query - but NOT for transaction related queries.
            if (isNonTransactionalQuery(myQueryScript))
            {
                _QueryResult = myQueryManager.ExecuteQuery(myQueryScript, myGraphDBSession.GetDBContext(), myGraphDBSession);
            }

            else
            {

                // create a inner transaction for each statement
                // This is mandatory because the GetContent of each node need to read inside a transaction
                // This should be changed sometimes: 
                    //1) no query is allowed without a valid transaction
                    //2) GetContent does not get any DBContext, so we can create the transaction just for the execute and with that we know we need either a ReadOnly or Serializeable

                if (myGraphDBSession.GetLatestTransaction().IsRunning())
                {

                    #region Create a new transaction based in the existing transaction

                    var existingTransaction = myGraphDBSession.GetLatestTransaction();
                    using (var _Transaction = myGraphDBSession.BeginTransaction(existingTransaction.Distributed, existingTransaction.LongRunning, existingTransaction.IsolationLevel))
                    {
                        _QueryResult = myQueryManager.ExecuteQuery(myQueryScript, _Transaction.GetDBContext(), myGraphDBSession);
                        _QueryResult.AddErrorsAndWarnings(_Transaction.Commit());
                    }

                    #endregion

                }

                else
                {
                    using (var _Transaction = myGraphDBSession.BeginTransaction(myIsolationLevel: IsolationLevel.Serializable))
                    {
                        _QueryResult = myQueryManager.ExecuteQuery(myQueryScript, _Transaction.GetDBContext(), myGraphDBSession);
                        _QueryResult.AddErrorsAndWarnings(_Transaction.Commit());
                    }
                }

            }

            _QueryResult.Query = myQueryScript;

            return _QueryResult;

        }

        /// <summary>
        /// As soon as the GetContent method of each statement needs no DBContext this methods should be removed and replaced by checking directly the parsed statement.
        /// But currently, we need to start a transaction for each query - but NOT for transaction related queries.
        /// </summary>
        /// <param name="myQueryScript"></param>
        /// <returns></returns>
        private Boolean isNonTransactionalQuery(String myQueryScript)
        {
            var qs = myQueryScript.ToUpper();
            return qs.Contains("TRANSACTION") && (qs.StartsWith("BEGIN") || qs.StartsWith("COMMIT") || qs.StartsWith("ROLLBACK"));
        }
        */
        #endregion
                

        #region MapAndReduce(myDBTypeName, myMap, myReduce)

        public Exceptional<Object> MapAndReduce(DBContext dbContext, String myDBTypeName, Func<DBObjectMR, Object> myMap, Func<Object, Object> myReduce)
        {

            var _ListOfDBObjectMRs = new List<Object>();

            var myDBTypeStream = dbContext.DBTypeManager.GetTypeByName(myDBTypeName);

            var objectLocation = new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation, FSPathConstants.PathDelimiter, DBConstants.DBObjectsLocation));
            var allDBOLocations = _IGraphFSSession.GetFilteredDirectoryListing(objectLocation, null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null, null, null, null, null, null);

            if (allDBOLocations.Failed())
                return new Exceptional<Object>(allDBOLocations.Errors);

            Exceptional<DBObjectStream> _DBObjectExceptional = null;

            foreach (var loc in allDBOLocations.Value)
            {

                _DBObjectExceptional = dbContext.DBObjectManager.LoadDBObject(new ObjectLocation(String.Concat(myDBTypeStream.ObjectLocation, FSPathConstants.PathDelimiter, DBConstants.DBObjectsLocation, FSPathConstants.PathDelimiter, loc)));

                if (_DBObjectExceptional.Failed())
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


        #region DDL & DML

        public QueryResult AlterType(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, List<Managers.AlterType.AAlterTypeCommand> myAlterCommands)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("AlterType", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {
                var dbInnerContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "AlterType");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                #region check GraphType

                var dbType = dbInnerContext.DBTypeManager.GetTypeByName(myTypeName);
                if (dbType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                #endregion

                QueryResult qr = new QueryResult();

                List<DBObjectReadout> resultingDBreadouts = new List<DBObjectReadout>();

                foreach (var alterTypeCmd in myAlterCommands)
                {
                    var result = dbInnerContext.DBTypeManager.AlterType(dbInnerContext, dbType, alterTypeCmd);
                    qr.AddErrorsAndWarnings(result);

                    if (result.Value != null)
                    {
                        resultingDBreadouts.AddRange(result.Value.Results.Objects);
                    }
                }

                qr.SetResult(new SelectionResultSet(resultingDBreadouts));

                #region Commit transaction and add all Warnings and Errors

                qr.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return qr;
            }
        }

        public QueryResult CreateIndex(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, string myIndexName, string myIndexEdition, string myIndexType, List<IndexAttributeDefinition> myAttributeList)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("CreateIndex", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var _Transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                DBContext transactionContext = (DBContext)_Transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "CreateIndex");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var qresult = new QueryResult();

                #region Create the index

                var resultOutput = transactionContext.DBIndexManager.CreateIndex(transactionContext, myTypeName, myIndexName, myIndexEdition, myIndexType, myAttributeList);
                qresult.AddErrorsAndWarnings(resultOutput);

                #endregion

                #region Commit transaction and add all Warnings and Errors

                qresult.AddErrorsAndWarnings(_Transaction.Commit());

                #endregion

                if (qresult.ResultType == ResultType.Successful)
                {
                    qresult.SetResult(resultOutput.Value);
                }

                return qresult;

            }

        }

        public QueryResult CreateTypes(SessionToken mySessionToken, DBContext mySessionContext, List<GraphDBTypeDefinition> myGraphDBTypeDefinitions)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("CreateTypes", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var transactionContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "CreateTypes");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var result = transactionContext.DBTypeManager.AddBulkTypes(myGraphDBTypeDefinitions, transactionContext);

                if (!result.Success())
                {

                    #region Rollback transaction and add all Warnings and Errors

                    result.AddErrorsAndWarnings(transaction.Rollback());

                    #endregion

                    return new QueryResult(result.Errors);

                }
                else
                {

                    #region Commit transaction and add all Warnings and Errors

                    result.Value.AddErrorsAndWarnings(transaction.Commit());

                    #endregion

                    return result.Value;

                }

            }

        }

        public QueryResult Delete(SessionToken mySessionToken, DBContext mySessionContext, List<TypeReferenceDefinition> myTypeReferenceDefinitions, List<IDChainDefinition> myIDChainDefinitions, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Delete", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                DBContext dbInnerContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "Delete");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var _TypeWithUndefAttrs = new Dictionary<GraphDBType, List<string>>();
                var _DBTypeAttributeToDelete = new Dictionary<GraphDBType, List<TypeAttribute>>();

                var _ReferenceTypeLookup = GetTypeReferenceLookup(dbInnerContext, myTypeReferenceDefinitions);
                if (_ReferenceTypeLookup.Failed())
                {
                    return new QueryResult(_ReferenceTypeLookup);
                }

                foreach (var id in myIDChainDefinitions)
                {

                    id.Validate(dbInnerContext, _ReferenceTypeLookup.Value, true);
                    if (id.ValidateResult.Failed())
                    {
                        return new QueryResult(id.ValidateResult);
                    }

                    if ((id.Level > 0) && (id.Depth > 1))
                    {
                        return new QueryResult(new Error_RemoveTypeAttribute(id.LastType, id.LastAttribute));
                    }

                    if (id.IsUndefinedAttribute)
                    {

                        if (!_TypeWithUndefAttrs.ContainsKey(id.LastType))
                        {
                            _TypeWithUndefAttrs.Add(id.LastType, new List<String>());
                        }
                        _TypeWithUndefAttrs[id.LastType].Add(id.UndefinedAttribute);

                    }
                    else
                    {
                        if (!_DBTypeAttributeToDelete.ContainsKey(id.LastType))
                        {
                            _DBTypeAttributeToDelete.Add(id.LastType, new List<TypeAttribute>());
                        }
                        if (id.LastAttribute != null) // in case we want to delete the complete DBO we have no attribute definition
                        {
                            _DBTypeAttributeToDelete[id.LastType].Add(id.LastAttribute);
                        }
                    }

                }


                var _ObjectManipulationManager = new ObjectManipulationManager();


                var result = _ObjectManipulationManager.Delete(myWhereExpression, dbInnerContext, _TypeWithUndefAttrs, _DBTypeAttributeToDelete, _ReferenceTypeLookup.Value);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

        public QueryResult Describe(SessionToken mySessionToken, DBContext mySessionContext, ADescribeDefinition myDescribeDefinition)
        {

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var transactionContext = (DBContext)transaction.GetDBContext();

                var result = myDescribeDefinition.GetResult(transactionContext);
                QueryResult qresult = null;

                if (result.Failed())
                {
                    qresult = new QueryResult(result);
                }
                else
                {
                    qresult = new QueryResult(result.Value);
                }

                qresult.AddErrorsAndWarnings(transaction.Commit());

                return qresult;
            }

        }

        public QueryResult DropIndex(SessionToken mySessionToken, DBContext mySessionContext, String myTypeName, String myIndexName, String myIndexEdition)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("DropIndex", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var transactionContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "DropIndex");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var graphDBTypeType = transactionContext.DBTypeManager.GetTypeByName(myTypeName);
                if (graphDBTypeType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                var RemoveIdxException = graphDBTypeType.RemoveIndex(myIndexName, myIndexEdition, transactionContext.DBTypeManager);

                if (!RemoveIdxException.Success())
                {
                    return new QueryResult(RemoveIdxException);
                }
                else
                {

                    #region Commit transaction and add all Warnings and Errors

                    return new QueryResult(transaction.Commit());

                    #endregion

                }
            }

        }

        public QueryResult DropType(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("DropType", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {


                var transactionContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "DropType");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                GraphDBType graphDBType = transactionContext.DBTypeManager.GetTypeByName(myTypeName);

                if (graphDBType == null)
                {
                    GraphDBError aError = new Error_TypeDoesNotExist(myTypeName);

                    return new QueryResult(aError);
                }

                var removeExcept = transactionContext.DBTypeManager.RemoveType(graphDBType);

                if (!removeExcept.Success())
                {
                    return new QueryResult(removeExcept);
                }
                else
                {

                    #region Commit transaction and add all Warnings and Errors

                    return new QueryResult(transaction.Commit());

                    #endregion

                }

            }

        }

        public QueryResult Export(SessionToken mySessionToken, DBContext mySessionContext, string myDumpFormat, string myDestination, IDumpable myGrammar, IEnumerable<string> myTypes, ImportExport.DumpTypes myDumpType, ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            var dumpReadout = new Dictionary<String, Object>();
            AGraphDBExport exporter;

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext, myIsolationLevel: GraphFS.Transactions.IsolationLevel.ReadCommitted))
            {

                DBContext transactionContext = (DBContext)transaction.GetDBContext();

                if (!transactionContext.DBPluginManager.HasGraphDBExporter(myDumpFormat))
                {
                    throw new GraphDBException(new Error_ExporterDoesNotExist(myDumpFormat));
                }

                exporter = transactionContext.DBPluginManager.GetGraphDBExporter(myDumpFormat);

                return exporter.Export(myDestination, transactionContext, myGrammar, myTypes, myDumpType, myVerbosityType);

            }

        }

        public QueryResult Import(IGraphDBSession myGraphDBSession, SessionToken mySessionToken, DBContext mySessionContext, string myImportFormat, string myLocation, uint myParallelTasks = 1, IEnumerable<string> myComments = null, ulong? myOffset = null, ulong? myLimit = null, ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Import", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                DBContext transactionContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "Import");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                if (!transactionContext.DBPluginManager.HasGraphDBImporter(myImportFormat))
                {
                    throw new GraphDBException(new Error_ImporterDoesNotExist(myImportFormat));
                }

                var importer = transactionContext.DBPluginManager.GetGraphDBImporter(myImportFormat);
                var importResult = importer.Import(myLocation, myGraphDBSession, transactionContext, myParallelTasks, myComments, myOffset, myLimit, myVerbosityType);

                if (importResult.ResultType == ResultType.Successful)
                {
                    importResult.AddErrorsAndWarnings(transaction.Commit());
                }

                return importResult;
            }

        }

        public QueryResult Insert(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Insert", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var dbInnerContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "Insert");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                ObjectManipulationManager _ObjectManipulationManager = new ObjectManipulationManager();

                var graphDBType = dbInnerContext.DBTypeManager.GetTypeByName(myTypeName);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                var attrsResult = _ObjectManipulationManager.EvaluateAttributes(dbInnerContext, graphDBType, myAttributeAssignList);
                if (!attrsResult.Success())
                {
                    return new QueryResult(attrsResult);
                }

                var result = _ObjectManipulationManager.Insert(dbInnerContext, graphDBType, attrsResult.Value);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

        public QueryResult InsertOrReplace(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("InsertOrReplace", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            #region Data

            var objectManipulationManager = new ObjectManipulationManager();

            #endregion

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var dbInnerContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "InsertOrReplace");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var graphDBType = dbInnerContext.DBTypeManager.GetTypeByName(myTypeName);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                var result = objectManipulationManager.InsertOrReplace(dbInnerContext, graphDBType, myAttributeAssignList, myWhereExpression);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

        public QueryResult InsertOrUpdate(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("InsertOrUpdate", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            var _ObjectManipulationManager = new ObjectManipulationManager();
            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var dbInnerContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "InsertOrUpdate");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var graphDBType = dbInnerContext.DBTypeManager.GetTypeByName(myTypeName);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                var result = _ObjectManipulationManager.InsertOrUpdate(dbInnerContext, graphDBType, myAttributeAssignList, myWhereExpression);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

        public QueryResult RebuildIndices(SessionToken mySessionToken, DBContext mySessionContext, HashSet<string> myTypeNames = null)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("RebuildIndices", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                Exceptional<Boolean> rebuildResult = null;
                IEnumerable<GraphDBType> typesToRebuild;
                QueryResult result = new QueryResult();

                DBContext transactionContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "RebuildIndices");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion
                
                if (myTypeNames.IsNullOrEmpty())
                {
                    typesToRebuild = transactionContext.DBTypeManager.GetAllTypes(false);
                }
                else
                {

                    #region Get types by name and return on error

                    typesToRebuild = new HashSet<GraphDBType>();
                    foreach (var typeName in myTypeNames)
                    {
                        var type = ((DBContext)transaction.GetDBContext()).DBTypeManager.GetTypeByName(typeName);
                        if (type == null)
                        {
                            return new QueryResult(new Errors.Error_TypeDoesNotExist(typeName));
                        }
                        (typesToRebuild as HashSet<GraphDBType>).Add(type);
                    }

                    #endregion

                }

                rebuildResult = ((DBContext)transaction.GetDBContext()).DBIndexManager.RebuildIndices(typesToRebuild);

                if (!rebuildResult.Success())
                {
                    result = new QueryResult(rebuildResult.Errors);

                    result.AddErrorsAndWarnings(transaction.Rollback());

                    return result;
                }
                else
                {
                    return new QueryResult(transaction.Commit());
                }

            }

        }

        public QueryResult Replace(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Replace", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            #region Data

            var _ObjectManipulationManager = new ObjectManipulationManager();

            #endregion

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var dbInnerContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "Replace");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var type = dbInnerContext.DBTypeManager.GetTypeByName(myTypeName);
                if (type == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                var result = _ObjectManipulationManager.Replace(dbInnerContext, type, myAttributeAssignList, myWhereExpression);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

        public QueryResult Select(SessionToken mySessionToken, DBContext mySessionContext, List<Tuple<AExpressionDefinition, string, SelectValueAssignment>> mySelectedElements, List<TypeReferenceDefinition> myReferenceAndTypeList,
            BinaryExpressionDefinition myWhereExpressionDefinition = null, List<IDChainDefinition> myGroupBy = null, BinaryExpressionDefinition myHaving = null, OrderByDefinition myOrderByDefinition = null,
            ulong? myLimit = null, ulong? myOffset = null, long myResolutionDepth = -1, Boolean myRunWithTimeout = false)
        {

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext, myIsolationLevel: IsolationLevel.ReadCommitted))
            {

                var selectManager = new SelectManager();
                DBContext transactionContext = (DBContext)transaction.GetDBContext();

                if (myRunWithTimeout)
                {

                    var timeout = Convert.ToInt32(transactionContext.DBSettingsManager.GetSettingValue((new SettingSelectTimeOut()).ID, transactionContext, TypesSettingScope.DB).Value.Value);

                    #region select in Task

                    var selectTask = Task.Factory.StartNew(() =>
                    {
                        return selectManager.ExecuteSelect((DBContext)transaction.GetDBContext(), mySelectedElements, myReferenceAndTypeList, myWhereExpressionDefinition, myGroupBy, myHaving,
                        myOrderByDefinition, myLimit, myOffset, myResolutionDepth);
                    });

                    if (selectTask.Wait(timeout))
                    {
                        var qresult = selectTask.Result;
                        qresult.AddErrorsAndWarnings(transaction.Commit());
                        return qresult;
                    }
                    else
                    {
                        return new QueryResult(new Error_SelectTimeOut(10000));
                    }

                    #endregion

                }
                else
                {

                    var qresult = selectManager.ExecuteSelect((DBContext)transaction.GetDBContext(), mySelectedElements, myReferenceAndTypeList, myWhereExpressionDefinition, myGroupBy, myHaving,
                        myOrderByDefinition, myLimit, myOffset, myResolutionDepth);
                    qresult.AddErrorsAndWarnings(transaction.Commit());
                    return qresult;

                }

            }

        }


        public QueryResult Setting(SessionToken mySessionToken, DBContext mySessionContext, TypesOfSettingOperation myTypeOfSettingOperation, Dictionary<string, string> mySettings, ASettingDefinition myASettingDefinition)
        {

            if (myTypeOfSettingOperation != TypesOfSettingOperation.GET && !IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Setting", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                DBContext transactionContext = (DBContext)transaction.GetDBContext();

                var result = transactionContext.DBSettingsManager.ExecuteSettingOperation(transactionContext, myASettingDefinition, myTypeOfSettingOperation, mySettings);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;
            }

        }

        public QueryResult Truncate(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Truncate", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var _Transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var _DBInnerContext = (DBContext)_Transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(_DBInnerContext, "Truncate");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var _ObjectManipulationManager = new ObjectManipulationManager(_DBInnerContext);
                var _GraphDBType = _DBInnerContext.DBTypeManager.GetTypeByName(myTypeName);

                if (_GraphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                if (_DBInnerContext.DBTypeManager.GetAllSubtypes(_GraphDBType, false).Count > 0)
                {
                    return new QueryResult(new Error_TruncateNotAllowedOnInheritedType(myTypeName));
                }

                Exceptional truncateResult = _ObjectManipulationManager.Truncate(_DBInnerContext, _GraphDBType);
                if (truncateResult.Failed())
                {
                    return new QueryResult(truncateResult);
                }

                #region Commit transaction and add all Warnings and Errors

                var queryResult = new QueryResult(_Transaction.Commit().Push(truncateResult));

                #endregion

                return queryResult;

            }

        }

        public QueryResult Update(SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, HashSet<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction(mySessionToken))
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Update", GetLatestTransaction(mySessionToken).IsolationLevel));
            }

            using (var transaction = BeginTransaction(mySessionToken, mySessionContext))
            {

                var transactionContext = (DBContext)transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "Update");
                if (readWriteCheck.Failed())
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var graphDBType = transactionContext.DBTypeManager.GetTypeByName(myTypeName);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                var objectManipulationManager = new ObjectManipulationManager();
                var queryResult = objectManipulationManager.Update(myListOfUpdates, transactionContext, graphDBType, myWhereExpression);

                // Commit transaction and add all Warnings and Errors
                queryResult.AddErrorsAndWarnings(transaction.Commit());

                return queryResult;

            }

        }

        #endregion


        #region BeginTransaction

        public DBTransaction BeginTransaction(SessionToken mySessionToken, DBContext dbContext, Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {
            var fsTransaction = _IGraphFSSession.BeginFSTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);

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

        #region GetLatestTransaction

        public DBTransaction GetLatestTransaction(SessionToken mySessionToken)
        {
            return DBTransaction.GetLatestTransaction(mySessionToken.SessionInfo.SessionUUID);
        }
        
        #endregion

        #region Shutdown(mySessionToken)

        /// <summary>
        /// Initiates the Shutdown of this Database Instance Manager
        /// </summary>
        public void Shutdown(SessionToken mySessionToken)
        {

            // Shutdown the notification dispatcher
            if (_NotificationDispatcher != null)
                _NotificationDispatcher.Dispose();

        }

        #endregion

        #endregion

        #region Private methods

        private Exceptional<Dictionary<string, GraphDBType>> GetTypeReferenceLookup(DBContext myDBContext, List<TypeReferenceDefinition> myTypeReferenceDefinitions)
        {
            var _ReferenceTypeLookup = new Dictionary<string, GraphDBType>();
            foreach (var trd in myTypeReferenceDefinitions)
            {
                var dbType = myDBContext.DBTypeManager.GetTypeByName(trd.TypeName);

                if (dbType == null)
                {
                    return new Exceptional<Dictionary<string, GraphDBType>>(new Errors.Error_TypeDoesNotExist(trd.TypeName));
                }

                _ReferenceTypeLookup.Add(trd.Reference, dbType);
            }

            return new Exceptional<Dictionary<string, GraphDBType>>(_ReferenceTypeLookup);
        }

        public Boolean IsWriteTransaction(SessionToken mySessionToken)
        {

            #region Check whether the statement is readWrite and the transaction is readOnly <- error!

            var latestTrans = GetLatestTransaction(mySessionToken);
            if (latestTrans.IsRunning())
            {
                if (latestTrans.IsReadonly())
                {
                    return false;
                }
            }

            #endregion

            return true;

        }

        internal Exceptional VerifyReadWriteOperationIsValid(DBContext myDBContext, String myOperation = "")
        {
            var isReadOnlySettingValue = myDBContext.DBSettingsManager.GetSettingValue(new SettingReadonly().Name, myDBContext, TypesSettingScope.DB);
            if (isReadOnlySettingValue.Failed())
            {
                return new Exceptional(isReadOnlySettingValue);
            }

            if ((isReadOnlySettingValue.Value as DBBoolean).GetValue())
            {
                return new Exceptional(new Error_ReadOnlyViolation(myOperation));
            }

            return Exceptional.OK;
        }

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

        /// <summary>
        /// Starts a traversal and returns the found paths or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="mySessionToken">The currenct session</param>
        /// <param name="myStartVertex">The starting vertex</param>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myFollowThisPath">Follow this path (== actual path + NEW edge + NEW dbobject? Based on edge/object UUID, TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public T TraversePath<T>(   SessionToken                             mySessionToken,
                                    DBVertex                                 myStartVertex,
                                    TraversalOperation                       TraversalOperation  = TraversalOperation.BreathFirst,
                                    Func<DBPath, sones.GraphDB.NewAPI.DBEdge, Boolean>            myFollowThisEdge    = null,
                                    Func<DBPath, sones.GraphDB.NewAPI.DBEdge, DBVertex, Boolean> myFollowThisPath = null,
                                    Func<DBPath, Boolean>                    myMatchEvaluator    = null,
                                    Action<DBPath>                           myMatchAction       = null,
                                    Func<TraversalState, Boolean>            myStopEvaluator     = null,
                                    Func<IEnumerable<DBPath>, T>             myWhenFinished      = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts a traversal and returns the found vertices or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="mySessionToken">The currenct session</param>
        /// <param name="myStartVertex">The starting vertex</param>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public T TraverseVertex<T>( SessionToken                    mySessionToken,
                                    DBVertex                        myStartVertex,
                                    TraversalOperation              TraversalOperation      = TraversalOperation.BreathFirst,
                                    Func<DBVertex, sones.GraphDB.NewAPI.DBEdge, Boolean> myFollowThisEdge = null,
                                    Func<DBVertex, Boolean>         myMatchEvaluator        = null,
                                    Action<DBVertex>                myMatchAction           = null,
                                    Func<TraversalState, Boolean>   myStopEvaluator         = null,
                                    Func<IEnumerable<DBVertex>, T>  myWhenFinished          = null)
        {
            TraversalState myTraversalState = new TraversalState(myStartVertex);


            if (myWhenFinished != null)
            {
                return myWhenFinished(TraverseVertex_private(mySessionToken, myStartVertex, null, TraversalOperation, true, myFollowThisEdge, myMatchEvaluator, myMatchAction, myStopEvaluator, myTraversalState));
            }
            else
            {
                return (T)TraverseVertex_private(mySessionToken, myStartVertex, null, TraversalOperation, true, myFollowThisEdge, myMatchEvaluator, myMatchAction, myStopEvaluator, myTraversalState);
            }

        }

        /// <summary>
        /// Starts a traversal and returns the found vertices
        /// </summary>
        /// <param name="sessionToken">The currenct session</param>
        /// <param name="currentVertex">The current vertex</param>
        /// <param name="viaEdge">The edge which has lead to the current vertex</param>
        /// <param name="traversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myAvoidCircles">Avoidance of circles</param>
        /// <param name="followThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="matchEvaluator">Mhm, this vertex looks interesting!</param>
        /// <param name="matchAction">Hey! I have found something interesting!</param>
        /// <param name="stopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="traversalState">The traversal state</param>
        /// <returns>An IEnumerable of DBVertex</returns>
        private IEnumerable<DBVertex> TraverseVertex_private(   SessionToken sessionToken,
                                                                DBVertex currentVertex,
                                                                NewAPI.DBEdge viaEdge,
                                                                TraversalOperation traversalOperation,
                                                                Boolean myAvoidCircles,
                                                                Func<DBVertex, NewAPI.DBEdge, bool> followThisEdge,
                                                                Func<DBVertex, bool> matchEvaluator,
                                                                Action<DBVertex> matchAction,
                                                                Func<TraversalState, bool> stopEvaluator,
                                                                TraversalState traversalState)
        {
            #region stop evaluation?
            //are we allowed to stop the current traversal

            if (stopEvaluator != null)
            {
                if (stopEvaluator(traversalState))
                {
                    yield break;
                }
            }

            #endregion

            #region currentVertex match?
            //does the current node match the requirements?

            Boolean match = false;

            #region match evaluation

            if (matchEvaluator != null)
            {
                //there is a match evaluator... use it

                if (matchEvaluator(currentVertex))
                {
                    match = true;
                }
            }
            else
            {
                //there is no special function that evaluates if the current vertex matches... so EVERY Vertex matches

                match = true;
            }

            #endregion

            if (match)
            {
                #region match action

                if (matchAction != null)
                {
                    matchAction.Invoke(currentVertex);
                }

                #endregion

                #region update traversal state
                //update number of found elements

                traversalState.IncreaseNumberOfFoundElements();

                #endregion
            }

            #endregion

            #region update statistics on traversal state

            traversalState.AddVisitedVertexViaEdge(currentVertex, viaEdge);

            #endregion

            #region return and traverse

            if (match)
            {
                //return the current vertex if it matched
                yield return currentVertex;
            }

            #region recursive traverse
            //get all edges and try to traverse them


            switch (traversalOperation)
            {
                case TraversalOperation.BreathFirst:

                    #region BreathFirst

                    throw new NotImplementedException();

                    #endregion

                case TraversalOperation.DepthFirst:

                    #region DepthFirst

                    foreach (var aEdge in currentVertex.GetEdges())
                    {
                        #region try to traverse via aEdge

                        //check for circle avoidance
                        if (myAvoidCircles)
                        {
                            #region check traversal state
                            //check the traversal state for circles... if there is one, break!

                            if (traversalState.AlreadyVisitedVertexViaEdge(aEdge))
                            {
                                continue;
                            }

                            #endregion
                        }

                        //check the "follow this Edge" function
                        if (followThisEdge != null)
                        {
                            #region check edge
                            //check if the edge should be followed... if not, break!

                            if (!followThisEdge(currentVertex, aEdge))
                            {
                                continue;
                            }

                            #endregion
                        }

                        //move recursive in depth
                        foreach (var aMatchingVertex in TraverseVertex_private(sessionToken, aEdge.TargetVertex, aEdge, traversalOperation, myAvoidCircles, followThisEdge, matchEvaluator, matchAction, stopEvaluator, traversalState))
                        {
                            yield return aMatchingVertex;
                        }

                        #endregion
                    }

                    break;

                    #endregion

                default:

                    #region default

                    throw new NotImplementedException();

                    #endregion
            }

            #endregion

            #endregion

            yield break;
        }
    }

}
