/* <id name="GraphDB – ABinaryCompareOperator" />
 * <copyright file="ABinaryCompareOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;



using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.UUID;
using sones.Lib;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;

using sones.GraphDB.Managers.Structures;

using sones.GraphDB.TypeManagement;



#endregion

namespace sones.GraphDB.Structures.Operators
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
            return new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(leftData.IDChainDefinitions.Item1, rightData.IDChainDefinitions.Item1), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Operands.Item1, rightData.Operands.Item1), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Extraordinaries.Item1, rightData.Extraordinaries.Item1));
        }

        /// <summary>
        /// Extracts data for a binary expression
        /// </summary>
        /// <param name="myComplexValue">The complex part of the binary expression.</param>
        /// <param name="mySimpleValue">The simple/atomic part of the expression.</param>
        /// <param name="errors">The list of errors.</param>
        /// <param name="typeOfBinExpr">The kind of the binary expression</param>
        /// <returns>A data tuple.</returns>
        private Exceptional<DataContainer> ExtractData(AExpressionDefinition myComplexValue, AExpressionDefinition mySimpleValue, ref TypesOfBinaryExpression typeOfBinExpr, DBObjectCache myDBObjectCache, SessionSettings mySessionToken, DBContext dbContext, Boolean aggregateAllowed)
        {
            #region data

            //the complex IDNode (sth. like U.Age or Count(U.Friends))
            IDChainDefinition complexIDNode = null;

            //the value that is on the opposite of the complex IDNode
            AExpressionDefinition simpleValue = null;

            //a complex IDNode may result in a complexValue (i.e. Count(U.Friends) --> 3)
            AExpressionDefinition complexValue = null;

            //reference to former myComplexValue
            AExpressionDefinition extraordinaryValue = null;

            #endregion

            #region extraction

            if (myComplexValue is IDChainDefinition)
            {
                #region IDNode

                #region Data

                complexIDNode = (IDChainDefinition)myComplexValue;
                var validateResult = complexIDNode.Validate(dbContext, false);
                if (validateResult.Failed())
                {
                    return new Exceptional<DataContainer>(validateResult);
                }

                if (complexIDNode.Any(id => id is ChainPartFuncDefinition))
                {
                    if (complexIDNode.Edges.IsNullOrEmpty())
                    {
                        #region parameterless function

                        var fcn = (complexIDNode.First(id => id is ChainPartFuncDefinition) as ChainPartFuncDefinition);

                        // somes functions (aggregates) like SUM are not valid for where expressions, though they are not resolved
                        if (fcn.Function == null)
                            return new Exceptional<DataContainer>(new Error_FunctionDoesNotExist(fcn.FuncName));

                        Exceptional<FuncParameter> pResult = fcn.Function.ExecFunc(dbContext);
                        if (pResult.Failed())
                        {
                            return new Exceptional<DataContainer>(pResult);
                        }

                        //simpleValue = new AtomValue(fcn.Function.TypeOfResult, ((FuncParameter)pResult.Value).Value); //the new simple value extraced from the function
                        simpleValue = new ValueDefinition(((FuncParameter)pResult.Value).Value);
                        typeOfBinExpr = TypesOfBinaryExpression.Unknown; //we do not know if we are left or right associated
                        complexIDNode = null; //we resolved it... so it's null

                        #endregion
                    }
                    else
                    {
                        //extraordinaryValue = (complexIDNode.First(id => id is ChainPartFuncDefinition) as ChainPartFuncDefinition);
                        extraordinaryValue = complexIDNode;

                        if (mySimpleValue is ValueDefinition)
                        {
                            simpleValue = mySimpleValue;
                        }
                    }
                }
                else
                {
                    if(mySimpleValue is ValueDefinition)
                    {
                        try
                        {
                            if (complexIDNode.IsUndefinedAttribute)
                            {
                                throw new GraphDBException(new Error_AttributeIsNotDefined(complexIDNode.UndefinedAttribute));
                            }
                            
                            simpleValue = GetCorrectValueDefinition(complexIDNode.LastAttribute, complexIDNode.LastType, ((ValueDefinition)mySimpleValue), dbContext, mySessionToken);
                        }
                        catch (FormatException)
                        {
                            return new Exceptional<DataContainer>(new Error_DataTypeDoesNotMatch(complexIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name, ((ValueDefinition)mySimpleValue).Value.ObjectName));
                        }
                    }
                    else
                    {
                        if (mySimpleValue is TupleDefinition)
                        {
                            ((TupleDefinition)mySimpleValue).ConvertToAttributeType(complexIDNode.LastAttribute, dbContext);

                            simpleValue = mySimpleValue;
                        }
                        //else if (mySimpleValue is TupleNode)
                        //{
                        //    var simpleValE = (mySimpleValue as TupleNode).GetAsTupleValue(dbContext, complexIDNode.LastAttribute);
                        //    if (!simpleValE.Success())
                        //    {
                        //        return new Exceptional<DataContainer>(simpleValE);
                        //    }
                        //    simpleValue = simpleValE.Value;
                        //}
                        else
                        {
                            //return new Exceptional<DataContainer>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }
                    }
                }

                #endregion


                #endregion
            }
            else if (myComplexValue is TupleDefinition)
            {
                #region TupleSetNode

                complexValue = ((TupleDefinition)myComplexValue);
                simpleValue = mySimpleValue;
                typeOfBinExpr = TypesOfBinaryExpression.Atom;

                #endregion
            }
            else if (myComplexValue is AggregateDefinition)
            {
                #region AggregateNode

                if (aggregateAllowed)
                {
                    if (((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Parameters.Count != 1)
                    {
                        return new Exceptional<DataContainer>(new Error_ArgumentException("An aggregate must have exactly one expression."));
                    }

                    if (!(((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Parameters[0] is IDChainDefinition))
                    {
                       return new Exceptional<DataContainer>(new Error_ArgumentException("An aggregate must have exactly one IDNode."));
                    }

                    #region Data

                    complexIDNode = (((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Parameters[0] as IDChainDefinition);

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
                    return new Exceptional<DataContainer>(new Error_AggregateNotAllowed(((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition));
                }
                #endregion
            }
            else
            {
                return new Exceptional<DataContainer>(new Error_NotImplementedExpressionNode(myComplexValue.GetType())); 
            }

            #endregion

            return new Exceptional<DataContainer>(new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(complexIDNode, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(simpleValue, complexValue), new Tuple<AExpressionDefinition, AExpressionDefinition>(extraordinaryValue, null)));
        }

        private ValueDefinition GetCorrectValueDefinition(TypeAttribute typeAttribute, GraphDBType graphDBType, ValueDefinition myValueDefinition, DBContext dbContext, SessionSettings mySessionsInfos)
        {
            
            if (typeAttribute.IsBackwardEdge)
            {
                return GetCorrectValueDefinition(dbContext.DBTypeManager.GetTypeByUUID(typeAttribute.BackwardEdgeDefinition.TypeUUID).GetTypeAttributeByUUID(typeAttribute.BackwardEdgeDefinition.AttrUUID), graphDBType, myValueDefinition, dbContext, mySessionsInfos);
            }
            else
            {
                if (typeAttribute is ASpecialTypeAttribute && typeAttribute.UUID == SpecialTypeAttribute_UUID.AttributeUUID)
                {
                    //var uuid = SpecialTypeAttribute_UUID.ConvertToUUID(atomValue.Value.Value.ToString(), graphDBType, mySessionsInfos, dbContext.DBTypeManager);


                    //if (uuid.Failed())
                    //{
                    //    throw new GraphDBException(uuid.Errors);
                    //}
                    //return new AtomValue(new DBReference(uuid.Value));
                    return new ValueDefinition(new DBReference(ObjectUUID.FromString(myValueDefinition.Value.Value.ToString())));
                }
                else if (!typeAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    return new ValueDefinition(GraphDBTypeMapper.GetADBBaseObjectFromUUID(typeAttribute.DBTypeUUID, myValueDefinition.Value));
                }
                else
                {
                    throw new GraphDBException(new Error_InvalidAttributeValue(typeAttribute.Name, myValueDefinition.Value));
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
        public override Exceptional<IExpressionGraph> TypeOperation(AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph resultGr, Boolean aggregateAllowed = true)        
        {
            #region Data
            
            //list of errors
            List<GraphDBError> errors = new List<GraphDBError>();
            
            //DataContainer for all data that is used by a binary expression/comparer
            Exceptional<DataContainer> data;

            //the index of the left attribute
            IEnumerable<Tuple<GraphDBType, AAttributeIndex>> leftIndex = null;

            //the index of the right attribute
            IEnumerable<Tuple<GraphDBType, AAttributeIndex>> rightIndex = null;

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
                    if (!data.Success())
                    {
                        return new Exceptional<IExpressionGraph>(data);
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    //sth like 21 = U.Age
                    #region Get RightComplex data

                    data = ExtractData(myRightValueObject, myLeftValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!data.Success())
                    {
                        return new Exceptional<IExpressionGraph>(data);
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.Complex:

                    //sth like U.Age = F.Alter
                    #region Get Complex data

                    var leftData = ExtractData(myLeftValueObject, myRightValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!leftData.Success())
                    {
                        return new Exceptional<IExpressionGraph>(leftData);
                    }

                    var rightData = ExtractData(myRightValueObject, myLeftValueObject, ref typeOfBinExpr, dbContext.DBObjectCache, dbContext.SessionSettings, dbContext, aggregateAllowed);
                    if (!rightData.Success())
                    {
                        return new Exceptional<IExpressionGraph>(rightData);
                    }
                    
                    if (typeOfBinExpr == TypesOfBinaryExpression.Unknown)
                    {
                        typeOfBinExpr = SetTypeOfBinaryExpression(leftData, rightData);

                        switch (typeOfBinExpr)
                        {
                            case TypesOfBinaryExpression.Atom:

                                data = new Exceptional<DataContainer>(new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(null, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Value.Operands.Item1, leftData.Value.Operands.Item1), new Tuple<AExpressionDefinition, AExpressionDefinition>(null, null)));

                                break;
                            case TypesOfBinaryExpression.LeftComplex:

                                data = new Exceptional<DataContainer>(new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(leftData.Value.IDChainDefinitions.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(rightData.Value.Operands.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(null, null)));

                                break;
                            case TypesOfBinaryExpression.RightComplex:

                                data = new Exceptional<DataContainer>(new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(rightData.Value.IDChainDefinitions.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Value.Operands.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(null, null)));

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

            if (data.Failed())
            {
                return new Exceptional<IExpressionGraph>(data);
            }

            #endregion

            #endregion

            #region get indexes

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    leftIndex = GetIndex(data.Value.IDChainDefinitions.Item1.LastAttribute, dbContext, data.Value.IDChainDefinitions.Item1.LastType, data.Value.Extraordinaries.Item1);

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    //data.IdNodes.TupelElement1 is correct, because of correct handling in extract data (data.IdNodes.TupelElement2 should be null here)
                    rightIndex = GetIndex(data.Value.IDChainDefinitions.Item1.LastAttribute, dbContext, data.Value.IDChainDefinitions.Item1.LastType, data.Value.Extraordinaries.Item1);

                    break;

                case TypesOfBinaryExpression.Complex:

                    //both indexe have to be catched

                    leftIndex = GetIndex(data.Value.IDChainDefinitions.Item1.LastAttribute, dbContext, data.Value.IDChainDefinitions.Item1.LastType, data.Value.Extraordinaries.Item1);
                    rightIndex = GetIndex(data.Value.IDChainDefinitions.Item2.LastAttribute, dbContext, data.Value.IDChainDefinitions.Item2.LastType, data.Value.Extraordinaries.Item1);

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
                return new Exceptional<IExpressionGraph>(new Error_InvalidBinaryExpression(this, data.Value.IDChainDefinitions, data.Value.Operands, typeOfBinExpr));
            }

            if (matchDataResult != null && matchDataResult.Failed())
                return new Exceptional<IExpressionGraph>(matchDataResult);
            else
                return new Exceptional<IExpressionGraph>(resultGr);
        }

        private TypesOfBinaryExpression SetTypeOfBinaryExpression(Exceptional<DataContainer> leftData, Exceptional<DataContainer> rightData)
        {
            TypesOfBinaryExpression result;

            if (leftData.Value.IDChainDefinitions.Item1 != null && rightData.Value.IDChainDefinitions.Item1 != null)
            {
                result = TypesOfBinaryExpression.Complex;
            }
            else
            {
                if (leftData.Value.IDChainDefinitions.Item1 == null && rightData.Value.IDChainDefinitions.Item1 == null)
                {
                    result = TypesOfBinaryExpression.Atom;
                }
                else
                {
                    if (leftData.Value.IDChainDefinitions.Item1 != null)
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

        private Exceptional<Boolean> MatchComplexData(TypesOfAssociativity associativity, DataContainer data, DBContext dbContext, DBObjectCache dbObjectCache, TypesOfBinaryExpression typeOfBinExpr, IEnumerable<Tuple<GraphDBType, AAttributeIndex>> leftIndices, IEnumerable<Tuple<GraphDBType, AAttributeIndex>> rightIndices, ref List<GraphDBError> errors, ref IExpressionGraph result, SessionSettings mySessionToken)
        {
            #region data

            Dictionary<DBObjectStream, AOperationDefinition> operandsLeft = null;
            Dictionary<DBObjectStream, AOperationDefinition> operandsRight = null;

            #endregion

            #region handle extraordinaries

            if (data.Extraordinaries.Item1 != null)
            {
                #region left extraordinary
                //there is something like a function or so

                operandsLeft = new Dictionary<DBObjectStream, AOperationDefinition>();

                //we have to calculate the real operand.
                //TODO: try to use attribute idx instead of uuid idx

                foreach (var aLeftIDX in leftIndices)
                {
                    AAttributeIndex currentLeftIdx = null;

                    #region get UUID idx
                    if (!(aLeftIDX.Item2.IsUUIDIndex))
                    {
                        currentLeftIdx = aLeftIDX.Item1.GetUUIDIndex(dbContext.DBTypeManager);
                    }
                    else
                    {
                        currentLeftIdx = aLeftIDX.Item2;
                    }

                    #endregion

                    var currentIndexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(currentLeftIdx.IndexRelatedTypeUUID);

                    foreach (var aObjectUUIDs_Left in currentLeftIdx.GetAllValues(currentIndexRelatedType, dbContext))
                    {
                        foreach (var aObjectUUID_Left in aObjectUUIDs_Left)
                        {
                            var leftDBObject = dbObjectCache.LoadDBObjectStream(aLeftIDX.Item1, aObjectUUID_Left);
                            if (leftDBObject.Failed())
                            {
                                throw new NotImplementedException();
                            }

                            if (IsValidDBObjectStreamForBinExpr(leftDBObject.Value, data.IDChainDefinitions.Item1.LastAttribute, dbContext.DBTypeManager))
                            {
                                var oper = GetOperand(data.IDChainDefinitions.Item1, data.Extraordinaries.Item1, dbContext, leftDBObject.Value, dbObjectCache, mySessionToken);
                                if (oper.Failed())
                                    return new Exceptional<bool>(oper);

                                if (oper != null)
                                {
                                    operandsLeft.Add(leftDBObject.Value, oper.Value);
                                }
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
                operandsRight = new Dictionary<DBObjectStream, AOperationDefinition>();

                foreach (var aRightIDX in rightIndices)
                {
                    AAttributeIndex currentRightIdx = null;

                    #region get UUID idx
                    //we have to calculate the real operand.
                    //TODO: try to use attribute idx instead of uuid idx

                    if (!(aRightIDX.Item2.IsUUIDIndex))
                    {
                        currentRightIdx = aRightIDX.Item1.GetUUIDIndex(dbContext.DBTypeManager);
                    }
                    else
                    {
                        currentRightIdx = aRightIDX.Item2;
                    }

                    #endregion

                    var currentIndexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(currentRightIdx.IndexRelatedTypeUUID);

                    foreach (var aObjectUUIDs_Right in currentRightIdx.GetAllValues(currentIndexRelatedType, dbContext))
                    {
                        foreach (var aObjectUUID_Right in aObjectUUIDs_Right)
                        {
                            var rightDBObject = dbObjectCache.LoadDBObjectStream(aRightIDX.Item1, aObjectUUID_Right);
                            if (rightDBObject.Failed())
                            {
                                throw new NotImplementedException();
                            }

                            if (IsValidDBObjectStreamForBinExpr(rightDBObject.Value, data.IDChainDefinitions.Item2.LastAttribute, dbContext.DBTypeManager))
                            {
                                var oper = GetOperand(data.IDChainDefinitions.Item2, data.Extraordinaries.Item2, dbContext, rightDBObject.Value, dbObjectCache, mySessionToken);
                                if (oper.Failed())
                                    return new Exceptional<bool>(oper);

                                if (oper != null)
                                {
                                    operandsRight.Add(rightDBObject.Value, oper.Value);
                                }
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
                            GetComplexAtom(dbContext, operandsLeft, operandsRight, data.IDChainDefinitions.Item1, dbObjectCache, ref result);
                            GetComplexAtom(dbContext, operandsRight, operandsLeft, data.IDChainDefinitions.Item2, dbObjectCache, ref result);
                            break;

                        case TypesOfAssociativity.Left:

                            GetComplexAtom(dbContext, operandsLeft, operandsRight, data.IDChainDefinitions.Item1, dbObjectCache, ref result);

                            break;
                        case TypesOfAssociativity.Right:

                            GetComplexAtom(dbContext, operandsRight, operandsLeft, data.IDChainDefinitions.Item2, dbObjectCache, ref result);

                            break;

                        default:
                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.LeftComplex:

                    #region Left complex

                    GetComplexMatch(leftIndices, operandsRight, dbObjectCache, data.IDChainDefinitions.Item1, data.IDChainDefinitions.Item2, dbContext, associativity, ref result, mySessionToken);

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    #region Right complex

                    GetComplexMatch(rightIndices, operandsLeft, dbObjectCache, data.IDChainDefinitions.Item2, data.IDChainDefinitions.Item1, dbContext, associativity, ref result, mySessionToken);

                    #endregion

                    break;
                case TypesOfBinaryExpression.Complex:

                    #region Complex

                    LevelKey leftLevelKey = CreateLevelKey(data.IDChainDefinitions.Item1, dbContext.DBTypeManager);
                    LevelKey rightLevelKey = CreateLevelKey(data.IDChainDefinitions.Item2, dbContext.DBTypeManager);
                    GraphDBType leftGraphDBType = data.IDChainDefinitions.Item1.LastType;
                    GraphDBType rightGraphDBType = data.IDChainDefinitions.Item2.LastType;

                    #region exception

                    if (leftIndices.CountIsGreater(1) || rightIndices.CountIsGreater(1))
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    var leftIndexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(leftIndices.First().Item2.IndexRelatedTypeUUID);
                    var rightIndexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(rightIndices.First().Item2.IndexRelatedTypeUUID);

                    foreach (var ObjectUUIDs_left in leftIndices.First().Item2.GetAllValues(leftIndexRelatedType, dbContext))
                    {
                        foreach (var aLeftUUID in ObjectUUIDs_left)
                        {
                            var leftDBObject = dbObjectCache.LoadDBObjectStream(leftIndices.First().Item1, aLeftUUID);
                            if (leftDBObject.Failed())
                            {
                                return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                            if (IsValidDBObjectStreamForBinExpr(leftDBObject.Value, data.IDChainDefinitions.Item1.LastAttribute, dbContext.DBTypeManager))
                            {

                                foreach (var ObjectUUIDs_right in rightIndices.First().Item2.GetAllValues(rightIndexRelatedType, dbContext))
                                {
                                    foreach (var aRightUUID in ObjectUUIDs_right)
                                    {

                                        var rightDBObject = dbObjectCache.LoadDBObjectStream(rightIndices.First().Item1, aRightUUID);
                                        if (rightDBObject.Failed())
                                        {
                                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                        }

                                        if (IsValidDBObjectStreamForBinExpr(rightDBObject.Value, data.IDChainDefinitions.Item2.LastAttribute, dbContext.DBTypeManager))
                                        {
                                            //everything is valid

                                            var leftType = GraphDBTypeMapper.ConvertGraph2CSharp(data.IDChainDefinitions.Item1.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);
                                            var rightType = GraphDBTypeMapper.ConvertGraph2CSharp(data.IDChainDefinitions.Item2.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);

                                            AOperationDefinition leftValue;
                                            AOperationDefinition rightValue;

                                            if (data.IDChainDefinitions.Item1.LastAttribute.KindOfType == KindsOfType.SetOfReferences
                                                || data.IDChainDefinitions.Item1.LastAttribute.KindOfType == KindsOfType.ListOfNoneReferences || data.IDChainDefinitions.Item1.LastAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                                                leftValue = new TupleDefinition(leftType, leftDBObject.Value.GetAttribute(data.IDChainDefinitions.Item1.LastAttribute.UUID), data.IDChainDefinitions.Item1.LastAttribute.GetDBType(dbContext.DBTypeManager));
                                            else
                                                leftValue = new ValueDefinition(leftType, leftDBObject.Value.GetAttribute(data.IDChainDefinitions.Item1.LastAttribute.UUID, data.IDChainDefinitions.Item1.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbContext));

                                            if (data.IDChainDefinitions.Item2.LastAttribute.KindOfType == KindsOfType.SetOfReferences
                                                || data.IDChainDefinitions.Item2.LastAttribute.KindOfType == KindsOfType.ListOfNoneReferences || data.IDChainDefinitions.Item2.LastAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                                                rightValue = new TupleDefinition(rightType, rightDBObject.Value.GetAttribute(data.IDChainDefinitions.Item2.LastAttribute.UUID), data.IDChainDefinitions.Item2.LastAttribute.GetDBType(dbContext.DBTypeManager));
                                            else
                                                rightValue = new ValueDefinition(rightType, rightDBObject.Value.GetAttribute(data.IDChainDefinitions.Item2.LastAttribute.UUID, data.IDChainDefinitions.Item2.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbContext));

                                            var tempSimpleOperationResult = this.SimpleOperation(leftValue, rightValue, typeOfBinExpr);
                                            if (tempSimpleOperationResult.Failed())
                                                return new Exceptional<bool>(tempSimpleOperationResult);

                                            var tempOperatorResult = tempSimpleOperationResult.Value;

                                            if ((Boolean)(tempOperatorResult as ValueDefinition).Value.Value)
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

        private Exceptional<Boolean> GetComplexMatch(IEnumerable<Tuple<GraphDBType, AAttributeIndex>> myIDX, Dictionary<DBObjectStream, AOperationDefinition> operands, DBObjectCache dbObjectCache, IDChainDefinition primIDNode, IDChainDefinition operandIDNode, DBContext dbContext, TypesOfAssociativity associativity, ref IExpressionGraph resultGraph, SessionSettings mySessionToken)
        {
            LevelKey primLevelKey = CreateLevelKey(primIDNode, dbContext.DBTypeManager);
            LevelKey operandLevelKey = CreateLevelKey(operandIDNode, dbContext.DBTypeManager);

            foreach (var aIDX in myIDX)
            {
                if (aIDX.Item2.IsUUIDIndex)
                {
                    #region UUID idx

                    var currentIndexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(aIDX.Item2.IndexRelatedTypeUUID);

                    foreach (var aOperand in operands)
                    {

                        foreach (var _ObjectUUIDs in ((UUIDIndex)aIDX.Item2).GetAllUUIDs(currentIndexRelatedType, dbContext))
                        {
                            var DBObjectStream = dbObjectCache.LoadDBObjectStream(aIDX.Item1, _ObjectUUIDs);
                            if (DBObjectStream.Failed())
                            {
                                return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                            if (IsValidDBObjectStreamForBinExpr(DBObjectStream.Value, primIDNode.LastAttribute, dbContext.DBTypeManager))
                            {
                                var aCtype = GraphDBTypeMapper.ConvertGraph2CSharp(primIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);
                                IObject dbos = GetDbos(primIDNode, DBObjectStream.Value, dbContext, mySessionToken, dbObjectCache);

                                Exceptional<AOperationDefinition> tempResult;
                                if (aCtype == BasicType.SetOfDBObjects)
                                {
                                    tempResult = this.SimpleOperation(new TupleDefinition(aCtype, dbos, primIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager)), aOperand.Value, TypesOfBinaryExpression.Complex);
                                }
                                else
                                {
                                    tempResult = this.SimpleOperation(new ValueDefinition(aCtype, dbos), aOperand.Value, TypesOfBinaryExpression.Complex);

                                }

                                if (tempResult.Failed())
                                    return new Exceptional<bool>(tempResult);

                                var tempOperatorResult = ((ValueDefinition)tempResult.Value);

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
                }
            }

            return new Exceptional<bool>(true);

        }

        private Exceptional<IExpressionGraph> GetComplexAtom(DBContext dbContext, Dictionary<DBObjectStream, AOperationDefinition> operandsPrim, Dictionary<DBObjectStream, AOperationDefinition> operandsComparism, IDChainDefinition myIDChainDefinition, DBObjectCache dbObjectCache, ref IExpressionGraph result)
        {
            #region data

            LevelKey myLevelKey = CreateLevelKey(myIDChainDefinition, dbContext.DBTypeManager);

            #endregion

            foreach (var left in operandsPrim)
            {
                foreach (var right in operandsComparism)
                {
                    var tempResult = this.SimpleOperation(left.Value, right.Value, TypesOfBinaryExpression.Atom);
                    if (tempResult.Failed())
                        return new Exceptional<IExpressionGraph>(tempResult);

                    if ((Boolean)((ValueDefinition)tempResult.Value).Value.Value)
                    {
                        IntegrateInGraph(left.Key, result, myLevelKey, dbContext,dbObjectCache);
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
                    yield return new sones.GraphDB.Structures.ExpressionGraph.ExpressionNode(aObjectUUID, null);
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
        private Exceptional<Boolean> MatchData(DataContainer data, DBContext dbContext, DBObjectCache dbObjectCache, TypesOfBinaryExpression typeOfBinExpr, IEnumerable<Tuple<GraphDBType, AAttributeIndex>> idx, IExpressionGraph resultGraph, SessionSettings mySessionToken)
        {
            #region data

            LevelKey myLevelKey = CreateLevelKey(data.IDChainDefinitions.Item1, dbContext.DBTypeManager);

            #endregion

            foreach (var aIDX in idx)
            {

                //this is only for such type(DBVertex) that have no AAttributeIndex
                if (aIDX.Item2 == null)
                {
                    continue;
                }
                
                #region Execution

                if (aIDX.Item2.IsUUIDIndex && (data.IDChainDefinitions.Item1.LastAttribute != dbContext.DBTypeManager.GetUUIDTypeAttribute()))
                {
                    #region UUID idx - check ALL DBOs

                    var IndexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(aIDX.Item2.IndexRelatedTypeUUID);

                    var idxKeyValues = (aIDX.Item2).GetAllValues(IndexRelatedType, dbContext);
                    if (idxKeyValues.CountIs(0))
                    {
                        continue;
                    }

                    var uuids = idxKeyValues.Aggregate((elem, aggrresult) => aggrresult.Union(elem));

                    var result = IntegrateUUID(data, dbContext, dbObjectCache, typeOfBinExpr, resultGraph, mySessionToken, myLevelKey, dbObjectCache.LoadListOfDBObjectStreams(aIDX.Item1, uuids));
                    if (result.Failed())
                    {
                        return new Exceptional<bool>(result);
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

                    var operationValue = (AOperationDefinition)data.Operands.Item1;

                    #endregion

                    #region Attribute index

                    if (aIDX.Item2.IndexKeyDefinition.IndexKeyAttributeUUIDs.Count > 1)
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently it is not implemented to use compound indices."));
                    }

                    var interestingUUIDsByIdx = IndexSingleOperation(aIDX.Item2, operationValue, data.IDChainDefinitions.Item1.LastAttribute.UUID, typeOfBinExpr, dbContext);


                    #region integrate in graph

                    var runMT = DBConstants.RunMT;

                    if (runMT)
                    {
                        Parallel.ForEach(dbObjectCache.LoadListOfDBObjectStreams(data.IDChainDefinitions.Item1.LastType, interestingUUIDsByIdx), aDBO =>
                        {
                            IntegrateInGraph(aDBO.Value, resultGraph, myLevelKey, dbContext, dbObjectCache);
                        }
                        );
                    }
                    else
                    {
                        foreach (var aDBO in dbObjectCache.LoadListOfDBObjectStreams(data.IDChainDefinitions.Item1.LastType, interestingUUIDsByIdx))
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

        private void CleanLowerLevel(LevelKey myLevelKey, DBContext dbContext, DBObjectCache dbObjectCache, IExpressionGraph myGraph)
        {
            if (myLevelKey.Level > 0)
            {
                var previousLevelKey = myLevelKey.GetPredecessorLevel(dbContext.DBTypeManager);
                HashSet<ObjectUUID> toBeDeletedNodes = new HashSet<ObjectUUID>();

                foreach (var aLowerDBO in myGraph.Select(previousLevelKey, null, false))
                {
                    if(aLowerDBO.Failed())
                    {
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "Could not load DBObjectStream from lower level."));
                    }

                    foreach (var aReferenceUUID in ((IReferenceEdge)aLowerDBO.Value.GetAttribute(myLevelKey.LastEdge.AttrUUID)).GetAllReferenceIDs())
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
                if (aDBO.Failed())
                {
                    return new Exceptional<object>(aDBO);
                }

                if (IsValidDBObjectStreamForBinExpr(aDBO.Value, data.IDChainDefinitions.Item1.LastAttribute, dbContext.DBTypeManager))
                {
                    //check and integrate
                    var result = CheckAndIntegrateDBObjectStream(data, dbContext, dbObjectCache, typeOfBinExpr, resultGraph, aDBO.Value, myLevelKey, mySessionToken);
                    if(!result.Success())
                    {
                        return new Exceptional<object>(result);
                    }
                }
            }

            return new Exceptional<object>();
        }


        private LevelKey CreateLevelKey(IDChainDefinition myIDChainDefinition, DBTypeManager myTypeManager)
        {
            if (myIDChainDefinition.Level == 0)
            {
                return new LevelKey(new List<EdgeKey>() { new EdgeKey(myIDChainDefinition.Edges[0].TypeUUID, null) }, myTypeManager);
            }
            else
            {
                if (myIDChainDefinition.Last() is ChainPartFuncDefinition)
                {
                    // the funtion in the last idnode part processes the last attribute

                    if (myIDChainDefinition.Level == 1)
                    {
                        return new LevelKey(new List<EdgeKey>() { new EdgeKey(myIDChainDefinition.Edges[0].TypeUUID, null) }, myTypeManager);
                    }
                    else
                    {
                        return new LevelKey(myIDChainDefinition.Edges.Take(myIDChainDefinition.Level - 1), myTypeManager);
                    }
                }
                else
                {
                    return new LevelKey(myIDChainDefinition.Edges.Take(myIDChainDefinition.Level), myTypeManager);
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
            var operand = GetOperand(myData.IDChainDefinitions.Item1, myData.Extraordinaries.Item1, myTypeManager, myDBObjectStream, myQueryCache, mySessionToken);

            if(operand == null)
            {
                return new Exceptional<object>();
            }


            if (operand.Failed())
            {
                return new Exceptional<object>(operand);
            }

            Exceptional<AOperationDefinition> tempSimpleOperationResult;

            switch (myTypeOfBinExpr)
            {

                case TypesOfBinaryExpression.LeftComplex:

                    tempSimpleOperationResult = this.SimpleOperation(operand.Value, ((AOperationDefinition)myData.Operands.Item1), myTypeOfBinExpr);

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    tempSimpleOperationResult = this.SimpleOperation(((AOperationDefinition)myData.Operands.Item1), operand.Value, myTypeOfBinExpr);

                    break;

                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Atom:
                default:

                    throw new ArgumentException();
            }

            if (tempSimpleOperationResult.Failed())
            {
                return new Exceptional<object>(tempSimpleOperationResult);
            }

            var tempOperatorResult = ((ValueDefinition)tempSimpleOperationResult.Value);

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
        /// <param name="myIDChainDefinition">The corresponding IDNode.</param>
        /// <param name="myExtraordinary">Sth extraordinary like an aggregate or function call.</param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        /// <param name="myDBObjectStream">The DBObjectStream.</param>
        /// <returns>An IOperationValue.</returns>
        private Exceptional<AOperationDefinition> GetOperand(IDChainDefinition myIDChainDefinition, AExpressionDefinition myExtraordinary, DBContext dbContext, DBObjectStream myDBObjectStream, DBObjectCache dbObjectCache, SessionSettings mySessionToken)
        {
            var aCtype = GraphDBTypeMapper.ConvertGraph2CSharp(myIDChainDefinition.LastAttribute.GetDBType(dbContext.DBTypeManager).Name);
            IObject dbos = GetDbos(myIDChainDefinition, myDBObjectStream, dbContext, mySessionToken, dbObjectCache);

            if (dbos == null)
            {
                return null;
            }

            AOperationDefinition operand = null;

            if (myExtraordinary != null)
            {
                #region extraordinary

                if (myExtraordinary is AggregateDefinition)
                {
                    System.Diagnostics.Debug.Assert(false);

                    //var aggrNode = ((AggregateDefinition)myExtraordinary).ChainPartAggregateDefinition;

                    //aggrNode.Validate(dbContext);

                    ////result of aggregate
                    //var pResult = aggrNode.Aggregate.Aggregate(dbos, myIDChainDefinition.LastAttribute, dbContext);
                    //if (pResult.Failed())
                    //    return new Exceptional<AOperationDefinition>(pResult);

                    //aCtype = aggrNode.Aggregate.TypeOfResult;
                    //operand = new ValueDefinition(aggrNode.Aggregate.TypeOfResult, pResult.Value);
                }
                else
                {
                    if (myExtraordinary is IDChainDefinition)
                    {
                        var chainFunc = (myExtraordinary as IDChainDefinition).First(id => id is ChainPartFuncDefinition) as ChainPartFuncDefinition;

                        chainFunc.Validate(dbContext);

                        var func = chainFunc.Function;
                        func.CallingAttribute = myIDChainDefinition.LastAttribute;
                        func.CallingObject = dbos;
                        //result of function

                        var pResult = chainFunc.Execute(myIDChainDefinition.LastAttribute.GetDBType(dbContext.DBTypeManager), myDBObjectStream, myIDChainDefinition.Reference.Item1, dbContext);
                        if (pResult.Failed())
                            return new Exceptional<AOperationDefinition>(pResult);

                        //aCtype = funcCallNode.Function.TypeOfResult;
                        //aCtype = ((FuncParameter)pResult.Value).TypeOfOperatorResult;
                        operand = new ValueDefinition(((FuncParameter)pResult.Value).Value);
                        aCtype = (operand as ValueDefinition).Value.Type;
                    }
                    else
                    {
                    return new Exceptional<AOperationDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }

                #endregion
            }
            else
            {
                #region simple

                if (aCtype == BasicType.SetOfDBObjects)
                {
                    operand = new TupleDefinition(aCtype, dbos, myIDChainDefinition.LastAttribute.GetDBType(dbContext.DBTypeManager));
                }
                else
                {
                    if (dbos is AListOfBaseEdgeType)
                    {
                        operand = new TupleDefinition(aCtype, dbos, myIDChainDefinition.LastAttribute.GetDBType(dbContext.DBTypeManager));
                    }
                    else if (dbos is ADBBaseObject)
                    {
                        operand = new ValueDefinition(dbos as ADBBaseObject);
                    }
                    else
                    {
                        operand = new ValueDefinition(aCtype, dbos);
                    }
                }

                #endregion
            }
            return new Exceptional<AOperationDefinition>(operand);
        }

        private IObject GetDbos(IDChainDefinition myIDChainDefinition, DBObjectStream myDBObjectStream, DBContext dbContext, SessionSettings mySessionToken, DBObjectCache dbObjectCache)
        {
            if (myIDChainDefinition.LastAttribute.IsBackwardEdge)
            {
                var contBackwardExcept = myDBObjectStream.ContainsBackwardEdge(myIDChainDefinition.LastAttribute.BackwardEdgeDefinition, dbContext, dbObjectCache, myIDChainDefinition.LastAttribute.GetRelatedType(dbContext.DBTypeManager));

                if (contBackwardExcept.Failed())
                    throw new GraphDBException(contBackwardExcept.IErrors);

                if (contBackwardExcept.Value)
                {
                    var beStream = dbObjectCache.LoadDBBackwardEdgeStream(myIDChainDefinition.LastType, myDBObjectStream.ObjectUUID).Value;
                    if (beStream.ContainsBackwardEdge(myIDChainDefinition.LastAttribute.BackwardEdgeDefinition))
                        return beStream.GetBackwardEdges(myIDChainDefinition.LastAttribute.BackwardEdgeDefinition);
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
                if (myIDChainDefinition.LastAttribute.KindOfType == KindsOfType.SpecialAttribute)
                {
                    return myDBObjectStream.GetAttribute(myIDChainDefinition.LastAttribute.UUID, myIDChainDefinition.LastAttribute.GetRelatedType(dbContext.DBTypeManager), dbContext);
                }
                else
                {
                    return myDBObjectStream.GetAttribute(myIDChainDefinition.LastAttribute.UUID);
                }
            }
        }

        private bool IsValidDBObjectStreamForBinExpr(DBObjectStream DBObjectStream, TypeAttribute myTypeAttribute, DBTypeManager myDBTypeManager)
        {
            if (DBObjectStream.HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetRelatedType(myDBTypeManager)) || myTypeAttribute.IsBackwardEdge)
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

        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression typeOfBinExpr)
        {
            if (left is ValueDefinition && right is ValueDefinition)
                return SimpleOperation((ValueDefinition)left, (ValueDefinition)right);
            else if (left is ValueDefinition && right is TupleDefinition)
                return SimpleOperation((ValueDefinition)left, (TupleDefinition)right);
            else if (left is TupleDefinition && right is ValueDefinition)
                return SimpleOperation((TupleDefinition)left, (ValueDefinition)right);
            else if (left is TupleDefinition && right is TupleDefinition)
                return SimpleOperation((TupleDefinition)left, (TupleDefinition)right);

            return new Exceptional<AOperationDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        protected Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, ValueDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;

            #endregion

            ADBBaseObject leftObj  = left.Value;
            ADBBaseObject rightObj = right.Value;

            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObj.ObjectName, rightObj.ObjectName));
            //return new Exceptional<IOperationValue>(new GraphError(ErrorCode.DataTypeDoesNotMatchValue, (leftObj.ObjectName + " != " + rightObj.ToString())));

            var resultValue = Compare(leftObj, rightObj);
            resultObject = new ValueDefinition(BasicType.Boolean, (object)resultValue.Value);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, TupleDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            ADBBaseObject leftObj = left.Value;
            //foreach (ADBBaseObject rightObj in right.Values)
            foreach(var  val in right)
            {
                ADBBaseObject rightObj = (val.Value as ValueDefinition).Value;

                if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                {
                    return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObj.Type.ToString(), rightObj.Type.ToString()));
                }

                var comp = Compare(left.Value, rightObj);
                if (comp.Failed())
                    return new Exceptional<AOperationDefinition>(comp);

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
            resultObject = new ValueDefinition(BasicType.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected Exceptional<AOperationDefinition> SimpleOperation(TupleDefinition left, ValueDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            ADBBaseObject rightObj = right.Value;
            //foreach (ADBBaseObject leftObj in left.Values)
            foreach(var val in left)
            {
                ADBBaseObject leftObj = (val.Value as ValueDefinition).Value;

                if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                {
                    return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObj.Type.ToString(), rightObj.Type.ToString()));
                }

                var comp = Compare(leftObj, right.Value);
                if (comp.Failed())
                    return new Exceptional<AOperationDefinition>(comp);

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
            resultObject = new ValueDefinition(BasicType.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected Exceptional<AOperationDefinition> SimpleOperation(TupleDefinition left, TupleDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            //foreach (ADBBaseObject leftObj in left.Values)
            foreach (var valLeft in left)
            {
                ADBBaseObject leftObj = (valLeft.Value as ValueDefinition).Value;
                //foreach (ADBBaseObject rightObj in right.Values)
                foreach(var valRight in right)
                {
                    ADBBaseObject rightObj = (valRight.Value as ValueDefinition).Value;

                    if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObj, ref rightObj).Value)
                    {
                        return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObj.Type.ToString(), rightObj.Type.ToString()));
                    }
                    
                    var res = Compare(leftObj, rightObj);
                    if (res.Failed())
                        return new Exceptional<AOperationDefinition>(res);

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
            resultObject = new ValueDefinition(BasicType.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected abstract Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight);

        private IEnumerable<ObjectUUID> IndexSingleOperation(AAttributeIndex myIndex, AOperationDefinition myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            if (myOperationValue is ValueDefinition)
            {
                return IndexSingleOperation(myIndex, ((ValueDefinition)myOperationValue).Value, myAttributeUUID, typeOfBinExpr, dbContext);
            }
            else
            {
                if (myOperationValue is TupleDefinition)
                {
                    return IndexOperation(myIndex, (myOperationValue as TupleDefinition), typeOfBinExpr, dbContext);
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently, it is not implemented to execute an IndexOperation on anything else but AtomValue or TupleValue."));
                }
            }

        }

        public virtual IEnumerable<ObjectUUID> IndexOperationReloaded(AAttributeIndex myIndex, List<Tuple<AttributeUUID, ADBBaseObject>> myOperationValues, TypesOfBinaryExpression typeOfBinExpr)
        {
            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }


        public virtual IEnumerable<ObjectUUID> IndexOperation(AAttributeIndex myIndex, TupleDefinition myOperationValues, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            throw new GraphDBException(new Error_InvalidIndexOperation(myIndex.IndexName));
        }

        public virtual IEnumerable<ObjectUUID> IndexSingleOperation(AAttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            foreach (var keyValPair in myIndex.GetKeyValues(dbContext.DBTypeManager.GetTypeByUUID(myIndex.IndexRelatedTypeUUID), dbContext))
            {
                var res = Compare(keyValPair.Key.IndexKeyValues[0], myOperationValue);

                if (res.Failed())
                {
                    throw new GraphDBException(res.PushIError(new Error_InvalidIndexOperation(myIndex.IndexName, keyValPair.Key.IndexKeyValues[0].Value, myOperationValue.Value)).IErrors);
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

            private Tuple<IDChainDefinition, IDChainDefinition> _IDChainDefinitions;
            public Tuple<IDChainDefinition, IDChainDefinition> IDChainDefinitions
            {
                get
                {
                    return _IDChainDefinitions;
                }
            }

            #endregion

            #region Operands

            private Tuple<AExpressionDefinition, AExpressionDefinition> _operands;

            /// <summary>
            /// Tuple of operands (in most cases ValueDefinition or tupleDefinition)
            /// </summary>
            public Tuple<AExpressionDefinition, AExpressionDefinition> Operands
            {
                get { return _operands; }
            }

            #endregion

            #region Extraordinaries

            private Tuple<AExpressionDefinition, AExpressionDefinition> _extraordinaries;

            /// <summary>
            /// Extraordinaries are things like aggregates or functions
            /// </summary>
            public Tuple<AExpressionDefinition, AExpressionDefinition> Extraordinaries
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
            public DataContainer(Tuple<IDChainDefinition, IDChainDefinition> myIDChainDefinition, Tuple<AExpressionDefinition, AExpressionDefinition> Operands, Tuple<AExpressionDefinition, AExpressionDefinition> Extraordinaries)
            {
                _operands = Operands;
                _extraordinaries = Extraordinaries;
                _IDChainDefinitions = myIDChainDefinition;
           }

            #endregion
        }
    }
}
