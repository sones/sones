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
 * GraphDBSession
 * (c) sones GmbH, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using sones.GraphDB.Plugin;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Session;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphDB.Transactions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.Transactions;
using sones.Lib;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Notifications;


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

        public String DatabaseRootPath
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

        private Boolean IsWriteTransaction()
        {

            #region Check whether the statement is readWrite and the trnasaction is readOnly <- error!

            var latestTrans = GetLatestTransaction();
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

        private Exceptional VerifyReadWriteOperationIsValid(DBContext myDBContext, String myOperation = "")
        {
            var isReadOnlySettingValue = myDBContext.DBSettingsManager.GetSettingValue(new SettingReadonly().Name, myDBContext, TypesSettingScope.DB);
            if (isReadOnlySettingValue.Failed)
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

        #region IGraphDBSession Members

        #region DDL & DML

        public QueryResult AlterType(string myTypeName, List<Managers.AlterType.AAlterTypeCommand> myAlterCommands)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("AlterType", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {
                var dbInnerContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "AlterType");
                if (readWriteCheck.Failed)
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                #region check pandoraType

                var dbType = dbInnerContext.DBTypeManager.GetTypeByName(myTypeName);
                if (dbType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(myTypeName));
                }

                #endregion

                QueryResult qr = new QueryResult();

                foreach (var alterTypeCmd in myAlterCommands)
                {
                    var result = dbInnerContext.DBTypeManager.AlterType(dbInnerContext, dbType, alterTypeCmd);
                    qr.AddErrorsAndWarnings(result);

                    if (result.Value != null)
                    {
                        foreach (var resVal in result.Value.Results)
                            qr.AddResult(resVal);
                    }
                }

                #region Commit transaction and add all Warnings and Errors

                qr.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return qr;
            }
        }

        public QueryResult CreateIndex(string myTypeName, string myIndexName, string myIndexEdition, string myIndexType, List<IndexAttributeDefinition> myAttributeList)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("CreateIndex", GetLatestTransaction().IsolationLevel));
            }

            using (var _Transaction = BeginTransaction())
            {

                var transactionContext = _Transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "CreateIndex");
                if (readWriteCheck.Failed)
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
                    qresult.AddResult(resultOutput.Value);
                }

                return qresult;

            }

        }

        public QueryResult CreateTypes(List<GraphDBTypeDefinition> myGraphDBTypeDefinitions)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("CreateTypes", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var transactionContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "CreateTypes");
                if (readWriteCheck.Failed)
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var result = transactionContext.DBTypeManager.AddBulkTypes(myGraphDBTypeDefinitions, transaction.GetDBContext());

                if (!result.Success)
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

        public QueryResult Delete(List<TypeReferenceDefinition> myTypeReferenceDefinitions, List<IDChainDefinition> myIDChainDefinitions, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Delete", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "Delete");
                if (readWriteCheck.Failed)
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion

                var _TypeWithUndefAttrs = new Dictionary<GraphDBType, List<string>>();
                var _DBTypeAttributeToDelete = new Dictionary<GraphDBType, List<TypeAttribute>>();

                var _ReferenceTypeLookup = GetTypeReferenceLookup(dbInnerContext, myTypeReferenceDefinitions);
                if (_ReferenceTypeLookup.Failed)
                {
                    return new QueryResult(_ReferenceTypeLookup);
                }

                foreach (var id in myIDChainDefinitions)
                {

                    id.Validate(dbInnerContext, _ReferenceTypeLookup.Value, true);
                    if (id.ValidateResult.Failed)
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

        public QueryResult Describe(ADescribeDefinition myDescribeDefinition)
        {

            using (var transaction = BeginTransaction())
            {

                var transactionContext = transaction.GetDBContext();

                var result = myDescribeDefinition.GetResult(transactionContext);
                QueryResult qresult = null;

                if (result.Failed)
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

        public QueryResult DropIndex(String myTypeName, String myIndexName, String myIndexEdition)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("DropIndex", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var transactionContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "DropIndex");
                if (readWriteCheck.Failed)
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

                if (!RemoveIdxException.Success)
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

        public QueryResult DropType(string myTypeName)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("DropType", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {


                var transactionContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "DropType");
                if (readWriteCheck.Failed)
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

                if (!removeExcept.Success)
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

        public QueryResult Export(string myDumpFormat, string myDestination, IDumpable myGrammar, IEnumerable<string> myTypes, ImportExport.DumpTypes myDumpType, ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            var dumpReadout = new Dictionary<String, Object>();
            AGraphDBExport exporter;

            using (var transaction = BeginTransaction(myIsolationLevel: GraphFS.Transactions.IsolationLevel.ReadCommitted))
            {
                
                var transactionContext = transaction.GetDBContext();
                                
                if (!transactionContext.DBPluginManager.HasGraphDBExporter(myDumpFormat))
                {
                    throw new GraphDBException(new Error_ExporterDoesNotExist(myDumpFormat));
                }

                exporter = transactionContext.DBPluginManager.GetGraphDBExporter(myDumpFormat);

                return exporter.Export(myDestination, transactionContext, myGrammar, myTypes, myDumpType, myVerbosityType);

            }

        }

        public QueryResult Import(string myImportFormat, string myLocation, uint myParallelTasks = 1, IEnumerable<string> myComments = null, ulong? myOffset = null, ulong? myLimit = null, ImportExport.VerbosityTypes myVerbosityType = VerbosityTypes.Errors)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Import", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var transactionContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "Import");
                if (readWriteCheck.Failed)
                {
                    return new QueryResult(readWriteCheck);
                }

                #endregion
                
                if (!transactionContext.DBPluginManager.HasGraphDBImporter(myImportFormat))
                {
                    throw new GraphDBException(new Error_ImporterDoesNotExist(myImportFormat));
                }

                var importer = transactionContext.DBPluginManager.GetGraphDBImporter(myImportFormat);
                var importResult = importer.Import(myLocation, this, transactionContext, myParallelTasks, myComments, myOffset, myLimit, myVerbosityType);

                if (importResult.ResultType == Structures.ResultType.Successful)
                {
                    importResult.AddErrorsAndWarnings(transaction.Commit());
                }

                return importResult;
            }

        }

        public QueryResult Insert(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Insert", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "Insert");
                if (readWriteCheck.Failed)
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
                if (!attrsResult.Success)
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

        public QueryResult InsertOrReplace(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("InsertOrReplace", GetLatestTransaction().IsolationLevel));
            }

            #region Data

            var objectManipulationManager = new ObjectManipulationManager();

            #endregion

            using (var transaction = BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "InsertOrReplace");
                if (readWriteCheck.Failed)
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

        public QueryResult InsertOrUpdate(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("InsertOrUpdate", GetLatestTransaction().IsolationLevel));
            }

            var _ObjectManipulationManager = new ObjectManipulationManager();
            using (var transaction = BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "InsertOrUpdate");
                if (readWriteCheck.Failed)
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

        public QueryResult RebuilIndices(HashSet<string> myTypeNames = null)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("RebuilIndices", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                Exceptional<Boolean> rebuildResult = null;
                IEnumerable<GraphDBType> typesToRebuild;
                QueryResult result = new QueryResult();

                var transactionContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "RebuilIndices");
                if (readWriteCheck.Failed)
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
                        var type = transaction.GetDBContext().DBTypeManager.GetTypeByName(typeName);
                        if (type == null)
                        {
                            return new QueryResult(new Errors.Error_TypeDoesNotExist(typeName));
                        }
                        (typesToRebuild as HashSet<GraphDBType>).Add(type);
                    }

                    #endregion

                }

                rebuildResult = transaction.GetDBContext().DBIndexManager.RebuildIndices(typesToRebuild);

                if (!rebuildResult.Success)
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

        public QueryResult Replace(string myTypeName, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Replace", GetLatestTransaction().IsolationLevel));
            }

            #region Data

            var _ObjectManipulationManager = new ObjectManipulationManager();

            #endregion

            using (var transaction = BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(dbInnerContext, "Replace");
                if (readWriteCheck.Failed)
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

        public QueryResult Select(Dictionary<AExpressionDefinition, string> mySelectedElements, List<TypeReferenceDefinition> myReferenceAndTypeList,
            BinaryExpressionDefinition myWhereExpressionDefinition = null, List<IDChainDefinition> myGroupBy = null, BinaryExpressionDefinition myHaving = null, OrderByDefinition myOrderByDefinition = null,
            ulong? myLimit = null, ulong? myOffset = null, long myResolutionDepth = -1, Boolean myRunWithTimeout = false)
        {

            using (var transaction = BeginTransaction(myIsolationLevel: IsolationLevel.ReadCommitted))
            {

                var selectManager = new SelectManager();
                var transactionContext = transaction.GetDBContext();

                if (myRunWithTimeout)
                {

                    var timeout = Convert.ToInt32(transactionContext.DBSettingsManager.GetSettingValue((new SettingSelectTimeOut()).ID, transactionContext, TypesSettingScope.DB).Value.Value);

                    #region select in Task

                    var selectTask = Task.Factory.StartNew(() =>
                    {
                        return selectManager.ExecuteSelect(transaction.GetDBContext(), mySelectedElements, myReferenceAndTypeList, myWhereExpressionDefinition, myGroupBy, myHaving,
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

                    var qresult = selectManager.ExecuteSelect(transaction.GetDBContext(), mySelectedElements, myReferenceAndTypeList, myWhereExpressionDefinition, myGroupBy, myHaving,
                        myOrderByDefinition, myLimit, myOffset, myResolutionDepth);
                    qresult.AddErrorsAndWarnings(transaction.Commit());
                    return qresult;

                }

            }

        }


        public QueryResult Setting(TypesOfSettingOperation myTypeOfSettingOperation, Dictionary<string, string> mySettings, ASettingDefinition myASettingDefinition)
        {

            if (myTypeOfSettingOperation != TypesOfSettingOperation.GET && !IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Setting", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var transactionContext = transaction.GetDBContext();

                var result = transactionContext.DBSettingsManager.ExecuteSettingOperation(transaction.GetDBContext(), myASettingDefinition, myTypeOfSettingOperation, mySettings);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;
            }

        }

        public QueryResult Truncate(string myTypeName)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Truncate", GetLatestTransaction().IsolationLevel));
            }

            using (var _Transaction = BeginTransaction())
            {

                var _DBInnerContext = _Transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(_DBInnerContext, "Truncate");
                if (readWriteCheck.Failed)
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
                if (truncateResult.Failed)
                {
                    return new QueryResult(truncateResult);
                }

                #region Commit transaction and add all Warnings and Errors

                var queryResult = new QueryResult(_Transaction.Commit().Push(truncateResult));

                #endregion

                return queryResult;

            }

        }

        public QueryResult Update(string myTypeName, HashSet<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, BinaryExpressionDefinition myWhereExpression)
        {

            if (!IsWriteTransaction())
            {
                return new QueryResult(new Error_StatementExpectsWriteTransaction("Update", GetLatestTransaction().IsolationLevel));
            }

            using (var transaction = BeginTransaction())
            {

                var transactionContext = transaction.GetDBContext();

                #region Verify that DB is not set to readonly

                var readWriteCheck = VerifyReadWriteOperationIsValid(transactionContext, "Update");
                if (readWriteCheck.Failed)
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

        #region Transactions

        public DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {
            return _GraphDB.BeginTransaction(_DBContext, _SessionToken, myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);
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
