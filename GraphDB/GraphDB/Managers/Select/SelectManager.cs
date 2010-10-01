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
 * SelectManager
 * (c) Stefan Licht, 2009-2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;
using sones.GraphDB.Errors;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.Select
{

    public class SelectManager
    {

        #region SelectManager

        #region ExecuteSelect

        #region ExecuteSelect(myDBContext, mySelectDefinition)

        public QueryResult ExecuteSelect(DBContext myDBContext, SelectDefinition mySelectDefinition)
        {
            return ExecuteSelect(myDBContext,
                                 mySelectDefinition.SelectedElements,
                                 mySelectDefinition.TypeList,
                                 mySelectDefinition.WhereExpressionDefinition,
                                 mySelectDefinition.GroupByIDs,
                                 mySelectDefinition.Having,
                                 mySelectDefinition.OrderByDefinition,
                                 mySelectDefinition.Limit,
                                 mySelectDefinition.Offset,
                                 mySelectDefinition.ResolutionDepth);
        }

        #endregion

        #region ExecuteSelect(...)

        /// <summary>
        /// Execute the current select and return a List of SelectionListElementResults
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="mySelectedElements">The selected elements and their optional alias as String</param>
        /// <param name="myTypeList">The dictionary hold al reference and corresponding type definition (U,User or C,Car). </param>
        /// <param name="myWhereExpressionDefinition">An optional where expression.</param>
        /// <param name="myGroupBy">An optional group by.</param>
        /// <param name="myHaving">An optional having.</param>
        /// <param name="myOrderByDefinition">An optional order by.</param>
        /// <param name="myLimit">An optional Limit.</param>
        /// <param name="myOffset">An optional offset.</param>
        /// <param name="myResolutionDepth">The resolution depth. Either set to a positive value or an setting is used.</param>
        /// <returns></returns>
        public QueryResult ExecuteSelect(DBContext                      myDBContext,
                                         List<Tuple<AExpressionDefinition, String, SelectValueAssignment>> mySelectedElements,
                                         List<TypeReferenceDefinition>  myTypeList,
                                         BinaryExpressionDefinition     myWhereExpressionDefinition = null,
                                         List<IDChainDefinition>        myGroupBy = null,
                                         BinaryExpressionDefinition     myHaving = null,
                                         OrderByDefinition              myOrderByDefinition = null,
                                         UInt64?                        myLimit = null,
                                         UInt64?                        myOffset = null,
                                         Int64                          myResolutionDepth = -1)
        {

            #region Data

            List<String> alreadyUsedTypes = new List<string>();

            //GraphResult pResult = validateSelect();

            #endregion

            #region Resolve types

            var typeList = ResolveTypes(myTypeList, myDBContext.DBTypeManager);
            if (typeList.Failed())
            {
                return new QueryResult(typeList);
            }

            #endregion

            #region Create SelectResultManager which will do the lazy select

            var createSelectResultManagerResult = CreateResultManager(myDBContext, mySelectedElements, typeList.Value, myGroupBy, myHaving);

            if (createSelectResultManagerResult.Failed())
            {
                return new QueryResult(createSelectResultManagerResult);
            }
            var selectResultManager = createSelectResultManagerResult.Value;

            #endregion

            #region Get an expressiongraph if a whereExpression exists

            IExpressionGraph exprGraph;

            if (myWhereExpressionDefinition != null)
            {
                var tempExprGraph = GetExpressionGraphFromWhere(myDBContext, myWhereExpressionDefinition);

                if (tempExprGraph.Failed())
                {
                    return new QueryResult(tempExprGraph);
                }

                exprGraph = tempExprGraph.Value;
                selectResultManager.ExpressionGraph = exprGraph;
            }

            #endregion

            var _ListOfVertices1 = CreateVertices(myDBContext, typeList.Value, selectResultManager, myWhereExpressionDefinition, myResolutionDepth, myOrderByDefinition, myLimit, myOffset);
            if (_ListOfVertices1.Failed())
            {
                return new QueryResult(_ListOfVertices1);
            }

            #region TypeIndependendResults

            var _ListOfVertices2 = selectResultManager.GetTypeIndependendResult();
            if (_ListOfVertices2.CountIsGreater(0))
            {
                _ListOfVertices1.Value.Add(_ListOfVertices2);
            }

            #endregion

            return new QueryResult(AggregateListOfVertices(_ListOfVertices1.Value));

        }

        #endregion

        #endregion

        #region CreateVertices for all selected types

        /// <summary>
        /// Creates all readouts of all types in the <paramref name="myTypeList"/>
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="myTypeList"></param>
        /// <param name="mySelectResultManager"></param>
        /// <param name="myWhereExpressionDefinition"></param>
        /// <param name="myResolutionDepth"></param>
        /// <param name="myOrderByDefinition"></param>
        /// <param name="myLimit"></param>
        /// <param name="myOffset"></param>
        /// <returns></returns>
        private Exceptional<List<IEnumerable<Vertex>>> CreateVertices(DBContext myDBContext,
                                                                      Dictionary<String, GraphDBType> myTypeList,
                                                                      SelectResultManager mySelectResultManager,
                                                                      BinaryExpressionDefinition myWhereExpressionDefinition,
                                                                      Int64 myResolutionDepth,
                                                                      OrderByDefinition myOrderByDefinition,
                                                                      UInt64? myLimit,
                                                                      UInt64? myOffset)
        {

            var _ListOfListOfVertices = new List<IEnumerable<Vertex>>();

            // Create vertices for type
            foreach (var typeRef in myTypeList)
            {

                var _Vertices = CreateVerticesForType(myDBContext,
                                                      typeRef,
                                                      mySelectResultManager,
                                                      myWhereExpressionDefinition,
                                                      myResolutionDepth,
                                                      myOrderByDefinition,
                                                      myLimit,
                                                      myOffset);

                if (_Vertices.Failed())
                {
                    return new Exceptional<List<IEnumerable<Vertex>>>(_Vertices);
                }

                // If this type did not returned any result, we wont add it to the result
                if (_Vertices.Value != null)
                {
                    _ListOfListOfVertices.Add(_Vertices.Value);
                }

            }

            return new Exceptional<List<IEnumerable<Vertex>>>(_ListOfListOfVertices);

        }
        
        #endregion

        #region CreateVerticesForType - for a particular type

        /// <summary>
        /// Creates the lazy readouts for the given <paramref name="myTypeReference"/>
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="myTypeReference"></param>
        /// <param name="mySelectResultManager"></param>
        /// <param name="myWhereExpressionDefinition"></param>
        /// <param name="myResolutionDepth"></param>
        /// <param name="myOrderByDefinition"></param>
        /// <param name="myLimit"></param>
        /// <param name="myOffset"></param>
        /// <returns></returns>
        private Exceptional<IEnumerable<Vertex>> CreateVerticesForType(DBContext myDBContext,
                                                                       KeyValuePair<String, GraphDBType> myTypeReference,
                                                                       SelectResultManager mySelectResultManager,
                                                                       BinaryExpressionDefinition myWhereExpressionDefinition,
                                                                       Int64 myResolutionDepth,
                                                                       OrderByDefinition myOrderByDefinition,
                                                                       UInt64? myLimit,
                                                                       UInt64? myOffset)
        {

            IEnumerable<Vertex> _Vertices = new List<Vertex>();

            #region Check groupings & aggregates

            var initGroupingOrAggregateResult = mySelectResultManager.ValidateGroupingAndAggregate(myTypeReference.Key, myTypeReference.Value);

            if (initGroupingOrAggregateResult.Failed())
            {
                return new Exceptional<IEnumerable<Vertex>>(initGroupingOrAggregateResult);
            }

            #endregion

            #region Check, whether the type was affected by the where expressions. This will use either the Graph or the GUID index of the type

            Boolean isInterestingWhere = (myWhereExpressionDefinition != null && IsInterestingWhereForReference(myTypeReference.Key, myWhereExpressionDefinition));

            #endregion

            #region Create an IEnumerable of Readouts for this typeNode

            var result = mySelectResultManager.Examine(myResolutionDepth, myTypeReference.Key, myTypeReference.Value, isInterestingWhere, ref _Vertices);

            if (result.Failed())
            {
                return new Exceptional<IEnumerable<Vertex>>(result);
            }

            #endregion

            #region If this type did not returned any result, we wont add it to the result

            Boolean isValidTypeForSelect = result.Value;

            if (!isValidTypeForSelect)
            {
                return new Exceptional<IEnumerable<Vertex>>();
            }

            #endregion

            #region If there was a result for this typeNode we will add a new SelectionListElementResult

            _Vertices = mySelectResultManager.GetResult(myTypeReference.Key, myTypeReference.Value, _Vertices, isInterestingWhere);

            #region OrderBy

            if (myOrderByDefinition != null)
            {

                var orderResult = OrderVertices(myDBContext, myOrderByDefinition, ref _Vertices);
                if (orderResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(orderResult);
                }

            }

            #endregion

            #region Limit

            if (myLimit != null || myOffset != null)
            {
                ApplyLimitAndOffset(myLimit, myOffset, ref _Vertices);
            }

            #endregion


            #endregion

            return new Exceptional<IEnumerable<Vertex>>(_Vertices);

        }
        
        #endregion

        #region ApplyLimitAndOffset

        /// <summary>
        /// Applys a limit (if not null) and an offset (if not null) to the <paramref name="myVertices"/>
        /// </summary>
        /// <param name="myLimit"></param>
        /// <param name="myOffset"></param>
        /// <param name="myVertices"></param>
        private void ApplyLimitAndOffset(ulong? myLimit, ulong? myOffset, ref IEnumerable<Vertex> myVertices)
        {

            Int64 start = 0;
            if (myOffset.HasValue)
            {
                start = (Int64)myOffset.Value;
            }

            //Int64 count = dbObjectReadouts.Count() - start;
            if (myLimit.HasValue)
            {
                //count = Math.Min((Int64)_LimitNode.Count, dbObjectReadouts.Count() - start);
                myVertices = myVertices.Skip((Int32)start).Take((Int32)myLimit.Value);
            }
            else
            {
                myVertices = myVertices.Skip((Int32)start);
            }

        }
        
        #endregion

        #region OrderVertices

        /// <summary>
        /// Orders the readouts
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="myOrderByDefinition"></param>
        /// <param name="myVertices"></param>
        /// <returns></returns>
        private Exceptional OrderVertices(DBContext myDBContext, OrderByDefinition myOrderByDefinition, ref IEnumerable<Vertex> myVertices)
        {

            #region Set the as alias for all not set ones to the attribute name

            foreach (var attrDef in myOrderByDefinition.OrderByAttributeList)
            {
                if (attrDef.IDChainDefinition != null)
                {
                    var validateResult = attrDef.IDChainDefinition.Validate(myDBContext, true);
                    if (validateResult.Failed())
                    {
                        return new Exceptional(validateResult);
                    }

                    if (String.IsNullOrEmpty(attrDef.AsOrderByString))
                    {

                        if (attrDef.IDChainDefinition.IsUndefinedAttribute)
                        {
                            attrDef.AsOrderByString = attrDef.IDChainDefinition.UndefinedAttribute;
                        }
                        else
                        {
                            attrDef.AsOrderByString = attrDef.IDChainDefinition.LastAttribute.Name;
                        }
                    }
                }
            }

            #endregion

            #region ORDER BY

            var comparer = new Vertices_OrderByComparer(myOrderByDefinition);
            if (myOrderByDefinition.OrderDirection == SortDirection.Asc)
            {
                myVertices = myVertices.OrderBy(dbo => dbo, comparer);
            }
            else
            {
                myVertices = myVertices.OrderByDescending(dbo => dbo, comparer);
            }

            #endregion

            return new Exceptional();

        }

        #endregion

        #region Vertices_OrderByComparer

        /// <summary>
        /// A comparer for a OrderByDefinition
        /// </summary>
        class Vertices_OrderByComparer : IComparer<Vertex>
        {

            OrderByDefinition _OrderByDefinition;

            public Vertices_OrderByComparer(OrderByDefinition myOrderByDefinition)
            {
                _OrderByDefinition = myOrderByDefinition;
            }

            #region IComparer<Vertex> Members

            public int Compare(Vertex myVertex1, Vertex myVertex2)
            {

                Int32 retVal = 0;

                foreach (var attrDef in _OrderByDefinition.OrderByAttributeList)
                {

                    if (myVertex1.HasAttribute(attrDef.AsOrderByString) && myVertex2.HasAttribute(attrDef.AsOrderByString))
                    {
                        retVal = ((IComparable)myVertex1.GetProperty(attrDef.AsOrderByString)).CompareTo(myVertex2.GetProperty(attrDef.AsOrderByString));
                    }

                    else
                    {
                        if ((!myVertex1.HasAttribute(attrDef.AsOrderByString)) && myVertex2.HasAttribute(attrDef.AsOrderByString))
                            retVal = -1;
                        else if (myVertex1.HasAttribute(attrDef.AsOrderByString) && (!(myVertex2.HasAttribute(attrDef.AsOrderByString))))
                            retVal = 1;
                        else
                            retVal = 0;
                    }

                    if (retVal != 0)
                    {
                        break;
                    }

                }

                return retVal;

            }

            #endregion

        }
        
        #endregion

        #region ResolveTypes

        /// <summary>
        /// Resolves the TypeReferenceDefinition list to a dictionary of reference (coming from the FROM) and corresponding type
        /// </summary>
        /// <param name="myTypeList"></param>
        /// <param name="myDBTypeManager"></param>
        /// <returns></returns>
        private Exceptional<Dictionary<String, GraphDBType>> ResolveTypes(List<TypeReferenceDefinition> myTypeList, DBTypeManager myDBTypeManager)
        {

            var typeList = new Dictionary<String, GraphDBType>();
            
            foreach (var type in myTypeList)
            {

                var t = myDBTypeManager.GetTypeByName(type.TypeName);

                if (t == null)
                {
                    return new Exceptional<Dictionary<String, GraphDBType>>(new Error_TypeDoesNotExist(type.TypeName));
                }

                typeList.Add(type.Reference, t);
            
            }
            
            return new Exceptional<Dictionary<String, GraphDBType>>(typeList);

        }

        #endregion

        #region AggregateListOfVertices

        /// <summary>
        /// Aggregates different enumerations of vertices
        /// </summary>
        /// <param name="myListOfListOfVertices"></param>
        /// <returns></returns>
        private IEnumerable<Vertex> AggregateListOfVertices(List<IEnumerable<Vertex>> myListOfListOfVertices)
        {

            foreach (var _ListOfVertices in myListOfListOfVertices)
            {
                foreach (var _Vertex in _ListOfVertices)
                {
                    yield return _Vertex;
                }
            }

            yield break;

        }

        #endregion

        #region CreateResultManager

        /// <summary>
        /// Creates the result manager and adds all selected elements
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="mySelectedElements"></param>
        /// <param name="myTypeList"></param>
        /// <param name="myGroupBy"></param>
        /// <param name="myHaving"></param>
        /// <returns></returns>
        private Exceptional<SelectResultManager> CreateResultManager(DBContext myDBContext,
                                                                     List<Tuple<AExpressionDefinition, String, SelectValueAssignment>> mySelectedElements,
                                                                     Dictionary<String, GraphDBType> myTypeList,
                                                                     List<IDChainDefinition> myGroupBy = null,
                                                                     BinaryExpressionDefinition myHaving = null)
        {

            var _SelectResultManager = new SelectResultManager(myDBContext);

            Exceptional exceptional = new Exceptional();

            foreach (var selection in mySelectedElements)
            {

                #region Add all selection elements to SelectResultManager

                if (selection.Item1 is IDChainDefinition)
                {

                    #region IDChainDefinition

                    var idChainSelection = (selection.Item1 as IDChainDefinition);

                    Exceptional validateException = idChainSelection.Validate(myDBContext, true);

                    exceptional.PushIExceptional(validateException);
                    if (exceptional.Failed())
                    {
                        return new Exceptional<SelectResultManager>(exceptional);
                    }

                    if (idChainSelection.SelectType != TypesOfSelect.None)
                    {

                        #region Asterisk, Minus, Rhomb, Ad

                        //there's no limitation
                        foreach (var typeRef in myTypeList)
                        {
                            if (idChainSelection.TypeName != null)
                            {
                                var type = myDBContext.DBTypeManager.GetTypeByName(idChainSelection.TypeName);

                                if (type == null)
                                {
                                    return new Exceptional<SelectResultManager>(new Error_TypeDoesNotExist(idChainSelection.TypeName));
                                }

                                TypeUUID typeID = type.UUID;
                                _SelectResultManager.AddSelectionType(typeRef.Key, typeRef.Value, idChainSelection.SelectType, typeID);
                            }
                            else
                            {
                                _SelectResultManager.AddSelectionType(typeRef.Key, typeRef.Value, idChainSelection.SelectType);
                            }
                        }

                        #endregion

                        continue;
                    }

                    #region Either IDNode or parameterless function

                    if (idChainSelection.Reference == null)
                    { /// this might be a parameterless function without a calling attribute
                        exceptional.PushIExceptional(_SelectResultManager.AddElementToSelection(selection.Item2, null, idChainSelection, false));
                    }
                    else //if (!(aColumnItemNode.ColumnSourceValue is AggregateNode))
                    {

                        GraphDBType theType;
                        String reference = idChainSelection.Reference.Item1;
                        // this will happen, if the user selected FROM User u SELECT Name
                        if (!myTypeList.ContainsKey(reference))
                        {
                            // if there is only one type, than we can treat this as the reference
                            if (myTypeList.Count == 1)
                            {
                                theType = myTypeList.First().Value;
                            }
                            else
                            {
                                throw new Exception("Missing type reference for " + reference);
                            }
                        }
                        else
                        {
                            theType = myTypeList[reference];
                        }

                        if (idChainSelection.SelectType != TypesOfSelect.None)
                        {
                            exceptional.PushIExceptional(_SelectResultManager.AddSelectionType(reference, theType, idChainSelection.SelectType));
                        }
                        else
                        {
                            exceptional.PushIExceptional(_SelectResultManager.AddElementToSelection(selection.Item2, reference, idChainSelection, false, selection.Item3));
                        }
                        //else
                        //{
                        //    return new Exceptional<SelectResultManager>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        //}
                    }
                    #endregion

                    #endregion

                    if (exceptional.Failed())
                    {
                        return new Exceptional<SelectResultManager>(exceptional);
                    }

                }

                else if (selection.Item1 is AggregateDefinition)
                {

                    #region Aggregate

                    var aggregateSelection = selection.Item1 as AggregateDefinition;

                    var validateResult = aggregateSelection.ChainPartAggregateDefinition.Validate(myDBContext);
                    if (validateResult.Failed())
                    {
                        return new Exceptional<SelectResultManager>(validateResult);
                    }

                    var selPartAggr = new SelectionElementAggregate(aggregateSelection.ChainPartAggregateDefinition.Aggregate, selection.Item2,
                        new EdgeList(aggregateSelection.ChainPartAggregateDefinition.Parameter.Edges), new LevelKey(aggregateSelection.ChainPartAggregateDefinition.Parameter.Edges, myDBContext.DBTypeManager),
                        aggregateSelection.ChainPartAggregateDefinition.Parameter, aggregateSelection);

                    validateResult = _SelectResultManager.AddAggregateElementToSelection(selection.Item2, aggregateSelection.ChainPartAggregateDefinition.Parameter.Reference.Item1, selPartAggr);
                    if (validateResult.Failed())
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

            if (myGroupBy.IsNotNullOrEmpty())
            {
                foreach (var group in myGroupBy)
                {
                    var validateResult = group.Validate(myDBContext, true);
                    if (validateResult.Failed())
                    {
                        return new Exceptional<SelectResultManager>(validateResult);
                    }
                    validateResult = _SelectResultManager.AddGroupElementToSelection(group.Reference.Item1, group);
                    if (validateResult.Failed())
                    {
                        return new Exceptional<SelectResultManager>(validateResult);
                    }
                }
            }

            #endregion

            #region Add having

            if (myHaving != null)
            {
                var validateResult = myHaving.Validate(myDBContext);
                if (validateResult.Failed())
                {
                    return new Exceptional<SelectResultManager>(validateResult);
                }

                _SelectResultManager.AddHavingToSelection(myHaving);
            }

            #endregion

            return new Exceptional<SelectResultManager>(_SelectResultManager);

        }

        #endregion

        #region IsInterestingWhereForReference

        /// <summary>
        /// Checks whether the <paramref name="myReference"/> is interesting for the <paramref name="myWhereExpressionDefinition"/>
        /// </summary>
        /// <param name="myReference">The reference defined in the FROM</param>
        /// <param name="myWhereExpressionDefinition"></param>
        /// <returns></returns>
        private Boolean IsInterestingWhereForReference(String myReference, BinaryExpressionDefinition myWhereExpressionDefinition)
        {

            #region check left

            var leftIDNode = myWhereExpressionDefinition.Left as IDChainDefinition;

            if (leftIDNode != null)
            {
                if (leftIDNode.Reference != null)
                {
                    if (leftIDNode.Reference.Item1.Equals(myReference))
                    {
                        return true;
                    }
                }
            }
            else if (myWhereExpressionDefinition.Left is BinaryExpressionDefinition)
            {
                return IsInterestingWhereForReference(myReference, (BinaryExpressionDefinition)myWhereExpressionDefinition.Left);
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
                    if (rightIDNode.Reference.Item1.Equals(myReference) || myWhereExpressionDefinition.Left is TupleDefinition)
                    {
                        return true;
                    }
                }
            }
            else if (myWhereExpressionDefinition.Right is BinaryExpressionDefinition)
            {
                return IsInterestingWhereForReference(myReference, (BinaryExpressionDefinition)myWhereExpressionDefinition.Right);
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
        
        #endregion
        
        #region GetExpressionGraphFromWhere

        /// <summary>
        /// Checks the where expression, optimize it and create an IExpressionGraph
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="myWhereExpressionDefinition"></param>
        /// <returns></returns>
        private Exceptional<IExpressionGraph> GetExpressionGraphFromWhere(DBContext myDBContext, BinaryExpressionDefinition myWhereExpressionDefinition)
        {

            #region interesting where

            #region calc the flow =)

            var optimizedExpr = OptimizeBinaryExpression(myWhereExpressionDefinition);

            #endregion

            #region exec expr

            var validateResult = optimizedExpr.Validate(myDBContext);
            if (validateResult.Failed())
            {
                return new Exceptional<IExpressionGraph>(validateResult);
            }
            var calculonResult = optimizedExpr.Calculon(myDBContext, new CommonUsageGraph(myDBContext), false);

            #endregion

            #region evaluate result

            if (calculonResult.Failed())
            {
                return new Exceptional<IExpressionGraph>(calculonResult);
            }

            #endregion

            #endregion

            return calculonResult;

        }

        #endregion

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
