/*
 * SelectManager
 * (c) Stefan Licht, 2009-2010
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
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select;

namespace sones.GraphDB.Managers.Select
{
    public class SelectManager
    {

        #region ExecuteSelect

        /// <summary>
        /// Execute the current select and return a List of SelectionListElementResults
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="mySelectedElements">The selected elements and their optional alias as String</param>
        /// <param name="myTypeList">The type and reference list. This is obligatory for the validation. </param>
        /// <param name="myWhereExpressionDefinition">An optional where expression.</param>
        /// <param name="myGroupBy">An optional group by.</param>
        /// <param name="myHaving">An optional having.</param>
        /// <param name="myOrderByDefinition">An optional order by.</param>
        /// <param name="myLimit">An optional Limit.</param>
        /// <param name="myOffset">An optional offset.</param>
        /// <param name="myResolutionDepth">The resolution depth. Either set to a positive value or an setting is used.</param>
        /// <returns></returns>
        public Exceptional<List<SelectionResultSet>> ExecuteSelect(DBContext myDBContext, Dictionary<AExpressionDefinition, String> mySelectedElements, Dictionary<String,GraphDBType> myTypeList,
            BinaryExpressionDefinition myWhereExpressionDefinition = null, List<IDChainDefinition> myGroupBy = null, BinaryExpressionDefinition myHaving = null, 
            OrderByDefinition myOrderByDefinition = null, UInt64? myLimit = null, UInt64? myOffset = null, Int64 myResolutionDepth = -1)
        {

            #region Data

            List<SelectionResultSet> selectionListResults = new List<SelectionResultSet>();
            List<String> alreadyUsedTypes = new List<string>();

            //PandoraResult pResult = validateSelect();

            #endregion

            var createSelectResultManagerResult = CreateResultManager(myDBContext, mySelectedElements, myTypeList, myGroupBy, myHaving);
            if (createSelectResultManagerResult.Failed)
            {
                return new Exceptional<List<SelectionResultSet>>(createSelectResultManagerResult);
            }
            var selectResultManager = createSelectResultManagerResult.Value;

            #region Get an expressiongraph if a whereExpression exists

            IExpressionGraph exprGraph;
            if (myWhereExpressionDefinition != null)
            {
                var tempExprGraph = GetExpressionGraphFromWhere(myDBContext, myWhereExpressionDefinition);
                if (tempExprGraph.Failed)
                {
                    return new Exceptional<List<SelectionResultSet>>(tempExprGraph);
                }
                exprGraph = tempExprGraph.Value;
                selectResultManager.ExpressionGraph = exprGraph;
            }

            #endregion

            #region Set DBCache and Sessiontoken

            selectResultManager.DBObjectCache = myDBContext.DBObjectCache;
            selectResultManager.SessionToken = myDBContext.SessionSettings;

            #endregion

            #region  Go through each type end create the selectionListResults
            /// For multitype selections we need to create seperate selectionListResults
            foreach (var typeRef in myTypeList)
            {

                IEnumerable<DBObjectReadout> dbObjectReadouts = new List<DBObjectReadout>();

                #region Check and initialize groupings & aggregates

                var initGroupingOrAggregateResult = selectResultManager.InitGroupingOrAggregate(typeRef.Key, typeRef.Value);
                if (initGroupingOrAggregateResult.Failed)
                    return new Exceptional<List<SelectionResultSet>>(initGroupingOrAggregateResult.Errors);

                #endregion

                #region Check, whether the type was affected by the where expressions. This will use either the Graph or the GUID index of the type

                Boolean isInterestingWhere = (myWhereExpressionDefinition != null && IsInterestingWhereForReference(typeRef.Key, myWhereExpressionDefinition));

                #endregion

                #region Create an IEnumerable of Readouts for this typeNode

                var result = selectResultManager.Examine(myResolutionDepth, typeRef.Key, typeRef.Value, isInterestingWhere, myDBContext.DBObjectCache, myDBContext.SessionSettings, ref dbObjectReadouts);
                if (result.Failed)
                    return new Exceptional<List<SelectionResultSet>>(result);

                #endregion

                #region If this type did not returned any result, we wont add it to the result

                Boolean isValidTypeForSelect = result.Value;
                if (!isValidTypeForSelect)
                    continue;

                #endregion

                #region If there was a result for this typeNode we will add a new SelectionListElementResult

                dbObjectReadouts = selectResultManager.GetResult(typeRef.Key, typeRef.Value, dbObjectReadouts, isInterestingWhere);

                // Show typ in selection list, even if there are no results
                //if (listOfDBObjectReadouts.Count == 0)
                //  continue;

                #region OrderBy and Limit

                if (myOrderByDefinition != null)
                {

                    #region ORDER BY

                    var toSort = dbObjectReadouts.ToList();
                    toSort.Sort(delegate(DBObjectReadout dbo1, DBObjectReadout dbo2)
                    {
                        Int32 retVal = 0;
                        foreach (var attrDef in myOrderByDefinition.OrderByAttributeList)
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

                            if (myOrderByDefinition.OrderDirection == SortDirection.Desc)
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

                if (myLimit != null || myOffset != null)
                {

                    #region Limit & Offset

                    Int64 start = 0;
                    if (myOffset.HasValue)
                    {
                        start = (Int64)myOffset.Value;
                    }

                    //Int64 count = dbObjectReadouts.Count() - start;
                    if (myLimit.HasValue)
                    {
                        //count = Math.Min((Int64)_LimitNode.Count, dbObjectReadouts.Count() - start);
                        dbObjectReadouts = dbObjectReadouts.Skip((Int32)start).Take((Int32)myLimit.Value);
                    }
                    else
                    {
                        dbObjectReadouts = dbObjectReadouts.Skip((Int32)start);
                    }

                    #endregion

                }

                #endregion

                var selectionResult = new SelectionResultSet(typeRef.Value, dbObjectReadouts, selectResultManager.GetSelectedAttributesList());
                selectionListResults.Add(selectionResult);

                #endregion
            }

            #endregion

            #region TypeIndependendResults

            var listOfDBObjectReadouts = selectResultManager.GetTypeIndependendResult();
            if (listOfDBObjectReadouts.CountIsGreater(0))
            {
                var selectionResultTypeIndependend = new SelectionResultSet(listOfDBObjectReadouts);
                selectionListResults.Add(selectionResultTypeIndependend);
            }

            #endregion

            return new Exceptional<List<SelectionResultSet>>(selectionListResults);

        }

        private Exceptional<SelectResultManager> CreateResultManager(DBContext dbContext, Dictionary<AExpressionDefinition, String> _SelectedElements, Dictionary<String, GraphDBType> _TypeList, 
            List<IDChainDefinition> _GroupBy = null, BinaryExpressionDefinition _Having = null)
        {

            var _SelectResultManager = new SelectResultManager(dbContext);
            Exceptional exceptional = new Exceptional();

            foreach (var selection in _SelectedElements)
            {

                #region Add all selection elements to SelectResultManager

                if (selection.Key is IDChainDefinition)
                {

                    #region IDChainDefinition

                    IDChainDefinition idChainSelection = (IDChainDefinition)selection.Key;
                    
                    Exceptional validateResult = idChainSelection.Validate(dbContext, false);
                    exceptional.Push(validateResult);

                    if (exceptional.Failed)
                    {
                        return new Exceptional<SelectResultManager>(exceptional);
                    }

                    if (idChainSelection.IsAsterisk)
                    {

                        #region Asterisk

                        //there's no limitation
                        foreach (var typeRef in _TypeList)
                        {
                            _SelectResultManager.AddAsteriskToSelection(typeRef.Key, typeRef.Value);
                        }

                        #endregion
                        continue;
                    }

                    #region Either IDNode or parameterless function

                    if (idChainSelection.Reference == null)
                    { /// this might be a parameterless function without a calling attribute
                        exceptional.Push(_SelectResultManager.AddElementToSelection(selection.Value, null, idChainSelection, false));
                    }
                    else //if (!(aColumnItemNode.ColumnSourceValue is AggregateNode))
                    {

                        GraphDBType theType;
                        String reference = idChainSelection.Reference.Item1;
                        // this will happen, if the user selected FROM User u SELECT Name
                        if (!_TypeList.ContainsKey(reference))
                        {
                            // if there is only one type, than we can treat this as the reference
                            if (_TypeList.Count == 1)
                            {
                                theType = _TypeList.First().Value;
                            }
                            else
                            {
                                throw new Exception("Missing type reference for " + reference);
                            }
                        }
                        else
                        {
                            theType = _TypeList[reference];
                        }

                        if (idChainSelection.LastAttribute != null)
                        {
                            exceptional.Push(_SelectResultManager.AddElementToSelection(selection.Value, reference, idChainSelection, false));
                        }
                        else if (idChainSelection.IsAsterisk)
                        {
                            exceptional.Push(_SelectResultManager.AddAsteriskToSelection(reference, theType));
                        }
                        else
                        {
                            return new Exceptional<SelectResultManager>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }
                    }
                    #endregion

                    #endregion

                    if (exceptional.Failed)
                    {
                        return new Exceptional<SelectResultManager>(exceptional);
                    }

                }
                else if (selection.Key is AggregateDefinition)
                {

                    #region Aggregate

                    var aggregateSelection = selection.Key as AggregateDefinition;

                    var validateResult = aggregateSelection.ChainPartAggregateDefinition.Validate(dbContext);
                    if (validateResult.Failed)
                    {
                        return new Exceptional<SelectResultManager>(validateResult);
                    }

                    var selPartAggr = new SelectionElementAggregate(aggregateSelection.ChainPartAggregateDefinition.Aggregate, selection.Value,
                        new EdgeList(aggregateSelection.ChainPartAggregateDefinition.Parameter.Edges), new LevelKey(aggregateSelection.ChainPartAggregateDefinition.Parameter.Edges, dbContext.DBTypeManager),
                        aggregateSelection.ChainPartAggregateDefinition.Parameter, aggregateSelection);

                    validateResult = _SelectResultManager.AddAggregateElementToSelection(selection.Value, aggregateSelection.ChainPartAggregateDefinition.Parameter.Reference.Item1, selPartAggr);
                    if (validateResult.Failed)
                    {
                        return new Exceptional<SelectResultManager>(validateResult);
                    }

                    #endregion

                }
                else
                {
                    return new Exceptional<SelectResultManager>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                #endregion

            }

            #region Add groupings

            foreach (var group in _GroupBy)
            {
                var validateResult = group.Validate(dbContext, true);
                if (validateResult.Failed)
                {
                    return new Exceptional<SelectResultManager>(validateResult);
                }
                _SelectResultManager.AddGroupElementToSelection(group.Reference.Item1, group);
            }

            #endregion

            #region Add having

            if (_Having != null)
            {
                var validateResult = _Having.Validate(dbContext);
                if (validateResult.Failed)
                {
                    return new Exceptional<SelectResultManager>(validateResult);
                }

                _SelectResultManager.AddHavingToSelection(_Having);
            }

            #endregion

            return new Exceptional<SelectResultManager>(_SelectResultManager);

        }

        private bool IsInterestingWhereForReference(string reference, BinaryExpressionDefinition myWhereExpressionDefinition)
        {
            #region check left

            var leftIDNode = myWhereExpressionDefinition.Left as IDChainDefinition;

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
            else if (myWhereExpressionDefinition.Left is BinaryExpressionDefinition)
            {
                return IsInterestingWhereForReference(reference, (BinaryExpressionDefinition)myWhereExpressionDefinition.Left);
            }
            // even atom value operation needs to be evaluate. Somthing like this:
            // where 12 = 12 or where true
            // should have a valid result!
            else if (myWhereExpressionDefinition.Left is AOperationDefinition || myWhereExpressionDefinition.Left is AggregateDefinition)
            {
                return true;
            }


            #endregion

            #region check right

            var rightIDNode = myWhereExpressionDefinition.Right as IDChainDefinition;

            if (rightIDNode != null)
            {
                if (rightIDNode.Reference != null)
                {
                    if (rightIDNode.Reference.Item1.Equals(reference) || myWhereExpressionDefinition.Left is TupleDefinition)
                    {
                        return true;
                    }
                }
            }
            else if (myWhereExpressionDefinition.Right is BinaryExpressionDefinition)
            {
                return IsInterestingWhereForReference(reference, (BinaryExpressionDefinition)myWhereExpressionDefinition.Right);
            }
            // even atom value operation needs to be evaluate. Somthing like this:
            // where 12 = 12 or where true
            // should have a valid result!
            else if (myWhereExpressionDefinition.Right is AOperationDefinition || myWhereExpressionDefinition.Right is AggregateDefinition)
            {
                return true;
            }


            #endregion

            return false;
        }

        private Exceptional<IExpressionGraph> GetExpressionGraphFromWhere(DBContext dbContext, BinaryExpressionDefinition myWhereExpressionDefinition)
        {

            #region interesting where

            #region calc the flow =)

            var optimizedExpr = OptimizeBinaryExpression(myWhereExpressionDefinition);

            #endregion

            #region exec expr

            var validateResult = optimizedExpr.Validate(dbContext);
            if (validateResult.Failed)
            {
                return new Exceptional<IExpressionGraph>(validateResult);
            }
            var calculonResult = optimizedExpr.Calculon(dbContext, new CommonUsageGraph(dbContext), false);

            #endregion

            #region evaluate result

            if (calculonResult.Failed)
            {
                return new Exceptional<IExpressionGraph>(calculonResult);
            }

            #endregion

            #endregion

            return calculonResult;

        }

        #endregion

        #region OptimizeBinaryExpression

        /// <summary>
        /// Currently not implemented
        /// </summary>
        /// <returns></returns>
        private BinaryExpressionDefinition OptimizeBinaryExpression(BinaryExpressionDefinition myBinaryExpressionDefinition)
        {
            //Todo: add some functionality here

            return myBinaryExpressionDefinition;
        }

        #endregion

    }
}
