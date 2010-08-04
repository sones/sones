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


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Query.Helpers;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Settings;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using System.Threading.Tasks;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;
using sones.GraphDB.Managers.Select;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select
{
    public class SelectNode : AStatement
    {

        #region Properties

        public override string StatementName
        {
            get { return "Select"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        #endregion

        #region Data

        /// <summary>
        /// List of selected types
        /// </summary>
        List<TypeReferenceDefinition> _TypeList = null;

        /// <summary>
        /// AExpressionDefinition, Alias
        /// </summary>
        Dictionary<AExpressionDefinition, String> _SelectedElements;

        /// <summary>
        /// Group by definitions
        /// </summary>
        List<IDChainDefinition> _GroupBy;

        /// <summary>
        /// Having definition
        /// </summary>
        BinaryExpressionDefinition _Having;

        /// <summary>
        /// OrderBy section
        /// </summary>
        OrderByDefinition _OrderByDefinition = null;

        /// <summary>
        /// Limit section
        /// </summary>
        UInt64? _Limit = null;

        /// <summary>
        /// Offset section
        /// </summary>
        UInt64? _Offset = null;

        /// <summary>
        /// Resolution depth
        /// </summary>
        Int64 _ResolutionDepth = -1;

        /// <summary>
        /// The type of the output
        /// </summary>
        SelectOutputTypes _SelectOutputType = SelectOutputTypes.Tree;

        SelectResultManager _SelectResultManager = null;

        BinaryExpressionDefinition _WhereExpressionDefinition = null;

        #endregion

        #region override AStatement.GetContent

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Data

            _TypeList = new List<TypeReferenceDefinition>();
            _GroupBy = new List<IDChainDefinition>();
            _SelectedElements = new Dictionary<AExpressionDefinition, string>();

            #endregion

            #region TypeList

            foreach (ParseTreeNode aNode in parseNode.ChildNodes[1].ChildNodes)
            {
                ATypeNode aType = (ATypeNode)aNode.AstNode;

                // use the overrides equals to check duplicated references
                if (!_TypeList.Contains(aType.ReferenceAndType))
                {
                    _TypeList.Add(aType.ReferenceAndType);
                }
                else
                {
                    throw new GraphDBException(new Error_DuplicateReferenceOccurence(aType.ReferenceAndType));
                }
            }

            #endregion

            #region selList

            foreach (ParseTreeNode aNode in parseNode.ChildNodes[3].ChildNodes)
            {
                SelectionListElementNode aColumnItemNode = (SelectionListElementNode)aNode.AstNode;

                if (aColumnItemNode.IsAsterisk)
                {
                    foreach (var reference in GetTypeReferenceDefinitions(context))
                    {
                        _SelectedElements.Add(new IDChainDefinition(new ChainPartTypeOrAttributeDefinition(reference.TypeName)), null);
                    }
                    continue;
                }

                _SelectedElements.Add(aColumnItemNode.ColumnSourceValue, aColumnItemNode.AliasId);

            }

            #endregion

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                if (tempWhereNode.BinExprNode != null)
                {
                    _WhereExpressionDefinition = tempWhereNode.BinExprNode.BinaryExpressionDefinition;
                }
            }

            #endregion

            #region groupClauseOpt

            if (parseNode.ChildNodes[5].HasChildNodes() && parseNode.ChildNodes[5].ChildNodes[2].HasChildNodes())
            {
                foreach (ParseTreeNode node in parseNode.ChildNodes[5].ChildNodes[2].ChildNodes)
                {
                    _GroupBy.Add(((IDNode)node.AstNode).IDChainDefinition);
                }
            }

            #endregion

            #region havingClauseOpt

            if (parseNode.ChildNodes[6].HasChildNodes())
            {
                _Having = ((BinaryExpressionNode)parseNode.ChildNodes[6].ChildNodes[1].AstNode).BinaryExpressionDefinition;
            }

            #endregion

            #region orderClauseOpt

            if (parseNode.ChildNodes[7].HasChildNodes())
            {
                _OrderByDefinition = ((OrderByNode)parseNode.ChildNodes[7].AstNode).OrderByDefinition;
            }

            #endregion

            //#region MatchingClause

            //if (parseNode.ChildNodes[8].HasChildNodes())
            //{
            //    throw new NotImplementedException();
            //}

            //#endregion

            #region Offset

            if (parseNode.ChildNodes[9].HasChildNodes())
            {
                _Offset = ((OffsetNode)parseNode.ChildNodes[9].AstNode).Count;
            }

            #endregion

            #region Limit

            if (parseNode.ChildNodes[10].HasChildNodes())
            {
                _Limit = ((LimitNode)parseNode.ChildNodes[10].AstNode).Count;
            }

            #endregion

            #region Depth

            if (parseNode.ChildNodes[11].HasChildNodes())
            {
                _ResolutionDepth = Convert.ToUInt16(parseNode.ChildNodes[11].ChildNodes[1].Token.Value);
            }

            #endregion

            #region Select Output

            if (parseNode.ChildNodes[12].HasChildNodes())
            {
                _SelectOutputType = (parseNode.ChildNodes[12].AstNode as SelectOutputOptNode).SelectOutputType;
            }

            #endregion

        }

        #endregion

        #region override AStatement.Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {
            /////////////////////////////////////////////////////////////////////////////////////////
            // WARNING!!! graphDBSession might be null when it is executed from PartialSelectStmtNode
            /////////////////////////////////////////////////////////////////////////////////////////


            #region Start select

            var selectManager = new SelectManager();
            var runThreaded = DBConstants.UseThreadedSelect;

#if DEBUG
            runThreaded = false;
#endif

            if (runThreaded)
            {
                #region select in Task

                var selectTask = Task.Factory.StartNew(() =>
                {
                    return ExecuteSelect(dbContext, selectManager);
                });

                var timeout = Convert.ToInt32(dbContext.DBSettingsManager.GetSettingValue((new SettingSelectTimeOut()).ID, dbContext, TypesSettingScope.DB).Value.Value);

                if (selectTask.Wait(timeout))
                {
                    //within time
                    if (!selectTask.Result.Success)
                    {
                        return new QueryResult(selectTask.Result);
                    }
                    else
                    {
                        return new QueryResult(selectTask.Result.Value);
                    }
                }
                else
                {
                    return new QueryResult(new Error_SelectTimeOut(10000));
                }

                #endregion
            }
            else
            {
                #region ususal select

                var _PreResult = ExecuteSelect(dbContext, selectManager);
                if (_PreResult.Failed)
                {
                    return new QueryResult(_PreResult);
                }
                else if (!_PreResult.Success)
                {
                    return new QueryResult(_PreResult.Value, _PreResult.Warnings);
                }
                else
                {
                    return new QueryResult(_PreResult.Value);
                }

                #endregion
            }


            #endregion

        }

        private Exceptional<List<SelectionResultSet>> ExecuteSelect(DBContext myDBContext, SelectManager mySelectManager)
        {
            var types = new Dictionary<String, GraphDBType>();
            foreach (var typeRef in _TypeList)
            {
                var dbtype = myDBContext.DBTypeManager.GetTypeByName(typeRef.TypeName);
                if (dbtype == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(typeRef.TypeName));
                }
                types.Add(typeRef.Reference, dbtype);
            }

            return mySelectManager.ExecuteSelect(myDBContext, _SelectedElements, types, _WhereExpressionDefinition, _GroupBy, _Having, _OrderByDefinition, _Limit, _Offset, _ResolutionDepth);
        }


        #endregion


    }
}
