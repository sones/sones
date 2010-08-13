/*
 * IGraphDBInterface
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB
{

    public interface IGraphDB
    {

//        sones.GraphDB.Structures.Result.QueryResult AlterType(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, System.Collections.Generic.List<sones.GraphDB.Managers.AlterType.AAlterTypeCommand> myAlterCommands);
//        DBTransaction BeginTransaction(sones.Lib.Session.SessionToken mySessionToken, DBContext dbContext, bool myDistributed = false, bool myLongRunning = false, sones.GraphFS.Transactions.IsolationLevel myIsolationLevel = IsolationLevel.Serializable, string myName = "", DateTime? timestamp = null);
//        sones.GraphDB.Structures.Result.QueryResult CreateIndex(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, string myIndexName, string myIndexEdition, string myIndexType, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.IndexAttributeDefinition> myAttributeList);
//        sones.GraphDB.Structures.Result.QueryResult CreateTypes(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.GraphDBTypeDefinition> myGraphDBTypeDefinitions);
        ObjectLocation DatabaseRootPath { get; }
//        System.Collections.Generic.Dictionary<string, sones.GraphDB.Settings.ADBSettingsBase> DBSettings { get; }
//        sones.GraphDB.Structures.Result.QueryResult Delete(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.TypeReferenceDefinition> myTypeReferenceDefinitions, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.IDChainDefinition> myIDChainDefinitions, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myWhereExpression);
//        sones.GraphDB.Structures.Result.QueryResult Describe(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, sones.GraphDB.Managers.Structures.Describe.ADescribeDefinition myDescribeDefinition);
//        sones.GraphDB.Structures.Result.QueryResult DropIndex(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, string myIndexName, string myIndexEdition);
//        sones.GraphDB.Structures.Result.QueryResult DropType(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName);
//        sones.GraphDB.Structures.Result.QueryResult Export(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myDumpFormat, string myDestination, sones.GraphDB.Interfaces.IDumpable myGrammar, System.Collections.Generic.IEnumerable<string> myTypes, sones.GraphDB.ImportExport.DumpTypes myDumpType, sones.GraphDB.ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors);
//        System.Collections.Generic.IEnumerable<T2> FilterMapReduce<T1, T2>(DBContext dbContext, string myDBTypeName, Func<sones.GraphDB.ObjectManagement.DBObjectMR, bool> myFilter, Func<sones.GraphDB.ObjectManagement.DBObjectMR, T1> myMap, Func<System.Collections.Generic.IEnumerable<T1>, System.Collections.Generic.IEnumerable<T2>> myReduce);
//        sones.GraphFS.DataStructures.EntityUUID GetDatabaseUserID();
//        sones.Lib.DataStructures.UUID.UUID GetDatabaseUUID();
//        sones.GraphDB.Transactions.DBTransaction GetLatestTransaction(sones.Lib.Session.SessionToken mySessionToken);
//        sones.Notifications.NotificationDispatcher GetNotificationDispatcher(sones.Lib.Session.SessionToken mySessionToken);
//        sones.Notifications.NotificationSettings GetNotificationSettings(sones.Lib.Session.SessionToken mySessionToken);
//        sones.GraphFS.Session.IGraphFSSession GraphFSSession { get; }
//        sones.GraphDB.Structures.Result.QueryResult Import(IGraphDBSession myGraphDBSession, sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myImportFormat, string myLocation, uint myParallelTasks = 1, System.Collections.Generic.IEnumerable<string> myComments = null, ulong? myOffset = null, ulong? myLimit = null, sones.GraphDB.ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors);
//        sones.GraphDB.Structures.Result.QueryResult Insert(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.AAttributeAssignOrUpdate> myAttributeAssignList);
//        sones.GraphDB.Structures.Result.QueryResult InsertOrReplace(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.AAttributeAssignOrUpdate> myAttributeAssignList, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myWhereExpression);
//        sones.GraphDB.Structures.Result.QueryResult InsertOrUpdate(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.AAttributeAssignOrUpdate> myAttributeAssignList, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myWhereExpression);
//        bool IsWriteTransaction(sones.Lib.Session.SessionToken mySessionToken);
//        sones.Lib.ErrorHandling.Exceptional<object> MapAndReduce(DBContext dbContext, string myDBTypeName, Func<sones.GraphDB.ObjectManagement.DBObjectMR, object> myMap, Func<object, object> myReduce);
//        sones.GraphDB.Structures.Result.QueryResult RebuilIndices(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, System.Collections.Generic.HashSet<string> myTypeNames = null);
//        sones.GraphDB.Structures.Result.QueryResult Replace(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.AAttributeAssignOrUpdate> myAttributeAssignList, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myWhereExpression);
//        sones.GraphDB.Structures.Result.QueryResult Select(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, System.Collections.Generic.Dictionary<sones.GraphDB.Managers.Structures.AExpressionDefinition, string> mySelectedElements, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.TypeReferenceDefinition> myReferenceAndTypeList, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myWhereExpressionDefinition = null, System.Collections.Generic.List<sones.GraphDB.Managers.Structures.IDChainDefinition> myGroupBy = null, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myHaving = null, sones.GraphDB.Managers.Structures.OrderByDefinition myOrderByDefinition = null, ulong? myLimit = null, ulong? myOffset = null, long myResolutionDepth = -1, bool myRunWithTimeout = false);
//        void SetNotificationDispatcher(sones.Notifications.NotificationDispatcher myNotificationDispatcher, sones.Lib.Session.SessionToken mySessionToken);
//        void SetNotificationSettings(sones.Notifications.NotificationSettings myNotificationSettings, sones.Lib.Session.SessionToken mySessionToken);
//        sones.GraphDB.Structures.Result.QueryResult Setting(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, sones.GraphDB.Structures.Enums.TypesOfSettingOperation myTypeOfSettingOperation, System.Collections.Generic.Dictionary<string, string> mySettings, sones.GraphDB.Managers.Structures.Setting.ASettingDefinition myASettingDefinition);
        void Shutdown(SessionToken mySessionToken);
//        sones.GraphDB.Structures.Result.QueryResult Truncate(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName);
//        sones.GraphDB.Structures.Result.QueryResult Update(sones.Lib.Session.SessionToken mySessionToken, DBContext mySessionContext, string myTypeName, System.Collections.Generic.HashSet<sones.GraphDB.Managers.Structures.AAttributeAssignOrUpdateOrRemove> myListOfUpdates, sones.GraphDB.Managers.Structures.BinaryExpressionDefinition myWhereExpression);
    
    }

}
