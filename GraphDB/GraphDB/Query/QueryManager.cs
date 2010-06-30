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


/* <id name="sones GraphDB – Query Manager" />
 * <copyright file="QueryManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <developer>Henning Rauch</developer>
 * <summary>This class is responsible for handling incoming queries.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.Exceptions;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;

using sones.GraphDB.ObjectManagement;
using sones.Lib.DataStructures;
using System.Diagnostics;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphDB.Warnings;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Settings;
using sones.GraphDB.Transactions;
using sones.Lib;
using GraphFSInterface.Transactions;

#endregion

namespace sones.GraphDB.Query
{
    /// <summary>
    /// This class is responsible for handling incoming queries.
    /// </summary>
    public class QueryManager
    {

        #region Data

        /// <summary>
        /// The IRONY compiler
        /// </summary>
        private         Compiler        _IronyCompiler = null;

        /// <summary>
        /// Logger
        /// </summary>
        //NLOG: temporarily commented
        //private static  Logger          //_Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region constructor

        public QueryManager()
        {

            //get grammar
            Grammar pandoraQL = new QueryLanguage.GraphQL();

            //build compiler
            this._IronyCompiler = new Compiler(pandoraQL);

            //check language
            if (this._IronyCompiler.Language.ErrorLevel != GrammarErrorLevel.NoError)
            {
                throw new GraphDBException(new Error_IronyCompiler(this._IronyCompiler.Language.Errors));
            }
        }

        #endregion

        /// <summary>
        /// Executes a query-string.
        /// </summary>
        /// <param name="query">The query-string</param>
        /// <param name="dbContext">The current dbContext which is passed to the IronyCompiler context and with this to each getContent of the nodes</param>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <returns>A QueryResult object</returns>
        public QueryResult ExecuteQuery(String query, DBContext dbContext, IGraphDBSession graphDBSession)
        {
            #region Data

            QueryResult         queryResult; //the result
            ParseTree           aTree;       //tree-like representation of the query-string
            AStatement          statement;   //executeable statement
            List<IError>        errors;      //list of errors

            #endregion

            #region Input exceptions

            if (query == null)
            {
                #region create error object

                return new QueryResult(new Error_GqlSyntax("Error! Query was null!"));

                #endregion
            }

            if (query.Length.Equals(0))
            {
                return new QueryResult(new Error_GqlSyntax("Error! Query was empty!"));
            }

            #endregion

            lock (_IronyCompiler)
	        {
                aTree = this._IronyCompiler.Parse(query, dbContext);
	        }

            #region error handling

            if (aTree == null)
            {
                queryResult = new QueryResult(new Error_GqlSyntax("Error! Query could not be parsed!"));
                return queryResult;
            }

            if (aTree.Errors.Count > 0)
            {
                errors = new List<IError>();

                if (aTree.Errors.Exists(item => item.Exception is GraphDBException))
                {
                    foreach (var aPandoraError in aTree.Errors.Where(item => item.Exception is GraphDBException))
                    {
                        errors.AddRange((aPandoraError.Exception as GraphDBException).GraphDBErrors);
                    }
                }
                else
                {
                    foreach (var aSyntaxError in aTree.Errors)
                    {
                        errors.Add(new Error_GqlSyntax(aSyntaxError, query));
                    }
                }

                queryResult = new QueryResult(errors);
                return queryResult;
            }

            #endregion

            #region Execution

            //get the statement from the tree
            statement = (AStatement)aTree.Root.AstNode;

            var isReadOnlySettingValue = IsDBSetReaddonly(dbContext);

            if (isReadOnlySettingValue.Failed)
            {
                return new QueryResult(new Error_SettingDoesNotExist(new SettingReadonly().Name));
            }

            if (isReadOnlySettingValue.Value)
            {
                if (statement.TypeOfStatement == TypesOfStatements.ReadWrite)
                {
                    return new QueryResult(new Error_ReadOnlyViolation(statement.StatementName));
                }
            }

            #region Check whether the statement is readWrite and the trnasaction is readOnly <- error!
            
            if (statement.TypeOfStatement == TypesOfStatements.ReadWrite && graphDBSession.GetLatestTransaction().IsRunning())
            {
                var latestTrans = graphDBSession.GetLatestTransaction();
                if (latestTrans.IsReadonly())
                {
                    return new QueryResult(new Error_StatementExpectsWriteTransaction(statement.StatementName, latestTrans.IsolationLevel));
                }
            }

            #endregion

            try
            {
                queryResult = statement.Execute(graphDBSession, dbContext);
            }
            catch (GraphDBWarningException we)
            {
                queryResult = new QueryResult(myErrors: new List<IError>(), myWarnings: new List<IWarning>(){ (we as GraphDBWarningException).GraphDBWarning });
                //NLOG: temporarily commented
                ////_Logger.ErrorException(query, we);
            }
            catch (GraphDBException ee)
            {
                queryResult = new QueryResult((ee as GraphDBException).GraphDBErrors);
                //NLOG: temporarily commented
                ////_Logger.ErrorException(query, ee);
            }
            catch (Exception e)
            {
                queryResult = new QueryResult(new Error_UnknownDBError(e));
                //NLOG: temporarily commented
                ////_Logger.ErrorException(query, e);
                return queryResult;
            }
            
            #endregion

            return queryResult;
        }

        private Exceptional<bool> IsDBSetReaddonly(DBContext dbContext)
        {
            return new Exceptional<bool>((bool)dbContext.DBSettingsManager.GetSettingValue(new SettingReadonly().Name, dbContext, TypesSettingScope.DB).Value.Value);

            //var readonlySetting = (new SettingReadonly()).Get(dbContext, TypesSettingScope.DB);
            //if ((readonlySetting.Failed) || (readonlySetting.Value == null))
            //{
            //    return new Exceptional<bool>(readonlySetting);
            //}

            //return new Exceptional<bool>((bool)readonlySetting.Value.Value.Value);
        }
    }
}
