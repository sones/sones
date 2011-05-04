using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Structure.Helper.Enums;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Helper.Enums;
using System.Diagnostics;
using sones.GraphQL.ErrorHandling;
using sones.Library.LanguageExtensions;

namespace sones.GraphQL.GQL.Manager.Select
{
    public sealed class SelectManager
    {
        #region data

        private readonly IGraphDB _graphdb;
        private readonly GQLPluginManager _pluginManager;

        #endregion

        #region constrcutor

        public SelectManager(IGraphDB myGraphDB, GQLPluginManager myPluginManager)
        {
            _graphdb = myGraphDB;
            _pluginManager = myPluginManager;
        }

        #endregion

        #region public methods

        public QueryResult ExecuteSelect(SecurityToken mySecurityToken, TransactionToken myTransactionToken, SelectDefinition selectDefinition, String myQuery)
        {
            #region Data

            List<String> alreadyUsedTypes = new List<string>();
            Stopwatch sw = Stopwatch.StartNew();

            #endregion

            #region Resolve types

            Dictionary<String, IVertexType> typeList = ResolveTypes(selectDefinition.TypeList, mySecurityToken, myTransactionToken);
            
            #endregion

            #region Create SelectResultManager which will do the lazy select

            var selectResultManager = CreateResultManager(mySecurityToken, myTransactionToken, selectDefinition, typeList);

            #endregion

            #region Get an expressiongraph if a whereExpression exists

            IExpressionGraph exprGraph;

            if (selectDefinition.WhereExpressionDefinition != null)
            {
                exprGraph = GetExpressionGraphFromWhere(selectDefinition.WhereExpressionDefinition, mySecurityToken, myTransactionToken);

                selectResultManager.ExpressionGraph = exprGraph;
            }

            #endregion

            List<IEnumerable<IVertexView>> _ListOfVertices1 = CreateVertices(mySecurityToken, myTransactionToken, typeList, selectResultManager, selectDefinition.WhereExpressionDefinition, selectDefinition.ResolutionDepth, selectDefinition.OrderByDefinition, selectDefinition.Limit, selectDefinition.Offset);

            #region TypeIndependendResults

            var _ListOfVertices2 = selectResultManager.GetTypeIndependendResult(mySecurityToken, myTransactionToken);
            if (_ListOfVertices2.CountIsGreater(0))
            {
                _ListOfVertices1.Add(_ListOfVertices2);
            }

            #endregion

            sw.Stop();

            return new QueryResult(myQuery, SonesGQLConstants.GQL, Convert.ToUInt64(sw.Elapsed.TotalMilliseconds), ResultType.Successful, AggregateListOfVertices(_ListOfVertices1));
        }

        #endregion

        #region private helper

        #region AggregateListOfVertices

        /// <summary>
        /// Aggregates different enumerations of vertices
        /// </summary>
        /// <param name="myListOfListOfVertices"></param>
        /// <returns></returns>
        private IEnumerable<IVertexView> AggregateListOfVertices(List<IEnumerable<IVertexView>> myListOfListOfVertices)
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

        #region ResolveTypes

