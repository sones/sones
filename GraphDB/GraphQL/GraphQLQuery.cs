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

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;


using sones.GraphDB.Errors;
using sones.GraphDB.GraphQL.StatementNodes;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Exceptions;

using sones.GraphDB.Warnings;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL
{

    public class GraphQLQuery
    {

        GraphQueryLanguage graphQL;
        Compiler _IronyCompiler;

        public GraphQLQuery(sones.GraphDB.Plugin.DBPluginManager pluginManager)
        {

            //get grammar
            graphQL = new GraphQueryLanguage();

            graphQL.SetFunctions(pluginManager.Functions.Values);
            graphQL.SetAggregates(pluginManager.Aggregates.Values);
            graphQL.SetGraphDBImporter(pluginManager.GraphDBImporter.Values);
            graphQL.SetIndices(pluginManager.Indices.Values);

            //build compiler
            this._IronyCompiler = new Compiler(graphQL);

            //check language
            if (this._IronyCompiler.Language.ErrorLevel != GrammarErrorLevel.NoError)
            {
                throw new GraphDBException(new Error_IronyCompiler(this._IronyCompiler.Language.Errors));
            }

        }

        private QueryResult ExecuteQuery(String query, IGraphDBSession graphDBSession)
        {

            ParseTree aTree;       //tree-like representation of the query-string
            AStatement statement;   //executeable statement
            QueryResult queryResult;
            List<IError> errors;

            #region Input exceptions - null or empty query

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

            #region Parse query

            lock (_IronyCompiler)
            {
                aTree = this._IronyCompiler.Parse(query);
            }

            #endregion

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
                    foreach (var aGraphError in aTree.Errors.Where(item => item.Exception is GraphDBException))
                    {
                        errors.AddRange((aGraphError.Exception as GraphDBException).GraphDBErrors);
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

            try
            {
                queryResult = statement.Execute(graphDBSession);
            }
            catch (GraphDBWarningException we)
            {
                queryResult = new QueryResult(new List<IError>(), new List<IWarning>(){ (we as GraphDBWarningException).GraphDBWarning });
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
        
        #region Query(QueryScript, myGraphDBSession, myQueryManager)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myQueryScript"></param>
        /// <param name="myGraphDBSession">Needed for BeginTransaction inside any AStatementNode</param>
        /// <returns></returns>
        public QueryResult Query(String myQueryScript, IGraphDBSession myGraphDBSession)
        {

            #region Data

            QueryResult _QueryResult;

            #endregion

            _QueryResult = ExecuteQuery(myQueryScript, myGraphDBSession);
            _QueryResult.Query = myQueryScript;

            return _QueryResult;

        }

        #endregion
 
    }

}
