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

/* <id name="sones GraphDB – ABinaryCompareOperator" />
 * <copyright file="ABinaryCompareOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>?</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Operator;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Lib.DataStructures.UUID;
using sones.Lib;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.Lib.Frameworks.Irony.Parsing;


#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    public abstract class ABinaryCompareOperator : ABinaryBaseOperator
    {

        /// <summary>
        /// This method joins two data tuples.
        /// </summary>
        /// <param name="leftData">Left data tuple.</param>
        /// <param name="rightData">Right data tuple.</param>
        /// <returns>A data tuple.</returns>
        private DataContainer JoinData(DataContainer leftData, DataContainer rightData)
        {
            return new DataContainer(new Tuple<IDNode, IDNode>(leftData.IdNodes.Item1, rightData.IdNodes.Item1), new Tuple<object, object>(leftData.Operands.Item1, rightData.Operands.Item1), new Tuple<object, object>(leftData.Extraordinaries.Item1, rightData.Extraordinaries.Item1));
        }

        /// <summary>
        /// Extracts data for a binary expression
        /// </summary>
        /// <param name="myComplexValue">The complex part of the binary expression.</param>
        /// <param name="mySimpleValue">The simple/atomic part of the expression.</param>
        /// <param name="errors">The list of errors.</param>
        /// <param name="typeOfBinExpr">The kind of the binary expression</param>
        /// <returns>A data tuple.</returns>
        private Exceptional<DataContainer> ExtractData(object myComplexValue, object mySimpleValue, ref TypesOfBinaryExpression typeOfBinExpr, DBObjectCache myDBObjectCache, SessionSettings mySessionToken, DBContext dbContext, Boolean aggregateAllowed)
        {
            #region data

            //the complex IDNode (sth. like U.Age or Count(U.Friends))
            IDNode complexIDNode = null;

            //the value that is on the opposite of the complex IDNode
            Object simpleValue = null;

            //a complex IDNode may result in a complexValue (i.e. Count(U.Friends) --> 3)
            Object complexValue = null;

            //reference to former myComplexValue
            Object extraordinaryValue = null;

            #endregion

            #region extraction

            if (myComplexValue is IDNode)
            {
                #region IDNode

                #region Data

                complexIDNode = (IDNode)myComplexValue;

                if (complexIDNode.IDNodeParts.Any(id => id is IDNodeFunc))
                {
                    if (complexIDNode.Edges.IsNullOrEmpty())
                    {
                        #region parameterless function

                        var fcn = (complexIDNode.IDNodeParts.First(id => id is IDNodeFunc) as IDNodeFunc).FuncCallNode;

                        // somes functions (aggregates) like SUM are not valid for where expressions, though they are not resolved
                        if (fcn.Function == null)
                            return new Exceptional<DataContainer>(new Error_FunctionDoesNotExist(fcn.FuncName));

                        var pResult = fcn.Function.ExecFunc(dbContext);
                        if (pResult.Failed)
                        {
                            return new Exceptional<DataContainer>(pResult);
                        }

                        //simpleValue = new AtomValue(fcn.Function.TypeOfResult, ((FuncParameter)pResult.Value).Value); //the new simple value extraced from the function
                        simpleValue = new AtomValue(((FuncParameter)pResult.Value).Value);
                        typeOfBinExpr = TypesOfBinaryExpression.Unknown; //we do not know if we are left or right associated
                        complexIDNode = null; //we resolved it... so it's null

                        #endregion
                    }
                    else
                    {
                        extraordinaryValue = (complexIDNode.IDNodeParts.First(id => id is IDNodeFunc) as IDNodeFunc).FuncCallNode;

                        if (mySimpleValue is AtomValue)
                        {
                            simpleValue = mySimpleValue;
                        }
                    }
                }
                else
                {
                    if(mySimpleValue is AtomValue)
                    {
                        try
                        {
                            simpleValue = GetCorrectAtomValue(complexIDNode.LastAttribute, complexIDNode.LastType, (AtomValue)mySimpleValue, dbContext, mySessionToken);
                        }
                        catch (FormatException)
                        {
                            return new Exceptional<DataContainer>(new Error_DataTypeDoesNotMatch(complexIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name, ((AtomValue)mySimpleValue).Value.ObjectName));
                        }
                    }
                    else
                    {
                        if (mySimpleValue is TupleValue)
                        {
                            ((TupleValue)mySimpleValue).ConvertToAttributeType(complexIDNode.LastAttribute, dbContext);

                            simpleValue = mySimpleValue;
                        }
                        else if (mySimpleValue is TupleNode)
                        {
                            var simpleValE = (mySimpleValue as TupleNode).GetAsTupleValue(dbContext, complexIDNode.LastAttribute);
                            if (!simpleValE.Success)
                            {
                                return new Exceptional<DataContainer>(simpleValE);
                            }
                            simpleValue = simpleValE.Value;
                        }
                        else
                        {
                            //return new Exceptional<DataContainer>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }
                    }
                }

                #endregion


                #endregion
            }
            else if (myComplexValue is TupleNode)
            {
                #region TupleSetNode

                complexValue = ((TupleNode)myComplexValue).Tuple;
                simpleValue = mySimpleValue;
                typeOfBinExpr = TypesOfBinaryExpression.Atom;

                #endregion
            }
            else if (myComplexValue is AggregateNode)
            {
                #region AggregateNode

                if (aggregateAllowed)
                {
                    if (((AggregateNode)myComplexValue).Expressions.Count != 1)
                    {
                        return new Exceptional<DataContainer>(new Error_ArgumentException("An aggregate must have exactly one expression."));
                    }

                    if (!(((AggregateNode)myComplexValue).Expressions[0] is IDNode))
                    {
                       return new Exceptional<DataContainer>(new Error_ArgumentException("An aggregate must have exactly one IDNode."));
                    }

                    #region Data

                    complexIDNode = (((AggregateNode)myComplexValue).Expressions[0] as IDNode);

                    if (complexIDNode == null)
                    {
                        return new Exceptional<DataContainer>(new Error_InvalidIDNode("Only single IDNodes are currently allowed in aggregates!"));
                    }

                    #endregion

                    #region values

                    simpleValue = mySimpleValue;
                    extraordinaryValue = myComplexValue;

                    #endregion
                }
                else
                {
                    return new Exceptional<DataContainer>(new Error_AggregateNotAllowed((AggregateNode)myComplexValue));
                }
                #endregion
            }
            else if (myComplexValue is FuncCallNode)
            {
                #region FuncCallNode

                #region Data

                if (((FuncCallNode)myComplexValue).Expressions.Count > 1)
                {
                    return new Exceptional<DataContainer>(new Error_InvalidIDNode("Only single IDNodes are currently allowed in functions!"));
                }

                #endregion

                #region parameterless function

                if (((FuncCallNode)myComplexValue).Expressions.Count == 0)
                {
                    var pResult = ((FuncCallNode)myComplexValue).Function.ExecFunc(dbContext);
                    if (pResult.Failed)
                    {
                        return new Exceptional<DataContainer>(pResult);
                    }

                    complexValue = new AtomValue(((FuncParameter)pResult.Value).Value);
                    typeOfBinExpr = TypesOfBinaryExpression.Atom;
                }
                else
                {
                    complexIDNode = ((IDNode)((FuncCallNode)myComplexValue).Expressions[0]);
                }

                #endregion

                #region values

                simpleValue = mySimpleValue;
                extraordinaryValue = myComplexValue;

                #endregion

                #endregion
            }
            else
            {
                return new Exceptional<DataContainer>(new Error_NotImplementedExpressionNode(myComplexValue.GetType())); 
            }

            #endregion

            return new Exceptional<DataContainer>(new DataContainer(new Tuple<IDNode, IDNode>(complexIDNode, null), new Tuple<Object, Object>(simpleValue, complexValue), new Tuple<Object, Object>(extraordinaryValue, null)));
        }

        private AtomValue GetCorrectAtomValue(TypeAttribute typeAttribute, GraphDBType graphDBType, AtomValue atomValue, DBContext dbContext, SessionSettings mySessionsInfos)
        {

            if (typeAttribute.IsBackwardEdge)
            {
                return GetCorrectAtomValue(dbContext.DBTypeManager.GetTypeByUUID(typeAttribute.BackwardEdgeDefinition.TypeUUID).GetTypeAttributeByUUID(typeAttribute.BackwardEdgeDefinition.AttrUUID), graphDBType, atomValue, dbContext, mySessionsInfos);
            }
            else
            {
                if (typeAttribute is ASpecialTypeAttribute && typeAttribute.UUID == SpecialTypeAttribute_UUID.AttributeUUID)
                {
                    //var uuid = SpecialTypeAttribute_UUID.ConvertToUUID(atomValue.Value.Value.ToString(), graphDBType, mySessionsInfos, dbContext.DBTypeManager);


                    //if (uuid.Failed)
                    //{
                    //    throw new GraphDBException(uuid.Errors);
                    //}
                    //return new AtomValue(new DBReference(uuid.Value));
                    return new AtomValue(new DBReference(ObjectUUID.FromString(atomValue.Value.Value.ToString())));
                }
                else if (!typeAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    return new AtomValue(GraphDBTypeMapper.GetADBBaseObjectFromUUID(typeAttribute.DBTypeUUID, atomValue.Value));
                }
                else
                {
                    throw new GraphDBException(new Error_InvalidAttributeValue(typeAttribute.Name, atomValue.Value));
                }
            }

        }

        /// <summary>
        /// Finds matching result corresponding to a binary expression.
        /// </summary>
        /// <param name="myLeftValueObject">The left value of a binary expression.</param>
        /// <param name="myRightValueObject">The right value of a binary expression.</param>
        /// <param name="currentTypeDefinitione"></param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        /// <param name="typeOfBinExpr">The type of the binary expression.</param>
        /// <param name="associativity">The associativity of the binary expression.</param>
        /// <param name="referenceList"></param>
        /// <param name="queryCache">The per query DBObject/BackwardEdge cache.</param>
        /// <returns></returns>
        public override Exceptional<IExpressionGraph> TypeOperation(object myLeftValueObject, object myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph resultGr, Boolean aggregateAllowed = true)        
        {
            #region Data
            
            //list of errors
            List<GraphDBError> errors = new List<GraphDBError>();
            
            //DataContainer for all data that is used by a binary expression/comparer
            Exceptional<DataContainer> data;

            //the index of the left attribute
            IEnumerable<Tuple<GraphDBType, AttributeIndex>> leftIndex = null;

            //the index of the right attribute
            IEnumerable<Tuple<GraphDBType, AttributeIndex>> rightIndex = null;

            #endregion

            #region extract data

            //data extraction with an eye on the type of the binary expression

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.Atom:

                    //sth like 3 = 4
                    #region Get Atom data

                    //no further data has to be generated

                    //data = new DataContainer(null, new Tuple<Object, Object>(myLeftValueObject, myRightValueObject), null);
                    data = new Exceptional<DataContainer>();

                    #endregion

                    break;
                case TypesOfBinaryExpression.LeftComplex:

                    //sth like U.Age = 21
                    #region Get LeftComplex data

                    data = ExtractData(myLeftValueObject, myRightValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!data.Success)
                    {
                        return new Exceptional<IExpressionGraph>(data);
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    //sth like 21 = U.Age
                    #region Get RightComplex data

                    data = ExtractData(myRightValueObject, myLeftValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!data.Success)
                    {
                        return new Exceptional<IExpressionGraph>(data);
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.Complex:

                    //sth like U.Age = F.Alter
                    #region Get Complex data

                    var leftData = ExtractData(myLeftValueObject, myRightValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!leftData.Success)
                    {
                        return new Exceptional<IExpressionGraph>(leftData);
                    }

                    var rightData = ExtractData(myRightValueObject, myLeftValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!rightData.Success)
                    {
                        return new Exceptional<IExpressionGraph>(rightData);
                    }
                    
                    if (typeOfBinExpr == TypesOfBinaryExpression.Unknown)
                    {
                        typeOfBinExpr = SetTypeOfBinaryExpression(leftData, rightData);

                        switch (typeOfBinExpr)
                        {
                            case TypesOfBinaryExpression.Atom:

                                data = new Exceptional<DataContainer>(new DataContainer(new Tuple<IDNode, IDNode>(null, null), new Tuple<object, object>(leftData.Value.Operands.Item1, leftData.Value.Operands.Item1), new Tuple<object, object>(null, null)));

                                break;
                            case TypesOfBinaryExpression.LeftComplex:

                                data = new Exceptional<DataContainer>(new DataContainer(new Tuple<IDNode, IDNode>(leftData.Value.IdNodes.Item1, null), new Tuple<object, object>(rightData.Value.Operands.Item1, null), new Tuple<object, object>(null, null)));

                                break;
                            case TypesOfBinaryExpression.RightComplex:

                                data = new Exceptional<DataContainer>(new DataContainer(new Tuple<IDNode, IDNode>(rightData.Value.IdNodes.Item1, null), new Tuple<object, object>(leftData.Value.Operands.Item1, null), new Tuple<object, object>(null, null)));

                                break;
                            case TypesOfBinaryExpression.Complex:
                            case TypesOfBinaryExpression.Unknown:
                            default:

                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }

                    }
                    else
                    {
                        data = new Exceptional<DataContainer>(JoinData(leftData.Value, rightData.Value));
                    }

                    #endregion

                    break;
                default:

                    throw new ArgumentException();
            }

            #region handle errors

            if (data.Failed)
            {
                return new Exceptional<IExpressionGraph>(data);
            }

            #endregion

            #endregion

            #region get indexes

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    leftIndex = GetIndex(data.Value.IdNodes.Item1.LastAttribute, dbContext, data.Value.IdNodes.Item1.LastType, data.Value.Extraordinaries.Item1);

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    //data.IdNodes.TupelElement1 is correct, because of correct handling in extract data (data.IdNodes.TupelElement2 should be null here)
                    rightIndex = GetIndex(data.Value.IdNodes.Item1.LastAttribute, dbContext, data.Value.IdNodes.Item1.LastType, data.Value.Extraordinaries.Item1);

                    break;

                case TypesOfBinaryExpression.Complex:

                    //both indexe have to be catched

                    leftIndex = GetIndex(data.Value.IdNodes.Item1.LastAttribute, dbContext, data.Value.IdNodes.Item1.LastType, data.Value.Extraordinaries.Item1);
                    rightIndex = GetIndex(data.Value.IdNodes.Item2.LastAttribute, dbContext, data.Value.IdNodes.Item2.LastType, data.Value.Extraordinaries.Item1);

                    break;
            }

            #endregion

            //time to compare some things
            Exceptional<Boolean> matchDataResult = null;

            if (IsValidIndexOperation(data.Value, dbContext, typeOfBinExpr))
            {
                #region match data

                switch (typeOfBinExpr)
                {
                    case TypesOfBinaryExpression.Atom:

                        #region Atom

                        //do nothing 3 = 3 (or 2 != 3) doesnt bother U

                        #endregion

                        break;

                    case TypesOfBinaryExpression.LeftComplex:

                        #region LeftComplex

                        matchDataResult = MatchData(data.Value, dbContext, dbContext.DBObjectCache, typeOfBinExpr, leftIndex, resultGr, dbContext.SessionSettings);

                        #endregion

                        break;

                    case TypesOfBinaryExpression.RightComplex:

                        #region RightComplex

                        matchDataResult = MatchData(data.Value, dbContext, dbContext.DBObjectCache, typeOfBinExpr, rightIndex, resultGr, dbContext.SessionSettings);

                        #endregion

                        break;

                    case TypesOfBinaryExpression.Complex:

                        #region Complex

                        matchDataResult = MatchComplexData(associativity, data.Value, dbContext, dbContext.DBObjectCache, typeOfBinExpr, leftIndex, rightIndex, ref errors, ref resultGr, dbContext.SessionSettings);

                        #endregion

                        break;
                }

                #endregion
            }
            else
            {
                return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this, data.Value.IdNodes, data.Value.Operands, typeOfBinExpr));
            }

            if (matchDataResult != null && matchDataResult.Failed)
                return new Exceptional<IExpressionGraph>(matchDataResult);
            else
                return new Exceptional<IExpressionGraph>(resultGr);
        }

        private TypesOfBinaryExpression SetTypeOfBinaryExpression(Exceptional<DataContainer> leftData, Exceptional<DataContainer> rightData)
        {
            TypesOfBinaryExpression result;

            if (leftData.Value.IdNodes.Item1 != null && rightData.Value.IdNodes.Item1 != null)
            {
                result = TypesOfBinaryExpression.Complex;
            }
            else
            {
                if (leftData.Value.IdNodes.Item1 == null && rightData.Value.IdNodes.Item1 == null)
                {
                    result = TypesOfBinaryExpression.Atom;
                }
                else
                {
                    if (leftData.Value.IdNodes.Item1 != null)
                    {
                        result = TypesOfBinaryExpression.LeftComplex;
                    }
                    else
                    {
                        result = TypesOfBinaryExpression.RightComplex;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method gets result of a simple atomic operation.
        /// </summary>
        /// <param name="myDBType">The DBTypeStream.</param>
        /// <param name="myTypeManager">The TypeManager of the database.</param>
        /// <param name="queryCache">The current query cache.</param>
        /// <param name="data">The DataContainer.</param>
        private void GetAtomResult(GraphDBType myDBType, DBContext myTypeManager, DBObjectCache dbObjectCache, DataContainer data, ref IExpressionGraph result)
        {
            //do nothing here
        }

        private Exceptional<Boolean> MatchComplexData(TypesOfAssociativity associativity, DataContainer data, DBContext dbContext, DBObjectCache dbObjectCache, TypesOfBinaryExpression typeOfBinExpr, IEnumerable<Tuple<GraphDBType, AttributeIndex>> leftIndices, IEnumerable<Tuple<GraphDBType, AttributeIndex>> rightIndices, ref List<GraphDBError> errors, ref IExpressionGraph result, SessionSettings mySessionToken)
        {
            #region data

            Dictionary<DBObjectStream, IOperationValue> operandsLeft = null;
            Dictionary<DBObjectStream, IOperationValue> operandsRight = null;

            #endregion

            #region handle extraordinaries

            if (data.Extraordinaries.Item1 != null)
            {
                #region left extraordinary
                //there is something like a function or so

                operandsLeft = new Dictionary<DBObjectStream, IOperationValue>();

                //we have to calculate the real operand.
                //TODO: try to use attribute idx instead of uuid idx

                foreach (var aLeftIDX in leftIndices)
                {
                    AttributeIndex currentLeftIdx;

                    #region get UUID idx
                    if (!aLeftIDX.Item2.IsUuidIndex)
                    {
                        currentLeftIdx = aLeftIDX.Item1.GetUUIDIndex(dbContext.DBTypeManager);
                    }
                    else
                    {
                        currentLeftIdx = aLeftIDX.Item2;
                    }

                    #endregion

                    var idxRef = currentLeftIdx.GetIndexReference(dbContext.DBIndexManager);
                    if (!idxRef.Success)
                    {
                        return new Exceptional<bool>(idxRef);
                    }

                    foreach (var ObjectUUIDs_left in idxRef.Value.Values())
                    {
                        var leftDBObject = dbObjectCache.LoadDBObjectStream(aLeftIDX.Item1, ObjectUUIDs_left.FirstOrDefault());
                        if (leftDBObject.Failed)
                        {
                            throw new NotImplementedException();
                        }

                        if (IsValidDBObjectStreamForBinExpr(leftDBObject.Value, data.IdNodes.Item1.LastAttribute, dbContext.DBTypeManager))
                        {
                            var oper = GetOperand(data.IdNodes.Item1, data.Extraordinaries.Item1, dbContext, leftDBObject.Value, dbObjectCache, mySessionToken);
                            if (oper.Failed)
                                return new Exceptional<bool>(oper);

                            if (oper != null)
                            {
                                operandsLeft.Add(leftDBObject.Value, oper.Value);
                            }
                        }
                    }
                }

                //from now on the binary expression is right complex, because there are atom values on the left
                typeOfBinExpr = TypesOfBinaryExpression.RightComplex;

                #endregion
            }

            if (data.Extraordinaries.Item2 != null)
            {
                #region right extraordinary
                //there is something like a function or so
                operandsRight = new Dictionary<DBObjectStream, IOperationValue>();

                foreach (var aRightIDX in rightIndices)
                {
                    AttributeIndex currentRightIdx;

                    #region get UUID idx
                    //we have to calculate the real operand.
                    //TODO: try to use attribute idx instead of uuid idx

                    if (!aRightIDX.Item2.IsUuidIndex)
                    {
                        currentRightIdx = aRightIDX.Item1.GetUUIDIndex(dbContext.DBTypeManager);
                    }
                    else
                    {
                        currentRightIdx = aRightIDX.Item2;
                    }

                    #endregion

                    var idxRef = currentRightIdx.GetIndexReference(dbContext.DBIndexManager);
                    if (!idxRef.Success)
                    {
                        return new Exceptional<bool>(idxRef);
                    }

                    foreach (var ObjectUUIDs_right in idxRef.Value.Values())
                    {
                        var rightDBObject = dbObjectCache.LoadDBObjectStream(aRightIDX.Item1, ObjectUUIDs_right.FirstOrDefault());
                        if (rightDBObject.Failed)
                        {
                            throw new NotImplementedException();
                        }

                        if (IsValidDBObjectStreamForBinExpr(rightDBObject.Value, data.IdNodes.Item2.LastAttribute, dbContext.DBTypeManager))
                        {
                            var oper = GetOperand(data.IdNodes.Item2, data.Extraordinaries.Item2, dbContext, rightDBObject.Value, dbObjectCache, mySessionToken);
                            if (oper.Failed)
                                return new Exceptional<bool>(oper);

                            if (oper != null)
                            {
                                operandsRight.Add(rightDBObject.Value, oper.Value);
                            }
                        }
                    }
                }

                if (typeOfBinExpr == TypesOfBinaryExpression.RightComplex)
                {
                    typeOfBinExpr = TypesOfBinaryExpression.Atom;
                }

                #endregion
            }

            #endregion

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.Atom:

                    #region atom

                    switch (associativity)
                    {
                        case TypesOfAssociativity.Unknown:
                        case TypesOfAssociativity.Neutral:
                            GetComplexAtom(dbContext, operandsLeft, operandsRight, data.IdNodes.Item1, dbObjectCache, ref result);
                            GetComplexAtom(dbContext, operandsRight, operandsLeft, data.IdNodes.Item2, dbObjectCache, ref result);
                            break;

                        case TypesOfAssociativity.Left:

                            GetComplexAtom(dbContext, operandsLeft, operandsRight, data.IdNodes.Item1,dbObjectCache, ref result);

                            break;
                        case TypesOfAssociativity.Right:

                            GetComplexAtom(dbContext, operandsRight, operandsLeft, data.IdNodes.Item2,dbObjectCache, ref result);

                            break;

                        default:
                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.LeftComplex:

                    #region Left complex

                    GetComplexMatch(leftIndices, operandsRight, dbObjectCache, data.IdNodes.Item1, data.IdNodes.Item2, dbContext, associativity, ref result, mySessionToken);

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    #region Right complex

                    GetComplexMatch(rightIndices, operandsLeft, dbObjectCache, data.IdNodes.Item2, data.IdNodes.Item1, dbContext, associativity, ref result, mySessionToken);

                    #endregion

                    break;
                case TypesOfBinaryExpression.Complex:

                    #region Complex

                    LevelKey leftLevelKey = CreateLevelKey(data.IdNodes.Item1);
                    LevelKey rightLevelKey = CreateLevelKey(data.IdNodes.Item2);
                    GraphDBType leftGraphDBType = data.IdNodes.Item1.LastType;
                    GraphDBType rightGraphDBType = data.IdNodes.Item2.LastType;

                    #region exception

                    if (leftIndices.CountIsGreater(1) || rightIndices.CountIsGreater(1))
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    var idxRefLeft = leftIndices.First().Item2.GetIndexReference(dbContext.DBIndexManager);
                    if (!idxRefLeft.Success)
                    {
                        return new Exceptional<bool>(idxRefLeft);
                    }

                    var idxRefRight = rightIndices.First().Item2.GetIndexReference(dbContext.DBIndexManager);
                    if (!idxRefRight.Success)
                    {
                        return new Exceptional<bool>(idxRefRight);
                    }

                    foreach (var ObjectUUIDs_left in idxRefLeft.Value.Values())
                    {
                        foreach (var aLeftUUID in ObjectUUIDs_left)
                        {
                            var leftDBObject = dbObjectCache.LoadDBObjectStream(leftIndices.First().Item1, aLeftUUID);
                            if (leftDBObject.Failed)
                            {
                                return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                            if (IsValidDBObjectStreamForBinExpr(leftDBObject.Value, data.IdNodes.Item1.LastAttribute, dbContext.DBTypeManager))
                            {

                                foreach (var ObjectUUIDs_right in idxRefRight.Value.Values())
                                {
                                    foreach (var aRightUUID in ObjectUUIDs_right)
                                    {

                                        var rightDBObject = dbObjectCache.LoadDBObjectStream(rightIndices.First().Item1, aRightUUID);
                                        if (rightDBObject.Failed)
                                        {
                                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                        }

                                        if (IsValidDBObjectStreamForBinExpr(rightDBObject.Value, data.IdNodes.Item2.LastAttribute, dbContext.DBTypeManager))
                                        {
                                            //everything is valid

                                            var leftType = GraphDBTypeMapper.ConvertPandora2CSharp(data.IdNodes.Item1.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);
                                            var rightType = GraphDBTypeMapper.ConvertPandora2CSharp(data.IdNodes.Item2.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);

                                            IOperationValue leftValue;
                                            IOperationValue rightValue;

                                            if (data.IdNodes.Item1.LastAttribute.KindOfType == KindsOfType.SetOfReferences
                                                || data.IdNodes.Item1.LastAttribute.KindOfType == KindsOfType.ListOfNoneReferences || data.IdNodes.Item1.LastAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                                                leftValue = new TupleValue(leftType, leftDBObject.Value.GetAttribute(data.IdNodes.Item1.LastAttribute.UUID), data.IdNodes.Item1.LastAttribute.GetDBType(dbContext.DBTypeManager));
                                            else
                                                leftValue = new AtomValue(leftType, leftDBObject.Value.GetAttribute(data.IdNodes.Item1.LastAttribute.UUID, data.IdNodes.Item1.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbContext));

                                            if (data.IdNodes.Item2.LastAttribute.KindOfType == KindsOfType.SetOfReferences
                                                || data.IdNodes.Item2.LastAttribute.KindOfType == KindsOfType.ListOfNoneReferences || data.IdNodes.Item2.LastAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                                                rightValue = new TupleValue(rightType, rightDBObject.Value.GetAttribute(data.IdNodes.Item2.LastAttribute.UUID), data.IdNodes.Item2.LastAttribute.GetDBType(dbContext.DBTypeManager));
                                            else
                                                rightValue = new AtomValue(rightType, rightDBObject.Value.GetAttribute(data.IdNodes.Item2.LastAttribute.UUID, data.IdNodes.Item2.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbContext));

                                            var tempSimpleOperationResult = this.SimpleOperation(leftValue, rightValue, typeOfBinExpr);
                                            if (tempSimpleOperationResult.Failed)
                                                return new Exceptional<bool>(tempSimpleOperationResult);

                                            var tempOperatorResult = tempSimpleOperationResult.Value;

                                            if ((Boolean)(tempOperatorResult as AtomValue).Value.Value)
                                            {
                                                //found sth that is really true

                                                #region insert into graph

                                                switch (associativity)
                                                {
                                                    case TypesOfAssociativity.Neutral:
                                                    case TypesOfAssociativity.Left:

                                                        IntegrateInGraph(leftDBObject.Value, result, leftLevelKey, dbContext, dbObjectCache);

                                                        break;
                                                    case TypesOfAssociativity.Right:

                                                        IntegrateInGraph(rightDBObject.Value, result, rightLevelKey, dbContext, dbObjectCache);

                                                        break;
                                                    case TypesOfAssociativity.Unknown:

                                                        if (Type == TypesOfOperators.AffectsLowerLevels)
                                                        {
                                                            result.AddNodesWithComplexRelation(leftDBObject, leftLevelKey, rightDBObject, rightLevelKey, dbObjectCache, 1);
                                                        }
                                                        else
                                                        {
                                                            result.AddNodesWithComplexRelation(leftDBObject, leftLevelKey, rightDBObject, rightLevelKey, dbObjectCache, 0);
                                                        }

                                                        break;
                                                    default:

                                                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                                }

                                                #endregion
                                            }
                                        }
                                    }
                                }

                                #region clean lower levels

                                if (Type == TypesOfOperators.AffectsLowerLevels)
                                {
                                    switch (associativity)
                                    {
                                        case TypesOfAssociativity.Neutral:
                                        case TypesOfAssociativity.Left:

                                            CleanLowerLevel(leftLevelKey, dbContext, dbObjectCache, result);

                                            break;
                                        case TypesOfAssociativity.Right:

                                            CleanLowerLevel(rightLevelKey, dbContext, dbObjectCache, result);

                                            break;
                                        case TypesOfAssociativity.Unknown:

                                            CleanLowerLevel(leftLevelKey, dbContext, dbObjectCache, result);
                                            CleanLowerLevel(rightLevelKey, dbContext, dbObjectCache, result);

                                            break;
                                        default:

                                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                    }
                                }

                                #endregion
                            }
                        }
                    }

                    #endregion

                    break;
                default:

                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            return new Exceptional<bool>(true);
        }

        private Exceptional<Boolean> GetComplexMatch(IEnumerable<Tuple<GraphDBType, AttributeIndex>> myIDX, Dictionary<DBObjectStream, IOperationValue> operands, DBObjectCache dbObjectCache, IDNode primIDNode, IDNode operandIDNode, DBContext dbContext, TypesOfAssociativity associativity, ref IExpressionGraph resultGraph, SessionSettings mySessionToken)
        {
            LevelKey primLevelKey = CreateLevelKey(primIDNode);
            LevelKey operandLevelKey = CreateLevelKey(operandIDNode);

            foreach (var aIDX in myIDX)
            {
                if (aIDX.Item2.IsUuidIndex)
                {
                    #region Guid idx

                    foreach (var aOperand in operands)
                    {

                        var idxRef = aIDX.Item2.GetIndexReference(dbContext.DBIndexManager);
                        if (!idxRef.Success)
                        {
                            return new Exceptional<bool>(idxRef);
                        }

                        foreach (var _ObjectUUIDs in idxRef.Value.Values())
                        {
                            var DBObjectStream = dbObjectCache.LoadDBObjectStream(aIDX.Item1, _ObjectUUIDs.FirstOrDefault());
                            if (DBObjectStream.Failed)
                            {
                                return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                            if (IsValidDBObjectStreamForBinExpr(DBObjectStream.Value, primIDNode.LastAttribute, dbContext.DBTypeManager))
                            {
                                var aCtype = GraphDBTypeMapper.ConvertPandora2CSharp(primIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);
                                Object dbos = GetDbos(primIDNode, DBObjectStream.Value, dbContext, mySessionToken, dbObjectCache);

                                Exceptional<IOperationValue> tempResult;
                                if (aCtype == TypesOfOperatorResult.SetOfDBObjects)
                                {
                                    tempResult = this.SimpleOperation(new TupleValue(aCtype, dbos, primIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager)), aOperand.Value, TypesOfBinaryExpression.Complex);
                                }
                                else
                                {
                                    tempResult = this.SimpleOperation(new AtomValue(aCtype, dbos), aOperand.Value, TypesOfBinaryExpression.Complex);

                                }

                                if (tempResult.Failed)
                                    return new Exceptional<bool>(tempResult);

                                var tempOperatorResult = (AtomValue)tempResult.Value;

                                if ((Boolean)tempOperatorResult.Value.Value)
                                {
                                    switch (associativity)
                                    {
                                        case TypesOfAssociativity.Neutral:
                                        case TypesOfAssociativity.Left:

                                            IntegrateInGraph(aOperand.Key, resultGraph, operandLevelKey, dbContext, dbObjectCache);

                                            break;
                                        case TypesOfAssociativity.Right:

                                            IntegrateInGraph(DBObjectStream.Value, resultGraph, primLevelKey, dbContext, dbObjectCache);

                                            break;
                                        default:

                                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    //TODO:
                    //#region attribute idx

                    //foreach (var aOperand in operands)
                    //{
                    //    var idxOper = this.IndexSingleOperation(idx, aOperand.Item2, TypesOfBinaryExpression.Complex);

                    //    foreach (var _ObjectUUID in idxOper)
                    //    {
                    //        switch (associativity)
                    //        {
                    //            case TypesOfAssociativity.Neutral:
                    //            case TypesOfAssociativity.Left:

                    //                IntegrateInGraph(aOperand.Item1, resultGraph, operandLevelKey, myTypeManager,dbObjectCache);

                    //                break;
                    //            case TypesOfAssociativity.Right:

                    //                var tempDBO = dbObjectCache.LoadDBObjectStream(primIDNode.LastAttribute.RelatedPandoraType, _ObjectUUID);
                    //                if(tempDBO.Failed)
                    //                {
                    //                    throw new NotImplementedException();
                    //                }

                    //                IntegrateInGraph(tempDBO.Value, resultGraph, primLevelKey, myTypeManager, dbObjectCache);

                    //                break;
                    //            default:

                    //                return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    //        }
                    //    }
                    //}

                    //#endregion
                }
            }

            return new Exceptional<bool>(true);

        }

        private Exceptional<IExpressionGraph> GetComplexAtom(DBContext myTypeManager, Dictionary<DBObjectStream, IOperationValue> operandsPrim, Dictionary<DBObjectStream, IOperationValue> operandsComparism, IDNode iDNode, DBObjectCache dbObjectCache, ref IExpressionGraph result)
        {
            #region data

            LevelKey myLevelKey = CreateLevelKey(iDNode);

            #endregion

            foreach (var left in operandsPrim)
            {
                foreach (var right in operandsComparism)
                {
                    var tempResult = this.SimpleOperation(left.Value, right.Value, TypesOfBinaryExpression.Atom);
                    if (tempResult.Failed)
                        return new Exceptional<IExpressionGraph>(tempResult);

                    if ((Boolean)((AtomValue)tempResult.Value).Value.Value)
                    {
                        IntegrateInGraph(left.Key, result, myLevelKey, myTypeManager,dbObjectCache);
                        break;
                    }
                }
            }

            return new Exceptional<IExpressionGraph>(result);

        }

        /// <summary>
        /// This method returns an IEnumerable of IExpressionNodes for all ObjectUUIDs of a given index.
        /// </summary>
        /// <param name="iIndexObject">A IIndexObject.</param>
        /// <returns>An IEnumerable of IExpressionNodes.</returns>
        private IEnumerable<IExpressionNode> GetAllNodesForIndex(IIndexObject<string, ObjectUUID> iIndexObject)
        {
            foreach (var _ObjectUUIDs in iIndexObject.Values())
            {
                foreach (var aObjectUUID in _ObjectUUIDs)
                {
                    yield return new sones.GraphDB.QueryLanguage.ExpressionGraph.ExpressionNode(aObjectUUID, null);
                }
            }
        }

        /// <param name="data">The DataContainer.</param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        /// <param name="queryCache">The current query cache.</param>
        /// <param name="typeOfBinExpr">The type of the binary expression.</param>
        /// <param name="idx">The index for the complex part.</param>
        /// <param name="errors">A list of errors.</param>
        /// <param name="resultGraph">The IExpressionGraph that serves as result.</param>
        /// <returns>True if the method succeeded</returns>
        private Exceptional<Boolean> MatchData(DataContainer data, DBContext dbContext, DBObjectCache dbObjectCache, TypesOfBinaryExpression typeOfBinExpr, IEnumerable<Tuple<GraphDBType, AttributeIndex>> idx, IExpressionGraph resultGraph, SessionSettings mySessionToken)
        {
            #region data

            LevelKey myLevelKey = CreateLevelKey(data.IdNodes.Item1);

            #endregion

            foreach (var aIDX in idx)
            {
                #region Execution

                if (aIDX.Item2.IsUuidIndex && (data.IdNodes.Item1.LastAttribute != dbContext.DBTypeManager.GetGUIDTypeAttribute()))
                {
                    #region Guid idx - check ALL DBOs

                    var idxRef = aIDX.Item2.GetIndexReference(dbContext.DBIndexManager);
                    if (!idxRef.Success)
                    {
                        return new Exceptional<bool>(idxRef);
                    }

                    foreach (var _ObjectUUIDs in idxRef.Value.Values())
                    {
                        var result = IntegrateUUID(data, dbContext, dbObjectCache, typeOfBinExpr, resultGraph, mySessionToken, myLevelKey, dbObjectCache.LoadListOfDBObjectStreams(aIDX.Item1, _ObjectUUIDs));
                        if (result.Failed)
                        {
                            return new Exceptional<bool>(result);
                        }
                    }

                    if (resultGraph.ContainsLevelKey(myLevelKey))
                    {
                        #region clean lower levels

                        if (Type == TypesOfOperators.AffectsLowerLevels)
                        {
                            CleanLowerLevel(myLevelKey, dbContext, dbObjectCache, resultGraph);
                        }

                        #endregion

                    }
                    else
                    {
                        resultGraph.AddEmptyLevel(myLevelKey);
                    }

                    #endregion
                }
                else
                {

                    #region Get operationValue and type

                    var operationValue = (IOperationValue)data.Operands.Item1;

                    #endregion

                    #region Attribute index

                    if (aIDX.Item2.IndexKeyDefinition.IndexKeyAttributeUUIDs.Count > 1)
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently it is not implemented to use compound indices."));
                    }

                    var interestingUUIDsByIdx = IndexSingleOperation(aIDX.Item2, operationValue, data.IdNodes.Item1.LastAttribute.UUID, typeOfBinExpr, dbContext.DBIndexManager);


                    #region integrate in graph

                    var runMT = DBConstants.RunMT;

                    if (runMT)
                    {
                        Parallel.ForEach(dbObjectCache.LoadListOfDBObjectStreams(data.IdNodes.Item1.LastType, interestingUUIDsByIdx), aDBO =>
                        {
                            IntegrateInGraph(aDBO.Value, resultGraph, myLevelKey, dbContext, dbObjectCache);
                        }
                        );
                    }
                    else
                    {
                        foreach (var aDBO in dbObjectCache.LoadListOfDBObjectStreams(data.IdNodes.Item1.LastType, interestingUUIDsByIdx))
                        {
                            IntegrateInGraph(aDBO.Value, resultGraph, myLevelKey, dbContext, dbObjectCache);
                        }
                    }

                    #endregion

                    if (resultGraph.ContainsLevelKey(myLevelKey))
                    {
                        #region clean lower levels

                        if (Type == TypesOfOperators.AffectsLowerLevels)
                        {
                            CleanLowerLevel(myLevelKey, dbContext, dbObjectCache, resultGraph);
                        }

                        #endregion

                    }
                    else
                    {
                        resultGraph.AddEmptyLevel(myLevelKey);
                    }

                    #endregion
                }

                #endregion
            }

            return new Exceptional<bool>(true);
        }

        public virtual bool IsValidIndexOperation(DataContainer data, DBContext myTypeManager, TypesOfBinaryExpression typeOfBinExpr)
        {
            return true;
        }

        private void CleanLowerLevel(LevelKey myLevelKey, DBContext myTypeManager, DBObjectCache dbObjectCache, IExpressionGraph myGraph)
        {
            if (myLevelKey.Level > 0)
            {
                var previousLevelKey = myLevelKey.GetPredecessorLevel();
                HashSet<ObjectUUID> toBeDeletedNodes = new HashSet<ObjectUUID>();

                foreach (var aLowerDBO in myGraph.Select(previousLevelKey, null, false))
                {
                    if(aLowerDBO.Failed)
                    {
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "Could not load DBObjectStream from lower level."));
                    }

                    foreach (var aReferenceUUID in ((IReferenceEdge)aLowerDBO.Value.GetAttribute(myLevelKey.LastEdge.AttrUUID)).GetAllUUIDs())
                    {
                        if (!myGraph.GetLevel(myLevelKey.Level).ExpressionLevels[myLevelKey].Nodes.ContainsKey(aReferenceUUID))
                        {
                            //a reference occurred that is not in the higher level --> found a Zoidberg

                            toBeDeletedNodes.Add(aLowerDBO.Value.ObjectUUID);
                            break;
                        }
                    }
                }

                foreach (var aToBeDeletedNode in toBeDeletedNodes)
                {
                    myGraph.GetLevel(previousLevelKey.Level).RemoveNode(previousLevelKey, aToBeDeletedNode);
                }
            }
        }

        private Exceptional<object> IntegrateUUID(DataContainer data, DBContext dbContext, DBObjectCache dbObjectCache, TypesOfBinaryExpression typeOfBinExpr, IExpressionGraph resultGraph, SessionSettings mySessionToken, LevelKey myLevelKey, IEnumerable<Exceptional<DBObjectStream>> myDBObjects)
        {
            foreach (var aDBO in myDBObjects)
            {
                if (aDBO.Failed)
                {
                    return new Exceptional<object>(aDBO);
                }

                if (IsValidDBObjectStreamForBinExpr(aDBO.Value, data.IdNodes.Item1.LastAttribute, dbContext.DBTypeManager))
                {
                    //check and integrate
                    var result = CheckAndIntegrateDBObjectStream(data, dbContext, dbObjectCache, typeOfBinExpr, resultGraph, aDBO.Value, myLevelKey, mySessionToken);
                    if(!result.Success)
                    {
                        return new Exceptional<object>(result);
                    }
                }
            }

            return new Exceptional<object>();
        }


        private LevelKey CreateLevelKey(IDNode iDNode)
        {
            if (iDNode.Level == 0)
            {
                return new LevelKey(new List<EdgeKey>() { new EdgeKey(iDNode.Edges[0].TypeUUID, null) }, 0);
            }
            else
            {
                if (iDNode.IDNodeParts.Count > iDNode.Edges.Count)
                {
                    //there must be a function
                    if (iDNode.IDNodeParts.Last() is IDNodeFunc)
                    {
                        // the funtion in the last idnode part processes the last attribute

                        if (iDNode.Level == 1)
                        {
                            return new LevelKey(new List<EdgeKey>() { new EdgeKey(iDNode.Edges[0].TypeUUID, null) }, 0);
                        }
                        else
                        {
                            return new LevelKey(iDNode.Edges.Take(iDNode.Level - 1), iDNode.Level - 1);
                        }
                    }
                    else
                    {
                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently it is not implemented to have a funtion in between two edges."));
                    }
                }
                else
                {
                    return new LevelKey(iDNode.Edges.Take(iDNode.Level), iDNode.Level);
                }
            }
        }

        private void IntegrateInGraph(DBObjectStream myDBObjectStream, IExpressionGraph myExpressionGraph, LevelKey myLevelKey, DBContext myTypeManager, DBObjectCache myQueryCache)
        {
            if (this.Type == TypesOfOperators.AffectsLowerLevels)
            {
                myExpressionGraph.AddNode(myDBObjectStream, myLevelKey, 1);
            }
            else
            {
                myExpressionGraph.AddNode(myDBObjectStream, myLevelKey, 0);
            }
        }

        /// <summary>
        /// We need to add an empty level in case, the DBO should not be integerated. Otherwise the select does not know, either the level was never touched or 
        /// not added due to an expression
        /// </summary>
        /// <param name="myDBObjectStream"></param>
        /// <param name="myExpressionGraph"></param>
        /// <param name="myLevelKey"></param>
        /// <param name="myTypeManager"></param>
        /// <param name="myQueryCache"></param>
        private void ExcludeFromGraph(DBObjectStream myDBObjectStream, IExpressionGraph myExpressionGraph, LevelKey myLevelKey, DBContext myTypeManager, DBObjectCache myQueryCache)
        {
            myExpressionGraph.AddEmptyLevel(myLevelKey);
        }
        
        /// <summary>
        /// This method checks and integrates a DBObjectStream into the result graph.
        /// </summary>
        /// <param name="myData">The DataContainer.</param>
        /// <param name="myTypeManager">The TypeManager of the database.</param>
        /// <param name="myQueryCache">The current query cache.</param>
        /// <param name="myTypeOfBinExpr">The kind of the binary expression.</param>
        /// <param name="myResultGraph">The resulting IExpressionGraph.</param>
        /// <param name="myDBObjectStream">The DBObjectStream.</param>
        private Exceptional<object> CheckAndIntegrateDBObjectStream(DataContainer myData, DBContext myTypeManager, DBObjectCache myQueryCache, TypesOfBinaryExpression myTypeOfBinExpr, IExpressionGraph myResultGraph, DBObjectStream myDBObjectStream, LevelKey myLevelKey, SessionSettings mySessionToken)
        {
          
            //get the operand
            var operand = GetOperand(myData.IdNodes.Item1, myData.Extraordinaries.Item1, myTypeManager, myDBObjectStream, myQueryCache, mySessionToken);

            if(operand == null)
            {
                return new Exceptional<object>();
            }


            if (operand.Failed)
            {
                return new Exceptional<object>(operand);
            }

            Exceptional<IOperationValue> tempSimpleOperationResult;

            switch (myTypeOfBinExpr)
            {

                case TypesOfBinaryExpression.LeftComplex:

                    tempSimpleOperationResult = this.SimpleOperation(operand.Value, (IOperationValue)myData.Operands.Item1, myTypeOfBinExpr);

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    tempSimpleOperationResult = this.SimpleOperation((IOperationValue)myData.Operands.Item1, operand.Value, myTypeOfBinExpr);

                    break;

                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Atom:
                default:

                    throw new ArgumentException();
            }

            if (tempSimpleOperationResult.Failed)
            {
                return new Exceptional<object>(tempSimpleOperationResult);
            }

            var tempOperatorResult = (AtomValue)tempSimpleOperationResult.Value;

            if ((Boolean)tempOperatorResult.Value.Value)
            {
                IntegrateInGraph(myDBObjectStream, myResultGraph, myLevelKey, myTypeManager, myQueryCache);
            }
            //else
            //{
            //    /// We need to add an empty level in case, the DBO should not be integerated. Otherwise the select does not know, either the level was never touched or 
            //    /// not added due to an expression
            //    ExcludeFromGraph(myDBObjectStream, myResultGraph, myLevelKey, myTypeManager, myQueryCache);
            //}

            return new Exceptional<object>();
        }

        /// <summary>
        /// This method extracts an IOperationValue out of a DBObjectStream.
        /// </summary>
        /// <param name="myIDNode">The corresponding IDNode.</param>
        /// <param name="myExtraordinary">Sth extraordinary like an aggregate or function call.</param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        /// <param name="myDBObjectStream">The DBObjectStream.</param>
        /// <returns>An IOperationValue.</returns>
        private Exceptional<IOperationValue> GetOperand(IDNode myIDNode, Object myExtraordinary, DBContext dbContext, DBObjectStream myDBObjectStream, DBObjectCache dbObjectCache, SessionSettings mySessionToken)
        {
            var aCtype = GraphDBTypeMapper.ConvertPandora2CSharp(myIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);
            Object dbos = GetDbos(myIDNode, myDBObjectStream, dbContext, mySessionToken, dbObjectCache);

            if (dbos == null)
            {
                return null;
            }
            
            IOperationValue operand = null;

            if (myExtraordinary != null)
            {
                #region extraordinary

                if (myExtraordinary is AggregateNode)
                {
                    var aggrNode = (AggregateNode)myExtraordinary;

                    //result of aggregate
                    var pResult = aggrNode.Aggregate.Aggregate(dbos, myIDNode.LastAttribute, dbContext, dbObjectCache, mySessionToken);
                    if (pResult.Failed)
                        return new Exceptional<IOperationValue>(pResult);

                    aCtype = aggrNode.Aggregate.TypeOfResult;
                    operand = new AtomValue(aggrNode.Aggregate.TypeOfResult, pResult.Value);
                }
                else
                {
                    if (myExtraordinary is FuncCallNode)
                    {
                        var funcCallNode = (FuncCallNode)myExtraordinary;
                        funcCallNode.CallingAttribute = myIDNode.LastAttribute;
                        funcCallNode.CallingObject = dbos;
                        //result of function

                        var pResult = funcCallNode.Execute(myIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager), myDBObjectStream, myIDNode.Reference.Item1, dbContext, myIDNode.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbObjectCache, mySessionToken);
                        if (pResult.Failed)
                            return new Exceptional<IOperationValue>(pResult);

                        //aCtype = funcCallNode.Function.TypeOfResult;
                        //aCtype = ((FuncParameter)pResult.Value).TypeOfOperatorResult;
                        operand = new AtomValue(((FuncParameter)pResult.Value).Value);
                        aCtype = operand.TypeOfValue;
                    }
                    else
                    {
                        return new Exceptional<IOperationValue>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }

                #endregion
            }
            else
            {
                #region simple

                if (aCtype == TypesOfOperatorResult.SetOfDBObjects)
                {
                    operand = new TupleValue(aCtype, dbos, myIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager));
                }
                else
                {
                    if (dbos is AListBaseEdgeType)
                    {
                        operand = new TupleValue(aCtype, dbos, myIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager));
                    }
                    else
                    {
                        operand = new AtomValue(aCtype, dbos);
                    }
                }

                #endregion
            }
            return new Exceptional<IOperationValue>(operand);
        }

        private object GetDbos(IDNode myIDNode, DBObjectStream myDBObjectStream, DBContext dbContext, SessionSettings mySessionToken, DBObjectCache dbObjectCache)
        {
            if (myIDNode.LastAttribute.IsBackwardEdge)
            {
                var contBackwardExcept = myDBObjectStream.ContainsBackwardEdge(myIDNode.LastAttribute.BackwardEdgeDefinition, dbContext, dbObjectCache, myIDNode.LastAttribute.GetRelatedType(dbContext.DBTypeManager));

                if (contBackwardExcept.Failed)
                    throw new GraphDBException(contBackwardExcept.Errors);

                if (contBackwardExcept.Value)
                {
                    var beStream = dbObjectCache.LoadDBBackwardEdgeStream(myIDNode.LastType, myDBObjectStream.ObjectUUID).Value;
                    if (beStream.ContainsBackwardEdge(myIDNode.LastAttribute.BackwardEdgeDefinition))
                        return beStream.GetBackwardEdgeUUIDs(myIDNode.LastAttribute.BackwardEdgeDefinition);
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (myIDNode.LastAttribute.KindOfType == KindsOfType.SpecialAttribute)
                {
                    return myDBObjectStream.GetAttribute(myIDNode.LastAttribute.UUID, myIDNode.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbContext);
                }
                else
                {
                    return myDBObjectStream.GetAttribute(myIDNode.LastAttribute.UUID);
                }
            }
        }

        private bool IsValidDBObjectStreamForBinExpr(DBObjectStream DBObjectStream, TypeAttribute myTypeAttribute, DBTypeManager myDBTypeManager)
        {
            if (DBObjectStream.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBTypeManager), null) || myTypeAttribute.IsBackwardEdge)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CompatibleEdges(List<EdgeKey> actual, List<EdgeKey> refernce)
        {

            UUID referenceUUIDLimit = new UUID(DBConstants.UpperUUIDLimitForBaseTypes);

            for (int i = 0; i < actual.Count; i++)
            {
                if (!actual[i].Equals(refernce[i]))
                {
                    if (i == (actual.Count - 1))
                    {
                        if (actual[i].TypeUUID < referenceUUIDLimit)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private object IIndexEnumerator(IIndexObject<String, ObjectUUID> iIndexDataStructure)
        {
            throw new NotImplementedException();
        }

        #region SimpleOperation Methods

        public override Exceptional<IOperationValue> SimpleOperation(IOperationValue left, IOperationValue right, TypesOfBinaryExpression typeOfBinExpr)
        {
            if (left is AtomValue && right is AtomValue)
                return SimpleOperation((AtomValue)left, (AtomValue)right);
            else if (left is AtomValue && right is TupleValue)
                return SimpleOperation((AtomValue)left, (TupleValue)right);
            else if (left is TupleValue && right is AtomValue)
                return SimpleOperation((TupleValue)left, (AtomValue)right);
            else if (left is TupleValue && right is TupleValue)
                return SimpleOperation((TupleValue)left, (TupleValue)right);

            return new Exceptional<IOperationValue>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        protected Exceptional<IOperationValue> SimpleOperation(AtomValue left, AtomValue right)
        {

            #region Data

            AtomValue resultObject = null;

            #endregion

            ADBBaseObject leftObj = left.Value;
            ADBBaseObject rightObj = right.Value;

            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                return new Exceptional<IOperationValue>(new Error_DataTypeDoesNotMatch(leftObj.ObjectName, rightObj.ObjectName));
            //return new Exceptional<IOperationValue>(new PandoraError(ErrorCode.DataTypeDoesNotMatchValue, (leftObj.ObjectName + " != " + rightObj.ToString())));

            var resultValue = Compare(leftObj, rightObj);
            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue.Value);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected Exceptional<IOperationValue> SimpleOperation(AtomValue left, TupleValue right)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            ADBBaseObject leftObj = left.Value;
            //foreach (ADBBaseObject rightObj in right.Values)
            for (Int32 i=0; i < right.Values.Count; i++)
            {
                ADBBaseObject rightObj = right.Values[i];

                if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                {
                    return new Exceptional<IOperationValue>(new Error_DataTypeDoesNotMatch(leftObj.Type.ToString(), rightObj.Type.ToString()));
                }

                var comp = Compare(left.Value, rightObj);
                if (comp.Failed)
                    return new Exceptional<IOperationValue>(comp);

                if (comp.Value)
                {
                    resultValue = true;
                }
                else
                {
                    resultValue = false;
                    break;
                }
            }
            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected Exceptional<IOperationValue> SimpleOperation(TupleValue left, AtomValue right)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            ADBBaseObject rightObj = right.Value;
            //foreach (ADBBaseObject leftObj in left.Values)
            for(Int32 i=0; i< left.Values.Count; i++)
            {
                ADBBaseObject leftObj = left.Values[i];

                if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                {
                    return new Exceptional<IOperationValue>(new Error_DataTypeDoesNotMatch(leftObj.Type.ToString(), rightObj.Type.ToString()));
                }

                var comp = Compare(leftObj, right.Value);
                if (comp.Failed)
                    return new Exceptional<IOperationValue>(comp);

                if (comp.Value)
                {
                    resultValue = true;
                }
                else
                {
                    resultValue = false;
                    break;
                }
            }
            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected Exceptional<IOperationValue> SimpleOperation(TupleValue left, TupleValue right)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            //foreach (ADBBaseObject leftObj in left.Values)
            for (Int32 i=0; i < left.Values.Count; i++)
            {
                ADBBaseObject leftObj = left.Values[i];
                //foreach (ADBBaseObject rightObj in right.Values)
                for (Int32 j=0; j < right.Values.Count; j++)
                {
                    ADBBaseObject rightObj = right.Values[i];

                    if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                    {
                        return new Exceptional<IOperationValue>(new Error_DataTypeDoesNotMatch(leftObj.Type.ToString(), rightObj.Type.ToString()));
                    }
                    
                    var res = Compare(leftObj, rightObj);
                    if (res.Failed)
                        return new Exceptional<IOperationValue>(res);

                    if (res.Value)
                    {
                        resultValue = true;
                    }
                    else
                    {
                        resultValue = false;
                        break;
                    }
                }
                if (!(Boolean)resultValue)
                    break;
            }
            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected abstract Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight);

        private IEnumerable<ObjectUUID> IndexSingleOperation(AttributeIndex myIndex, IOperationValue myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager indexManager)
        {
            if (myOperationValue is AtomValue)
            {
                return IndexSingleOperation(myIndex, ((AtomValue)myOperationValue).Value, myAttributeUUID, typeOfBinExpr, indexManager);
            }
            else
            {
                if (myOperationValue is TupleValue)
                {
                    return IndexOperation(myIndex, myOperationValue as TupleValue, typeOfBinExpr, indexManager);
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently, it is not implemented to execute an IndexOperation on anything else but AtomValue or TupleValue."));
                }
            }

        }

        public virtual IEnumerable<ObjectUUID> IndexOperationReloaded(AttributeIndex myIndex, List<Tuple<AttributeUUID, ADBBaseObject>> myOperationValues, TypesOfBinaryExpression typeOfBinExpr)
        {
            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }


        public virtual IEnumerable<ObjectUUID> IndexOperation(AttributeIndex myIndex, TupleValue myOperationValues, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager dbIndexManager)
        {
            throw new GraphDBException(new Error_InvalidIndexOperation(myIndex.IndexName));
        }

        public virtual IEnumerable<ObjectUUID> IndexSingleOperation(AttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager indexManager)
        {

            var idxRef = myIndex.GetIndexReference(indexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(idxRef.Errors);
            }

            foreach (var keyValPair in idxRef.Value)
            {
                //ADBBaseObject indexKey = myOperationValue.Clone(keyValPair.Key);
                //PandoraTypeMapper.ConvertToBestMatchingType(ref myOperationValue, ref indexKey);
                var res = Compare(keyValPair.Key.IndexKeyValues[0], myOperationValue);

                if (res.Failed)
                {
                    throw new GraphDBException(res.Push(new Error_InvalidIndexOperation(myIndex.IndexName, keyValPair.Key.IndexKeyValues[0].Value, myOperationValue.Value)).Errors);
                }

                if (res.Value)
                {
                    foreach (var aUUID in keyValPair.Value)
                    {
                        yield return aUUID;
                    }
                }
            }

            yield break;

        }

        #endregion

        /// <summary>
        /// Data container for everything that is needed by binary expressions
        /// </summary>
        public struct DataContainer
        {
            #region IDNodes

            private Tuple<IDNode, IDNode> _idNodes;

            /// <summary>
            /// Tuple of IDNodes (in most cases the left and right one)
            /// </summary>
            public Tuple<IDNode, IDNode> IdNodes
            {
                get { return _idNodes; }
            }

            #endregion

            #region Operands

            private Tuple<Object, Object> _operands;

            /// <summary>
            /// Tuple of operands (in most cases atom or tuple values)
            /// </summary>
            public Tuple<Object, Object> Operands
            {
                get { return _operands; }
            }

            #endregion

            #region Extraordinaries

            private Tuple<Object, Object> _extraordinaries;

            /// <summary>
            /// Extraordinaries are things like aggregates or functions
            /// </summary>
            public Tuple<Object, Object> Extraordinaries
            {
                get { return _extraordinaries; }
            }

            #endregion

            #region constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="IdNodes">The IDNodes of the DataContainer.</param>
            /// <param name="Operands">The Operands of the DataContainer.</param>
            /// <param name="Extraordinaries">The Extraordinaries of the DataContainer.</param>
            public DataContainer(Tuple<IDNode, IDNode> IdNodes, Tuple<Object, Object> Operands, Tuple<Object, Object> Extraordinaries)
            {
                _idNodes = IdNodes;
                _operands = Operands;
                _extraordinaries = Extraordinaries;
            }

            #endregion
        }
    }
}
