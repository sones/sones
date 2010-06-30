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
        List<ATypeNode> _TypeList = null;

        /// <summary>
        /// Dictionary of interesting attributes which are categorized by a reference string
        /// </summary>
        Dictionary<String, InterestingAttributes> _selectedClassNamesAndAttributes = null;

        /// <summary>
        /// OrderBy section
        /// </summary>
        OrderByNode _OrderByNode = null;

        /// <summary>
        /// Limit section
        /// </summary>
        LimitNode _LimitNode = null;

        /// <summary>
        /// Offset section
        /// </summary>
        OffsetNode _OffsetNode = null;

        /// <summary>
        /// Resolution depth
        /// </summary>
        Int64 _ResolutionDepth = -1;

        /// <summary>
        /// The type of the output
        /// </summary>
        SelectOutputTypes _SelectOutputType = SelectOutputTypes.Tree;

        SelectResultManager _SelectResultManager = null;

        BinaryExpressionNode _whereExpression = null;

        #endregion

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

            try
            {
                var runThreaded = DBConstants.UseThreadedSelect;

                if (runThreaded)
                {
                    #region select in Task

                    var selectTask = Task.Factory.StartNew(() =>
                    {
                        return ExecuteSelect(dbContext);
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

                    var _PreResult = ExecuteSelect(dbContext);
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
            }
            catch (GraphDBException GDBe)
            {
                return new QueryResult(GDBe.GraphDBErrors);
            }
            catch (Exception e)
            {
                if ((e.InnerException != null) && (e.InnerException is GraphDBException))
                {
                    return new QueryResult(((GraphDBException)e.InnerException).GraphDBErrors);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Execute the current select and return a List of SelectionListElementResults
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="myDBObjectCache"></param>
        /// <returns></returns>
        private Exceptional<List<SelectionResultSet>> ExecuteSelect(DBContext dbContext)
        {

            #region Data

            List<SelectionResultSet> selectionListResults = new List<SelectionResultSet>();
            List<String> alreadyUsedTypes = new List<string>();

            //PandoraResult pResult = validateSelect();

            #endregion

            #region Get an expressiongraph if a whereExpression exists

            IExpressionGraph exprGraph;
            if (_whereExpression != null)
            {
                var tempExprGraph = GetExpressionGraphFromWhere(dbContext);
                if (tempExprGraph.Failed)
                {
                    return new Exceptional<List<SelectionResultSet>>(tempExprGraph);
                }
                exprGraph = tempExprGraph.Value;
                _SelectResultManager.ExpressionGraph = exprGraph;
            }

            #endregion

            #region Set DBCache and Sessiontoken

            _SelectResultManager.DBObjectCache = dbContext.DBObjectCache;
            _SelectResultManager.SessionToken = dbContext.SessionSettings;

            #endregion

            #region  Go through each type end create the selectionListResults
            /// For multitype selections we need to create seperate selectionListResults
            foreach (ATypeNode tnode in _TypeList)
            {

                IEnumerable<DBObjectReadout> dbObjectReadouts = new List<DBObjectReadout>();

                #region Check and initialize groupings & aggregates
                /*
                var initGroupingOrAggregateResult = _SelectResultManager.InitGroupingOrAggregate(tnode);
                if (initGroupingOrAggregateResult.Failed)
                    return new Exceptional<List<SelectionListElementResult>>(initGroupingOrAggregateResult.Errors);
                */
                #endregion

                #region Check, whether the type was affected by the where expressions. This will use either the Graph or the GUID index of the type

                Boolean isInterestingWhere = (_whereExpression != null && IsInterestingWhereForReference(tnode.Reference, _whereExpression));

                #endregion

                #region Create an IEnumerable of Readouts for this typeNode

                var result = _SelectResultManager.Examine(_ResolutionDepth, tnode, isInterestingWhere, dbContext.DBObjectCache, dbContext.SessionSettings, ref dbObjectReadouts);
                if (result.Failed)
                    return new Exceptional<List<SelectionResultSet>>(result);

                #endregion

                #region If this type did not returned any result, we wont add it to the result

                Boolean isValidTypeForSelect = result.Value;
                if (!isValidTypeForSelect)
                    continue;

                #endregion

                #region If there was a result for this typeNode we will add a new SelectionListElementResult

                dbObjectReadouts = _SelectResultManager.GetResult(tnode, dbObjectReadouts, isInterestingWhere);

                // Show typ in selection list, even if there are no results
                //if (listOfDBObjectReadouts.Count == 0)
                //  continue;

                #region OrderBy and Limit

                if (_OrderByNode != null)
                {

                    #region ORDER BY

                    var toSort = dbObjectReadouts.ToList();
                    toSort.Sort(delegate(DBObjectReadout dbo1, DBObjectReadout dbo2)
                    {
                        Int32 retVal = 0;
                        foreach (var attrDef in _OrderByNode.OrderByAttributeList)
                        {
                            if (dbo1.Attributes.ContainsKey(attrDef.AsOrderByString) && dbo2.Attributes.ContainsKey(attrDef.AsOrderByString))
                            {
                                retVal = ((IComparable)dbo1.Attributes[attrDef.AsOrderByString]).CompareTo(dbo2.Attributes[attrDef.AsOrderByString]);
                            }
                            else
                            {
                                if ((!dbo1.Attributes.ContainsKey(attrDef.AsOrderByString)) && dbo2.Attributes.ContainsKey(attrDef.AsOrderByString))
                                    retVal = -1;
                                else if (dbo1.Attributes.ContainsKey(attrDef.AsOrderByString) && (!(dbo2.Attributes.ContainsKey(attrDef.AsOrderByString))))
                                    retVal = 1;
                                else
                                    retVal = 0;
                            }

                            if (_OrderByNode.OrderDirection == SortDirection.Desc)
                                retVal *= -1;

                            if (retVal != 0)
                            {
                                break;
                            }
                        }
                        return retVal;
                    }
                    );

                    dbObjectReadouts = toSort;

                    #endregion

                }

                if (_LimitNode != null || _OffsetNode != null)
                {

                    #region Limit & Offset

                    Int64 start = 0;
                    if (_OffsetNode != null)
                    {
                        start = (Int64)_OffsetNode.Count;
                    }

                    //Int64 count = dbObjectReadouts.Count() - start;
                    if (_LimitNode != null)
                    {
                        //count = Math.Min((Int64)_LimitNode.Count, dbObjectReadouts.Count() - start);
                        dbObjectReadouts = dbObjectReadouts.Skip((Int32)start).Take((Int32)_LimitNode.Count);
                    }
                    else
                    {
                        dbObjectReadouts = dbObjectReadouts.Skip((Int32)start);
                    }

                    #endregion

                }

                #endregion

                var selectionResult = new SelectionResultSet(tnode.DBTypeStream, dbObjectReadouts, _SelectResultManager.GetSelectedAttributesList());
                selectionListResults.Add(selectionResult);

                #endregion
            }

            #endregion

            #region TypeIndependendResults

            var listOfDBObjectReadouts = _SelectResultManager.GetTypeIndependendResult();
            if (listOfDBObjectReadouts.CountIsGreater(0))
            {
                var selectionResultTypeIndependend = new SelectionResultSet(listOfDBObjectReadouts);
                selectionListResults.Add(selectionResultTypeIndependend);
            }
            
            #endregion

            return new Exceptional<List<SelectionResultSet>>(selectionListResults);

        }

        private bool IsInterestingWhereForReference(string reference, BinaryExpressionNode binExpr)
        {
            #region check left

            var leftIDNode = binExpr.Left as IDNode;

            if (leftIDNode != null)
            {
                if (leftIDNode.Reference != null)
                {
                    if (leftIDNode.Reference.Item1.Equals(reference))
                    {
                        return true;
                    }
                }
            }
            else if (binExpr.Left is BinaryExpressionNode)
            {
                return IsInterestingWhereForReference(reference, (BinaryExpressionNode)binExpr.Left);
            }
            // even atom value operation needs to be evaluate. Somthing like this:
            // where 12 = 12 or where true
            // should have a valid result!
            else if (binExpr.Left is AtomValue || binExpr.Left is TupleValue || binExpr.Left is AggregateNode || binExpr.Left is FuncCallNode)
            {
                return true;
            }


            #endregion

            #region check right

            var rightIDNode = binExpr.Right as IDNode;

            if (rightIDNode != null)
            {
                if (rightIDNode.Reference != null)
                {
                    if (rightIDNode.Reference.Item1.Equals(reference) || binExpr.Left is TupleValue)
                    {
                        return true;
                    }
                }
            }
            else if (binExpr.Right is BinaryExpressionNode)
            {
                return IsInterestingWhereForReference(reference, (BinaryExpressionNode)binExpr.Right);
            }
            // even atom value operation needs to be evaluate. Somthing like this:
            // where 12 = 12 or where true
            // should have a valid result!
            else if (binExpr.Right is AtomValue || binExpr.Right is TupleValue || binExpr.Right is AggregateNode || binExpr.Right is FuncCallNode)
            {
                return true;
            }


            #endregion

            return false;
        }

        /// <summary>
        /// With this you can remove [GetDBObjectsFromWhere]
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="queryCache"></param>
        /// <returns></returns>
        private Exceptional<IExpressionGraph> GetExpressionGraphFromWhere(DBContext dbContext)
        {

            #region interesting where

            #region calc the flow =)

            var optimizedExpr = OptimizeBinaryExpression();

            #endregion

            #region exec expr

            var calculonResult = optimizedExpr.Calculon(dbContext, new CommonUsageGraph(dbContext), false);

            #endregion

            #region evaluate result

            if (calculonResult.Failed)
                return new Exceptional<IExpressionGraph>(calculonResult);

            #endregion

            #endregion

            return calculonResult;

        }

        /// <summary>
        /// Currently not implemented
        /// </summary>
        /// <returns></returns>
        private BinaryExpressionNode OptimizeBinaryExpression()
        {
            #region data

            BinaryExpressionNode optimizedExpr = new BinaryExpressionNode();

            #endregion

            //Todo: add some functionality here

            return _whereExpression;
        }


        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            #region Data

            _TypeList = new List<ATypeNode>();
            _SelectResultManager = new SelectResultManager(dbContext, _TypeList);

            #endregion

            #region TypeList

            foreach (ParseTreeNode aNode in parseNode.ChildNodes[1].ChildNodes)
            {
                ATypeNode aType = (ATypeNode)aNode.AstNode;

                // use the overrides equals to check duplicated references
                if (!_TypeList.Contains(aType))
                {
                    _TypeList.Add(aType);
                }
                else
                {
                    throw new GraphDBException(new Error_DuplicateReferenceOccurence(aType.DBTypeStream));
                }
            }

            #endregion

            #region selList

            _selectedClassNamesAndAttributes = new Dictionary<string, InterestingAttributes>();

            foreach (ParseTreeNode aNode in parseNode.ChildNodes[3].ChildNodes)
            {
                SelectionListElementNode aColumnItemNode = (SelectionListElementNode)aNode.AstNode;

                switch (aColumnItemNode.TypeOfColumnSource)
                {
                    case TypesOfColumnSource.Aggregate:
                        #region Aggregate with only 1 expression

                        if (((AggregateNode)aColumnItemNode.ColumnSourceValue).Expressions != null)
                        {
                            var aggrNode = ((AggregateNode)aColumnItemNode.ColumnSourceValue);

                            if (aggrNode.Expressions.Count > 1)
                                throw new GraphDBException(new Error_AggregateParameterCountMismatch(aggrNode.Aggregate, 1, ((AggregateNode)aColumnItemNode.ColumnSourceValue).Expressions.Count));

                            var expr = aggrNode.Expressions[0];

                            if (expr is IDNode)
                            {
                                var idnode = expr as IDNode;
                                _SelectResultManager.AddAggregateElementToSelection(aggrNode.Alias, idnode.Reference.Item1, idnode, aggrNode);
                            }
                            else
                            {
                                throw new GraphDBException(new Error_NotImplemented(new StackTrace(true)));
                            }

                        }
                        else
                        {
                            throw new GraphDBException(new Error_NotImplemented(new StackTrace(true)));
                        }

                        #endregion
                        break;

                    case TypesOfColumnSource.Asterisk:
                        #region Asterisk

                        //there's no limitation
                        foreach (var typeNode in _TypeList)
                        {
                            _SelectResultManager.AddAsteriskToSelection(typeNode);
                        }
                        #endregion
                        break;

                    default:
                        #region default, either IDNode or parameterless function

                        if (((IDNode)aColumnItemNode.ColumnSourceValue).Reference == null)
                        { /// this might be a parameterless function without a calling attribute
                            _SelectResultManager.AddElementToSelection(aColumnItemNode.AliasId, null, ((IDNode)aColumnItemNode.ColumnSourceValue), false);
                        }
                        else //if (!(aColumnItemNode.ColumnSourceValue is AggregateNode))
                        {
                            ATypeNode theType = _TypeList.Find(delegate(ATypeNode t) { return t.Reference == ((IDNode)aColumnItemNode.ColumnSourceValue).Reference.Item1; });

                            // this will happen, if the user selected FROM User u SELECT Name
                            if (theType == null)
                            {
                                // if there is only one type, than we can treat this as the reference
                                if (_TypeList.Count == 1)
                                {
                                    theType = _TypeList[0];
                                }
                                else
                                {
                                    throw new Exception("Missing type reference for " + ((IDNode)aColumnItemNode.ColumnSourceValue).Reference.Item1);
                                }
                            }

                            IDNode tempIDNode = ((IDNode)aColumnItemNode.ColumnSourceValue);

                            if (tempIDNode.LastAttribute != null)
                            {
                                _SelectResultManager.AddElementToSelection(aColumnItemNode.AliasId, theType.Reference, tempIDNode, false);
                            }
                            else if (tempIDNode.IsAsteriskSet)
                            {
                                _SelectResultManager.AddAsteriskToSelection(theType);
                            }
                            else
                            {
                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }
                        }
                        #endregion
                        break;
                }
            }

            #endregion

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                _whereExpression = tempWhereNode.BinExprNode;
            }

            #endregion

            #region groupClauseOpt

            if (parseNode.ChildNodes[5].HasChildNodes() && parseNode.ChildNodes[5].ChildNodes[2].HasChildNodes())
            {
                foreach (ParseTreeNode node in parseNode.ChildNodes[5].ChildNodes[2].ChildNodes)
                {
                    String reference = _TypeList.Find(item => item.Reference == ((IDNode)node.AstNode).Reference.Item1).Reference;
                    _SelectResultManager.AddGroupElementToSelection(reference, (IDNode)node.AstNode);
                }
            }

            #endregion

            #region Validate group and aggregates

            foreach (var typeNode in _TypeList)
            {
                var result = _SelectResultManager.InitGroupingOrAggregate(typeNode);
                if (result.Failed)
                    throw new GraphDBException(result.Errors);
            }

            #endregion

            #region havingClauseOpt

            if (parseNode.ChildNodes[6].HasChildNodes())
            {
                _SelectResultManager.AddHavingToSelection((BinaryExpressionNode)parseNode.ChildNodes[6].ChildNodes[1].AstNode);
            }

            #endregion

            #region orderClauseOpt

            if (parseNode.ChildNodes[7].HasChildNodes())
            {
                _OrderByNode = ((OrderByNode)parseNode.ChildNodes[7].AstNode);
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
                _OffsetNode = ((OffsetNode)parseNode.ChildNodes[9].AstNode);
            }

            #endregion

            #region Limit

            if (parseNode.ChildNodes[10].HasChildNodes())
            {
                _LimitNode = ((LimitNode)parseNode.ChildNodes[10].AstNode);
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

    }
}
