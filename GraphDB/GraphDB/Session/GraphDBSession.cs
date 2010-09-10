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
 * GraphDBSession
 * (c) sones GmbH, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ImportExport;
using sones.GraphDB.Interfaces;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.Managers.Structures.Setting;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Plugin;
using sones.GraphDB.Session;
using sones.GraphDB.Structures.Enums;

using sones.GraphFS.Transactions;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Session;
using sones.Notifications;
using sones.GraphFS.DataStructures;
using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.Transactions;
using sones.GraphDB.Managers.Select;

#endregion

namespace sones.GraphDB
{

    public class GraphDBSession : IGraphDBSession
    {

        #region Data

        private DBPluginManager _DBPluginManager;
        private GraphDB2     _GraphDB;
        private SessionToken _SessionToken;
        private DBContext    _DBContext = null;
        //private QueryManager _QueryManager = null;

        #endregion

        #region Properties

        #region DatabaseRootPath

        public ObjectLocation DatabaseRootPath
        {
            get { return _GraphDB.DatabaseRootPath; }
        }

        #endregion

        public DBPluginManager DBPluginManager
        {
            get { return _DBPluginManager; }
        }

        #endregion

        #region Constructor

        #region GraphDBSession(myGraphDB, myUsername)

        public GraphDBSession(GraphDB2 myGraphDB, String myUsername)
        {
            _GraphDB        = myGraphDB;
            _SessionToken   = new SessionToken(new DBSessionInfo(myUsername));
            _DBPluginManager = new Plugin.DBPluginManager(null);

            //TODO: remove true for rebuild indices as soon as they are really persistent
            _DBContext = new DBContext(myGraphDB.GraphFSSession, _GraphDB.DatabaseRootPath, null, _GraphDB.DBSettings, false, _DBPluginManager, new DBSessionSettings(_SessionToken.SessionSettings));

            //_QueryManager = new QueryManager(_DBContext.DBPluginManager);
        }

        #endregion

        #region GraphDBSession(myGraphDB, myUsername)

        internal GraphDBSession(GraphDB2 myGraphDB, String myUsername, DBPluginManager myDBPluginManager)
        {
            _GraphDB = myGraphDB;
            _SessionToken = new SessionToken(new DBSessionInfo(myUsername));
            _DBPluginManager = myDBPluginManager;

            //TODO: remove true for rebuild indices as soon as they are really persistent
            _DBContext = new DBContext(myGraphDB.GraphFSSession, _GraphDB.DatabaseRootPath, null, _GraphDB.DBSettings, false, _DBPluginManager, new DBSessionSettings(_SessionToken.SessionSettings));

            //_QueryManager = new QueryManager(_DBContext.DBPluginManager);
        }

        #endregion

        #endregion


        #region IGraphDBSession Members

        #region DDL & DML

        public QueryResult AlterType(string myTypeName, List<Managers.AlterType.AAlterTypeCommand> myAlterCommands)
        {

            return _GraphDB.AlterType(_SessionToken, _DBContext, myTypeName, myAlterCommands);

        }

        public QueryResult CreateIndex(string myTypeName, string myIndexName, string myIndexEdition, string myIndexType, List<IndexAttributeDefinition> myAttributeList)
        {

            return _GraphDB.CreateIndex(_SessionToken, _DBContext, myTypeName, myIndexName, myIndexEdition, myIndexType, myAttributeList);

        }

        public QueryResult CreateTypes(List<GraphDBTypeDefinition> myGraphDBTypeDefinitions)
        {

            return _GraphDB.CreateTypes(_SessionToken, _DBContext, myGraphDBTypeDefinitions);

        }

        public QueryResult Delete(List<TypeReferenceDefinition> myTypeReferenceDefinitions, List<IDChainDefinition> myIDChainDefinitions, BinaryExpressionDefinition myWhereExpression)
        {

            return _GraphDB.Delete(_SessionToken, _DBContext, myTypeReferenceDefinitions, myIDChainDefinitions, myWhereExpression);

        }

        public QueryResult Describe(ADescribeDefinition myDescribeDefinition)
        {

            return _GraphDB.Describe(_SessionToken, _DBContext, myDescribeDefinition);

        }

        public QueryResult DropIndex(String myTypeName, String myIndexName, String myIndexEdition)
        {

            return _GraphDB.DropIndex(_SessionToken, _DBContext, myTypeName, myIndexName, myIndexEdition);

        }

        public QueryResult DropType(string myTypeName)
        {

            return _GraphDB.DropType(_SessionToken, _DBContext, myTypeName);

        }

        public QueryResult Export(string myDumpFormat, string myDestination, IDumpable myGrammar, IEnumerable<string> myTypes, ImportExport.DumpTypes myDumpType, ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            return _GraphDB.Export(_SessionToken, _DBContext, myDumpFormat, myDestination, myGrammar, myTypes, myDumpType, myVerbosityType);

        }

        public QueryResult Import(string myImportFormat, string myLocation, uint myParallelTasks = 1, IEnumerable<string> myComments = null, ulong? myOffset = null, ulong? myLimit = null, ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            return _GraphDB.Import(this, _SessionToken, _DBContext, myImportFormat, myLocation, myParallelTasks, myComments, myOffset, myLimit, myVerbosityType);

        }

