/* 
 * IGraphDBSession
 * (c) sones GmbH, 2008 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ImportExport;
using sones.GraphDB.Interfaces;
using sones.GraphDB.Managers.AlterType;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.Managers.Structures.Setting;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Plugin;
using sones.GraphDB.Structures.Enums;

using sones.GraphFS.Transactions;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Notifications;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Result;
using sones.GraphDB.Transactions;
using sones.GraphDB.Managers.Select;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB
{
    /// <summary>
    /// The interface for the DB Session
    /// </summary>
    public interface IGraphDBSession
    {

        #region Notifications...

        /// <summary>
        /// Return the notification dispatcher instance for the session
        /// </summary>
        /// <returns>The notification dispatcher for the session</returns>
        NotificationDispatcher GetNotificationDispatcher();

        /// <summary>
        /// Return the settings for the notification dispatcher
        /// </summary>
        /// <returns>The notification dispatcher settings</returns>
        NotificationSettings GetNotificationSettings();

        /// <summary>
        /// Set a notification dispatcher for the session
        /// </summary>
        /// <param name="myNotificationDispatcher">Instance of a notification dispatcher</param>
        void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher);

        /// <summary>
        /// Set settings for the notification dispatcher
        /// </summary>
        /// <param name="myNotificationSettings">Settings for the notification dispatcher</param>
        void SetNotificationSettings(NotificationSettings myNotificationSettings);

        #endregion


        /// <summary>
        /// Return the database root path
        /// </summary>
        ObjectLocation DatabaseRootPath { get; }

        /// <summary>
        /// The db plugin manager
        /// </summary>
        DBPluginManager DBPluginManager { get; }

        //QueryResult             Query(String myQuery);
        //QueryResult             ActionQuery(Action<QueryResult> myCheckQuery, String myQuery);

        Exceptional<Object>     MapAndReduce(String myDBTypeName, Func<DBObjectMR, Object> myMap, Func<Object, Object> myReduce);
        IEnumerable<T2>         FilterMapReduce<T1, T2>(String myDBTypeName, Func<DBObjectMR, Boolean> myFilter, Func<DBObjectMR, T1> myMap, Func<IEnumerable<T1>, IEnumerable<T2>> myReduce);

        #region Database Management...

        /// <summary>
        /// Shutdown the database
        /// </summary>
        void Shutdown();
        
        /// <summary>
        /// Return the database unique ID
        /// </summary>
        /// <returns>The database ID</returns>
        UUID GetDatabaseUniqueID();

        #endregion

        #region DDL & DML

        #region Alter Type

        /// <summary>
        /// Execute an alter type command
        /// </summary>
        /// <param name="myTypeName">The name of the type</param>
        /// <param name="myAlterCommands">The alter type command for instance alter type add, alter type remove, alter type rename</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult AlterType(String myTypeName, List<AAlterTypeCommand> myAlterCommands);

        #endregion

        /// <summary>
        /// Create an attribute index on a type
        /// </summary>
        /// <param name="myTypeName">The name of the type for which the index should be created</param>
        /// <param name="myIndexName">The index name</param>
        /// <param name="myIndexEdition">The index edition</param>
        /// <param name="myIndexType">The index type</param>
        /// <param name="myAttributeList">The list of attributes</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult CreateIndex(String myTypeName, String myIndexName, String myIndexEdition, String myIndexType, List<IndexAttributeDefinition> myAttributeList);

        /// <summary>
        /// Create types
        /// </summary>
        /// <param name="myGraphDBTypeDefinitions">List of types to be created</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult CreateTypes(List<GraphDBTypeDefinition> myGraphDBTypeDefinitions);

        /// <summary>
        /// Delete db objects
        /// </summary>
        /// <param name="myTypeReferenceDefinitions"></param>
        /// <param name="myIDChainDefinitions"></param>
        /// <param name="myWhereExpression">The where expression, that select the objects to delete</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Delete(List<TypeReferenceDefinition> myTypeReferenceDefinitions, List<IDChainDefinition> myIDChainDefinitions, BinaryExpressionDefinition myWhereExpression);

        /// <summary>
        /// Execute a describe command
        /// </summary>
        /// <param name="myDescribeDefinition">The describe definition for example DescribeTypeDefinition, DescribeFuncDefinition, ...</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Describe(ADescribeDefinition myDescribeDefinition);

        #region Drop index

        /// <summary>
        /// Drops a index
        /// </summary>
        /// <param name="myTypeName">The typename</param>
        /// <param name="myIndexName">The indexname</param>
        /// <param name="myIndexEdition">The indexedition</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult DropIndex(String myTypeName, String myIndexName, String myIndexEdition);

        #endregion

        /// <summary>
        /// Drops a type and all dbobjects of this type
        /// </summary>
        /// <param name="myTypeName">The name of the type that is to delete</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult DropType(String myTypeName);

        /// <summary>
        /// Create a complete (query) dump in different formats
        /// </summary>
        /// <param name="myDumpFormat">The dump format</param>
        /// <param name="myDestination">The destination of the dump</param>
        /// <param name="myGrammar"></param>
        /// <param name="myTypes"></param>
        /// <param name="myDumpType"></param>
        /// <param name="verbosityType"></param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Export(String myDumpFormat, String myDestination, IDumpable myGrammar, IEnumerable<String> myTypes, DumpTypes myDumpType, VerbosityTypes verbosityType = VerbosityTypes.Errors);

        /// <summary>
        /// Import queries
        /// </summary>
        /// <param name="myImportFormat">The import format</param>
        /// <param name="myLocation">The location of the import files</param>
        /// <param name="parallelTasks"></param>
        /// <param name="comments"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="verbosityType"></param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Import(String myImportFormat, String myLocation, UInt32 parallelTasks = 1, IEnumerable<string> comments = null, UInt64? offset = null, UInt64? limit = null, VerbosityTypes verbosityType = VerbosityTypes.Errors);

        /// <summary>
        /// Adds data to a vertex
        /// </summary>
        /// <param name="myTypeName">Vertex name</param>
        /// <param name="myAttributeAssignList">The attributes with there values that should be assigned</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Insert(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList);

        /// <summary>
        /// Inserts or replaces data on a vertex
        /// </summary>
        /// <param name="myTypeName">The vertex name</param>
        /// <param name="myAttributeAssignList">The attributes with there values that should be assigned or replaced</param>
        /// <param name="myWhereExpression">The where condition</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult InsertOrReplace(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression);

        /// <summary>
        /// Inserts or updates data on a vertex
        /// </summary>
        /// <param name="myTypeName">The vertex name</param>
        /// <param name="myAttributeAssignList">The attributes with there values that should be assigned or update</param>
        /// <param name="myWhereExpression">The where condition</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult InsertOrUpdate(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression);

        /// <summary>
        /// Rebuild the indices on all types or on a list of types
        /// </summary>
        /// <param name="myTypeNames">The name of types(optional)</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult RebuildIndices(HashSet<String> myTypeNames = null);

        /// <summary>
        /// Replaces an objects
        /// </summary>
        /// <param name="myTypeName">The type of the objects</param>
        /// <param name="myAttributeAssignList">The attributes that should be replaced</param>
        /// <param name="myWhereExpression">The where condition</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Replace(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression);

        #region Select

        /// <summary>
        /// Retrieve objects or their data
        /// </summary>
        /// <param name="mySelectedElements">The selected elements</param>
        /// <param name="myReferenceAndTypeList">The dictionary hold al reference and corresponding type definition (U,User or C,Car)</param>
        /// <param name="myWhereExpressionDefinition">The where condition</param>
        /// <param name="myGroupBy">The grouping condition</param>
        /// <param name="myHaving">The having condition</param>
        /// <param name="myOrderByDefinition">The order by condition</param>
        /// <param name="myLimit">An limit of objects</param>
        /// <param name="myOffset"></param>
        /// <param name="myResolutionDepth">The resolution depth of a selection</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Select(List<Tuple<AExpressionDefinition, string, SelectValueAssignment>> mySelectedElements, List<TypeReferenceDefinition> myReferenceAndTypeList,
            BinaryExpressionDefinition myWhereExpressionDefinition = null, List<IDChainDefinition> myGroupBy = null, BinaryExpressionDefinition myHaving = null,
            OrderByDefinition myOrderByDefinition = null, UInt64? myLimit = null, UInt64? myOffset = null, Int64 myResolutionDepth = -1, Boolean myRunWithTimeout = false);

        #endregion

        /// <summary>
        /// Setting manipulation
        /// </summary>
        /// <param name="myTypeOfSettingOperation">The operation(update, remove or add values)</param>
        /// <param name="mySettings">The settings to be changed</param>
        /// <param name="myASettingDefinition">The setting</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Setting(TypesOfSettingOperation myTypeOfSettingOperation, Dictionary<string, string> mySettings, ASettingDefinition myASettingDefinition);

        #region Transactions...

        /// <summary>
        /// Starts a new transaction
        /// </summary>
        /// <param name="myDistributed">True for a distributed transaction</param>
        /// <param name="myLongRunning">True if it is a long running transaction</param>
        /// <param name="myIsolationLevel">The isolation level</param>
        /// <param name="myName">Optional transaction name</param>
        /// <param name="timestamp">Optional timestamp for the transaction</param>
        /// <returns>Return the startet transaction</returns>
        DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null);

        /// <summary>
        /// Commits a running transaction
        /// </summary>
        /// <param name="async"></param>
        /// <returns></returns>
        DBTransaction CommitTransaction(bool async = false);

        /// <summary>
        /// Revokes all changes of a transaction
        /// </summary>
        /// <param name="async"></param>
        /// <returns></returns>
        DBTransaction RollbackTransaction(bool async = false);

        /// <summary>
        /// Return the latest transaction
        /// </summary>
        /// <returns>The latest transaction</returns>
        DBTransaction GetLatestTransaction();

        #endregion

        /// <summary>
        /// Removes all records from a user-defined vertex
        /// </summary>
        /// <param name="myTypeName">The vertex name</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Truncate(String myTypeName);

        /// <summary>
        /// Updates an object within a graph
        /// </summary>
        /// <param name="myTypeName">The type to be change</param>
        /// <param name="myListOfUpdates">List of attributes to be change</param>
        /// <param name="myWhereExpression">The where condition</param>
        /// <returns>The result of the operation as QueryResult</returns>
        QueryResult Update(String myTypeName, HashSet<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, BinaryExpressionDefinition myWhereExpression);


        /// <summary>
        /// Starts a traversal and returns the found vertices.
        /// </summary>
        /// <param name="myStartVertex">The starting vertex</param>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <returns>The found vertices</returns>
        IEnumerable<IVertex> TraverseVertex(    IVertex myStartVertex,
                                                TraversalOperation TraversalOperation = TraversalOperation.BreathFirst,
                                                Func<IVertex, IEdge, Boolean> myFollowThisEdge = null,
                                                Func<IVertex, Boolean> myMatchEvaluator = null,
                                                Action<IVertex> myMatchAction = null,
                                                Func<TraversalState, Boolean> myStopEvaluator = null);
		

        #endregion

    }

}
