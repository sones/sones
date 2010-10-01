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

#endregion

namespace sones.GraphDB
{

    public interface IGraphDBSession
    {

        #region Notifications...

        NotificationDispatcher GetNotificationDispatcher();
        NotificationSettings GetNotificationSettings();

        void SetNotificationDispatcher(NotificationDispatcher myNotificationDispatcher);

        void SetNotificationSettings(NotificationSettings myNotificationSettings);

        #endregion

        ObjectLocation DatabaseRootPath { get; }
        DBPluginManager DBPluginManager { get; }

        //QueryResult             Query(String myQuery);
        //QueryResult             ActionQuery(Action<QueryResult> myCheckQuery, String myQuery);
        Exceptional<Object>     MapAndReduce(String myDBTypeName, Func<DBObjectMR, Object> myMap, Func<Object, Object> myReduce);
        IEnumerable<T2>         FilterMapReduce<T1, T2>(String myDBTypeName, Func<DBObjectMR, Boolean> myFilter, Func<DBObjectMR, T1> myMap, Func<IEnumerable<T1>, IEnumerable<T2>> myReduce);

        #region Database Management...

        void Shutdown();
        
        UUID GetDatabaseUniqueID();

        #endregion

        #region DDL & DML

        #region Alter Type

        QueryResult AlterType(String myTypeName, List<AAlterTypeCommand> myAlterCommands);

        #endregion

        QueryResult CreateIndex(String myTypeName, String myIndexName, String myIndexEdition, String myIndexType, List<IndexAttributeDefinition> myAttributeList);

        QueryResult CreateTypes(List<GraphDBTypeDefinition> myGraphDBTypeDefinitions);

        QueryResult Delete(List<TypeReferenceDefinition> myTypeReferenceDefinitions, List<IDChainDefinition> myIDChainDefinitions, BinaryExpressionDefinition myWhereExpression);

        QueryResult Describe(ADescribeDefinition myDescribeDefinition);

        #region Drop index

        QueryResult DropIndex(String myTypeName, String myIndexName, String myIndexEdition);

        #endregion

        QueryResult DropType(String myTypeName);

        QueryResult Export(String myDumpFormat, String myDestination, IDumpable myGrammar, IEnumerable<String> myTypes, DumpTypes myDumpType, VerbosityTypes verbosityType = VerbosityTypes.Errors);

        QueryResult Import(String myImportFormat, String myLocation, UInt32 parallelTasks = 1, IEnumerable<string> comments = null, UInt64? offset = null, UInt64? limit = null, VerbosityTypes verbosityType = VerbosityTypes.Errors);

        QueryResult Insert(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList);

        QueryResult InsertOrReplace(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression);

        QueryResult InsertOrUpdate(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression);

        QueryResult RebuildIndices(HashSet<String> myTypeNames = null);

        QueryResult Replace(String myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression);

        #region Select

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mySelectedElements"></param>
        /// <param name="myReferenceAndTypeList">The dictionary hold al reference and corresponding type definition (U,User or C,Car)</param>
        /// <param name="myWhereExpressionDefinition"></param>
        /// <param name="myGroupBy"></param>
        /// <param name="myHaving"></param>
        /// <param name="myOrderByDefinition"></param>
        /// <param name="myLimit"></param>
        /// <param name="myOffset"></param>
        /// <param name="myResolutionDepth"></param>
        /// <returns></returns>
        QueryResult Select(List<Tuple<AExpressionDefinition, string, SelectValueAssignment>> mySelectedElements, List<TypeReferenceDefinition> myReferenceAndTypeList,
            BinaryExpressionDefinition myWhereExpressionDefinition = null, List<IDChainDefinition> myGroupBy = null, BinaryExpressionDefinition myHaving = null,
            OrderByDefinition myOrderByDefinition = null, UInt64? myLimit = null, UInt64? myOffset = null, Int64 myResolutionDepth = -1, Boolean myRunWithTimeout = false);

        #endregion

        QueryResult Setting(TypesOfSettingOperation myTypeOfSettingOperation, Dictionary<string, string> mySettings, ASettingDefinition myASettingDefinition);

        #region Transactions...

        DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null);
        DBTransaction CommitTransaction(bool async = false);
        DBTransaction RollbackTransaction(bool async = false);
        DBTransaction GetLatestTransaction();

        #endregion

        QueryResult Truncate(String myTypeName);

        QueryResult Update(String myTypeName, HashSet<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, BinaryExpressionDefinition myWhereExpression);

        #endregion

    }

}