        public QueryResult Insert(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList)
        {

            return _GraphDB.Insert(_SessionToken, _DBContext, myTypeName, myAttributeAssignList);

        }

        public QueryResult InsertOrReplace(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            return _GraphDB.InsertOrReplace(_SessionToken, _DBContext, myTypeName, myAttributeAssignList, myWhereExpression);

        }

        public QueryResult InsertOrUpdate(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            return _GraphDB.InsertOrUpdate(_SessionToken, _DBContext, myTypeName, myAttributeAssignList, myWhereExpression);

        }

        public QueryResult RebuildIndices(HashSet<string> myTypeNames = null)
        {

            return _GraphDB.RebuildIndices(_SessionToken, _DBContext, myTypeNames);

        }

        public QueryResult Replace(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            return _GraphDB.Replace(_SessionToken, _DBContext, myTypeName, myAttributeAssignList, myWhereExpression);

        }

        public QueryResult Select(List<Tuple<AExpressionDefinition, string, SelectValueAssignment>> mySelectedElements, List<TypeReferenceDefinition> myReferenceAndTypeList,
            BinaryExpressionDefinition myWhereExpressionDefinition = null, List<IDChainDefinition> myGroupBy = null, BinaryExpressionDefinition myHaving = null, OrderByDefinition myOrderByDefinition = null,
            ulong? myLimit = null, ulong? myOffset = null, long myResolutionDepth = -1, Boolean myRunWithTimeout = false)
        {

            return _GraphDB.Select(_SessionToken, _DBContext, mySelectedElements, myReferenceAndTypeList, myWhereExpressionDefinition, myGroupBy, myHaving, myOrderByDefinition, myLimit, myOffset, myResolutionDepth, myRunWithTimeout);

        }


        public QueryResult Setting(TypesOfSettingOperation myTypeOfSettingOperation, Dictionary<string, string> mySettings, ASettingDefinition myASettingDefinition)
        {

            return _GraphDB.Setting(_SessionToken, _DBContext, myTypeOfSettingOperation, mySettings, myASettingDefinition);

        }

        public QueryResult Truncate(string myTypeName)
        {

            return _GraphDB.Truncate(_SessionToken, _DBContext, myTypeName);

        }

        public QueryResult Update(string myTypeName, HashSet<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, BinaryExpressionDefinition myWhereExpression)
        {

            return _GraphDB.Update(_SessionToken, _DBContext, myTypeName, myListOfUpdates, myWhereExpression);

        }

        #endregion

        #region Transactions

        public DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {
            return _GraphDB.BeginTransaction(_SessionToken, _DBContext, myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);
        }

        public DBTransaction CommitTransaction(bool async = false)
        {
            return _GraphDB.CommitTransaction(_SessionToken, async);
        }

        public DBTransaction RollbackTransaction(bool async = false)
        {
            return _GraphDB.RollbackTransaction(_SessionToken, async);
        }

        public DBTransaction GetLatestTransaction()
        {
            return DBTransaction.GetLatestTransaction(_SessionToken.SessionInfo.SessionUUID);
        }

        #endregion

        #region Notifications

        public NotificationDispatcher GetNotificationDispatcher()
        {
            return _GraphDB.GetNotificationDispatcher(_SessionToken);
        }

        public NotificationSettings GetNotificationSettings()
        {
            return _GraphDB.GetNotificationSettings(_SessionToken);
        }

        public void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher)
        {
            _GraphDB.SetNotificationDispatcher(myNotificationDispatcher, _SessionToken);
        }

        public void SetNotificationSettings(NotificationSettings myNotificationSettings)
        {
            _GraphDB.SetNotificationSettings(myNotificationSettings, _SessionToken);
        }

        #endregion

        #region MapReduce

        #region MapAndReduce(myDBTypeName, myMap, myReduce)

        public Exceptional<Object> MapAndReduce(String myName, Func<DBObjectMR, Object> myMapAction, Func<Object, Object> myReduceAction)
        {
            return _GraphDB.MapAndReduce(_DBContext, myName, myMapAction, myReduceAction);
        }

        #endregion

        #region FilterMapReduce(myDBTypeName, myFilter, myMap, myReduce)

        public IEnumerable<T2> FilterMapReduce<T1, T2>(String myDBTypeName, Func<DBObjectMR, Boolean> myFilter, Func<DBObjectMR, T1> myMap, Func<IEnumerable<T1>, IEnumerable<T2>> myReduce)
        {
            return _GraphDB.FilterMapReduce<T1, T2>(_DBContext, myDBTypeName, myFilter, myMap, myReduce);
        }

        #endregion

        #endregion

        #region Database Management

        public UUID GetDatabaseUniqueID()
        {
            return _GraphDB.GetDatabaseUUID();
        }
        
        public void Shutdown()
        {
            _GraphDB.Shutdown(_SessionToken);
        }


        #endregion

        #endregion

        #region internal methods

        /// <summary>
        /// If you need the DBContext, create a new Transaction and use that dbContext!!
        /// </summary>
        /// <returns></returns>
        internal DBContext GetDBContext()
        {
            return _DBContext;
        }

        #endregion

    }

}