        /// <summary>
        /// Resolves the TypeReferenceDefinition list to a dictionary of reference (coming from the FROM) and corresponding type
        /// </summary>
        private Dictionary<String, IVertexType> ResolveTypes(List<TypeReferenceDefinition> myTypeList, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {

            var typeList = new Dictionary<String, IVertexType>();

            foreach (var aType in myTypeList)
            {
                var t = _graphdb.GetVertexType(
                    mySecurityToken,
                    myTransactionToken,
                    new RequestGetVertexType(aType.TypeName),
                    (stats, type) => type);

                if (t == null)
                {
                    throw new VertexTypeDoesNotExistException(aType.TypeName, String.Empty);
                }

                typeList.Add(aType.Reference, t);

            }

            return typeList;

        }

        #endregion

        #region CreateResultManager

        /// <summary>
        /// Creates the result manager and adds all selected elements
        /// </summary>
        private SelectResultManager CreateResultManager(SecurityToken mySecurityToken, TransactionToken myTransactionToken, SelectDefinition selectDefinition, Dictionary<String, IVertexType> myTypeList)
        {

            var _SelectResultManager = new SelectResultManager(_graphdb, _pluginManager);

            foreach (var selection in selectDefinition.SelectedElements)
            {

                #region Add all selection elements to SelectResultManager

                if (selection.Item1 is IDChainDefinition)
                {

                    #region IDChainDefinition

                    var idChainSelection = (selection.Item1 as IDChainDefinition);

                    idChainSelection.Validate(_pluginManager, _graphdb, mySecurityToken, myTransactionToken, true);

                    if (idChainSelection.SelectType != TypesOfSelect.None)
                    {

                        #region Asterisk, Minus, Rhomb, Ad

                        //there's no limitation
                        foreach (var typeRef in myTypeList)
                        {
                            if (idChainSelection.TypeName != null)
                            {
                                var type = _graphdb.GetVertexType(
                                    mySecurityToken,
                                    myTransactionToken,
                                    new RequestGetVertexType(idChainSelection.TypeName),
                                    (stats, vType) => vType);

                                if (type == null)
                                {
                                    throw new VertexTypeDoesNotExistException(idChainSelection.TypeName, "");
                                }

                                Int64 typeID = type.ID;
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
                        _SelectResultManager.AddElementToSelection(selection.Item2, null, idChainSelection, false);
                    }
                    else //if (!(aColumnItemNode.ColumnSourceValue is AggregateNode))
                    {

                        IVertexType theType;
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
                            _SelectResultManager.AddSelectionType(reference, theType, idChainSelection.SelectType);
                        }
                        else
                        {
                            _SelectResultManager.AddElementToSelection(selection.Item2, reference, idChainSelection, false, selection.Item3);
                        }
                        //else
                        //{
                        //    return new Exceptional<SelectResultManager>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        //}
                    }
                    #endregion

                    #endregion
                }

                else if (selection.Item1 is AggregateDefinition)
                {

                    #region Aggregate

                    var aggregateSelection = selection.Item1 as AggregateDefinition;

                    aggregateSelection.ChainPartAggregateDefinition.Validate(_pluginManager, _graphdb, mySecurityToken, myTransactionToken);

                    var selPartAggr = new SelectionElementAggregate(aggregateSelection.ChainPartAggregateDefinition.Aggregate, selection.Item2,
                        new EdgeList(aggregateSelection.ChainPartAggregateDefinition.Parameter.Edges), new LevelKey(aggregateSelection.ChainPartAggregateDefinition.Parameter.Edges, _graphdb, mySecurityToken, myTransactionToken),
                        aggregateSelection.ChainPartAggregateDefinition.Parameter, aggregateSelection);

                    _SelectResultManager.AddAggregateElementToSelection(mySecurityToken, myTransactionToken, selection.Item2, aggregateSelection.ChainPartAggregateDefinition.Parameter.Reference.Item1, selPartAggr);
                    

                    #endregion

                }

                else
                {
                    throw new NotImplementedQLException("");
                }

                #endregion

            }

            #region Add groupings

            if (selectDefinition.GroupByIDs.IsNotNullOrEmpty())
            {
                foreach (var group in selectDefinition.GroupByIDs)
                {
                    group.Validate(_pluginManager, _graphdb, mySecurityToken, myTransactionToken, true);
                    
                    _SelectResultManager.AddGroupElementToSelection(group.Reference.Item1, group);
                }
            }

            #endregion

            #region Add having

            if (selectDefinition.Having != null)
            {
                selectDefinition.Having.Validate(_pluginManager, _graphdb, mySecurityToken, myTransactionToken);

                _SelectResultManager.AddHavingToSelection(selectDefinition.Having);
            }

            #endregion

            return _SelectResultManager;

        }

        #endregion

        #region GetExpressionGraphFromWhere

        /// <summary>
        /// Checks the where expression, optimize it and create an IExpressionGraph
        /// </summary>
        private IExpressionGraph GetExpressionGraphFromWhere(BinaryExpressionDefinition myWhereExpressionDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            #region interesting where

            #region exec expr

            myWhereExpressionDefinition.Validate(_pluginManager, _graphdb, mySecurityToken, myTransactionToken);
            
            var calculonResult = myWhereExpressionDefinition.Calculon(_pluginManager, _graphdb, mySecurityToken, myTransactionToken, new CommonUsageGraph(_graphdb, mySecurityToken, myTransactionToken), false);

            #endregion

            #endregion

            return calculonResult;

        }

        #endregion

        #region CreateVertices for all selected types

        /// <summary>
        /// Creates all readouts of all types in the <paramref name="myTypeList"/>
        /// </summary>
        private List<IEnumerable<IVertexView>> CreateVertices(        SecurityToken mySecurityToken,
                                                                      TransactionToken myTransactionToken,
                                                                      Dictionary<String, IVertexType> myTypeList,
                                                                      SelectResultManager mySelectResultManager,
                                                                      BinaryExpressionDefinition myWhereExpressionDefinition,
                                                                      Int64 myResolutionDepth,
                                                                      OrderByDefinition myOrderByDefinition,
                                                                      UInt64? myLimit,
                                                                      UInt64? myOffset)
        {

            var _ListOfListOfVertices = new List<IEnumerable<IVertexView>>();

            // Create vertices for type
            foreach (var typeRef in myTypeList)
            {

                var _Vertices = CreateVerticesForType(mySecurityToken,
                                                      myTransactionToken,
                                                      typeRef,
                                                      mySelectResultManager,
                                                      myWhereExpressionDefinition,
                                                      myResolutionDepth,
                                                      myOrderByDefinition,
                                                      myLimit,
                                                      myOffset);

                // If this type did not returned any result, we wont add it to the result
                if (_Vertices != null)
                {
                    _ListOfListOfVertices.Add(_Vertices);
                }

            }

            return _ListOfListOfVertices;

        }

        #endregion

        #region CreateVerticesForType - for a particular type

        /// <summary>
        /// Creates the lazy readouts for the given <paramref name="myTypeReference"/>
        /// </summary>
        private IEnumerable<IVertexView> CreateVerticesForType(        SecurityToken mySecurityToken,
                                                                       TransactionToken myTransactionToken,
                                                                       KeyValuePair<String, IVertexType> myTypeReference,
                                                                       SelectResultManager mySelectResultManager,
                                                                       BinaryExpressionDefinition myWhereExpressionDefinition,
                                                                       Int64 myResolutionDepth,
                                                                       OrderByDefinition myOrderByDefinition,
                                                                       UInt64? myLimit,
                                                                       UInt64? myOffset)
        {

            IEnumerable<IVertexView> _Vertices = new List<IVertexView>();

            #region Check groupings & aggregates

            mySelectResultManager.ValidateGroupingAndAggregate(myTypeReference.Key, myTypeReference.Value);

            #endregion

            #region Check, whether the type was affected by the where expressions. This will use either the Graph or the GUID index of the type

            Boolean isInterestingWhere = (myWhereExpressionDefinition != null && IsInterestingWhereForReference(myTypeReference.Key, myWhereExpressionDefinition));

            #endregion

            #region Create an IEnumerable of Readouts for this typeNode

            var result = mySelectResultManager.Examine(myResolutionDepth, myTypeReference.Key, myTypeReference.Value, isInterestingWhere, ref _Vertices, mySecurityToken, myTransactionToken);

            #endregion

            #region If this type did not returned any result, we wont add it to the result

            if (!result)
            {
                return new List<IVertexView>();
            }

            #endregion

            #region If there was a result for this typeNode we will add a new SelectionListElementResult

            _Vertices = mySelectResultManager.GetResult(_Vertices);

            #region OrderBy

            if (myOrderByDefinition != null)
            {
                OrderVertices(mySecurityToken, myTransactionToken , myOrderByDefinition, ref _Vertices);
            }

            #endregion

            #region Limit

            if (myLimit != null || myOffset != null)
            {
                ApplyLimitAndOffset(myLimit, myOffset, ref _Vertices);
            }

            #endregion


            #endregion

            return _Vertices;

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

        #region OrderVertices

        /// <summary>
        /// Orders the readouts
        /// </summary>
        private void OrderVertices(SecurityToken mySecurityToken, TransactionToken myTransactionToken, OrderByDefinition myOrderByDefinition, ref IEnumerable<IVertexView> myVertices)
        {

            #region Set the as alias for all not set ones to the attribute name

            foreach (var attrDef in myOrderByDefinition.OrderByAttributeList)
            {
                if (attrDef.IDChainDefinition != null)
                {
                    attrDef.IDChainDefinition.Validate(_pluginManager, _graphdb, mySecurityToken, myTransactionToken, true);

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
        }

        #endregion

        #region Vertices_OrderByComparer

        /// <summary>
        /// A comparer for a OrderByDefinition
        /// </summary>
        class Vertices_OrderByComparer : IComparer<IVertexView>
        {

            OrderByDefinition _OrderByDefinition;

            public Vertices_OrderByComparer(OrderByDefinition myOrderByDefinition)
            {
                _OrderByDefinition = myOrderByDefinition;
            }

            #region IComparer<Vertex> Members

            public int Compare(IVertexView myVertex1, IVertexView myVertex2)
            {

                Int32 retVal = 0;

                foreach (var attrDef in _OrderByDefinition.OrderByAttributeList)
                {

                    if (myVertex1.HasProperty(attrDef.AsOrderByString) && myVertex2.HasProperty(attrDef.AsOrderByString))
                    {
                        retVal = (myVertex1.GetProperty<IComparable>(attrDef.AsOrderByString)).CompareTo(myVertex2.GetProperty<IComparable>(attrDef.AsOrderByString));
                    }

                    else
                    {
                        if ((!myVertex1.HasProperty(attrDef.AsOrderByString)) && myVertex2.HasProperty(attrDef.AsOrderByString))
                            retVal = -1;
                        else if (myVertex1.HasProperty(attrDef.AsOrderByString) && (!(myVertex2.HasProperty(attrDef.AsOrderByString))))
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

        #region ApplyLimitAndOffset

        /// <summary>
        /// Applys a limit (if not null) and an offset (if not null) to the <paramref name="myVertices"/>
        /// </summary>
        private void ApplyLimitAndOffset(ulong? myLimit, ulong? myOffset, ref IEnumerable<IVertexView> myVertices)
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

        #endregion

    }
}
