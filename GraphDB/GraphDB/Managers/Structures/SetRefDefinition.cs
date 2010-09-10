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
 * SetRefDefinition
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.Structures.EdgeTypes;


namespace sones.GraphDB.Managers.Structures
{
    /// <summary>
    /// A definition for setReference
    /// </summary>
    public class SetRefDefinition
    {

        #region Fields

        private Boolean _IsREFUUID;

        #endregion

        #region Properties

        public ADBBaseObject[] Parameters { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public SetRefDefinition(Structures.TupleDefinition tupleDefinition, ADBBaseObject[] myParameters)
        {

            System.Diagnostics.Debug.Assert(tupleDefinition != null);

            this.TupleDefinition = tupleDefinition;
            this.Parameters = myParameters;
        }

        public SetRefDefinition(Structures.TupleDefinition tupleDefinition, bool myIsREFUUID, ADBBaseObject[] parameters)
        {
            TupleDefinition = tupleDefinition;
            _IsREFUUID = myIsREFUUID;
            Parameters = parameters;
        }

        #endregion

        #region GetCorrespondigDBObjects

        /// <summary>
        /// returns a guid which matches the tupleNode of the setref object.
        /// </summary>
        /// <param name="TypeOfAttribute">GraphType of the attribute.</param>
        /// <param name="dbContext">The TypeManager of the GraphDatabase</param>
        /// <returns>A Guid.</returns>
        public IEnumerable<Exceptional<DBObjectStream>> GetCorrespondigDBObjects(GraphDBType TypeOfAttribute, DBContext dbContext, GraphDBType validationType)
        {

            #region data

            ObjectUUID _referencingGuid = null;

            #endregion

            #region check types

            #region reference type

            //ask guid-index of type

            foreach (TupleElement aTupleElement in TupleDefinition)
            {
                switch (aTupleElement.TypeOfValue)
                {
                    case BasicType.String:

                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    case BasicType.NotABasicType:

                        if (aTupleElement.Value is BinaryExpressionDefinition)
                        {
                            #region BinExp


                            #region data

                            var aUniqueExpr = (BinaryExpressionDefinition)aTupleElement.Value;

                            #endregion

                            //ValidateBinaryExpression(aUniqueExpr, TypeOfAttribute, dbContext);

                            var validateResult = aUniqueExpr.Validate(dbContext, TypeOfAttribute);
                            if (validateResult.Failed())
                            {
                                throw new GraphDBException(validateResult.Errors);
                            }

                            //if (IsValidBinaryExpressionNode(aUniqueExpr, TypeOfAttribute))
                            //{
                            var aResult = aUniqueExpr.Calculon(dbContext, new CommonUsageGraph(dbContext));

                            if (aResult.Success())
                            {
                                foreach (var dbo in aResult.Value.Select(new LevelKey(TypeOfAttribute, dbContext.DBTypeManager), null, false))
                                    yield return dbo;
                            }
                            else
                            {
                                throw new GraphDBException(aResult.Errors);
                            }
                            //}
                            //else
                            //{
                            //throw new GraphDBException(new Error_UnknownDBError("Found an invalid BinaryExpression while analyzing a REF/REFERENCE insert."));
                            //}

                            #endregion
                        }
                        else
                        {
                            #region Tuple

                            if (aTupleElement.Value is TupleDefinition)
                            {
                                var aTupleNode = (TupleDefinition)aTupleElement.Value;

                                if (IsValidTupleNode(aTupleNode.TupleElements, TypeOfAttribute, dbContext))
                                {
                                    #region get partial results

                                    List<List<ObjectUUID>> partialResults = new List<List<ObjectUUID>>();
                                    BinaryExpressionDefinition tempNode = null;

                                    foreach (TupleElement aElement in aTupleNode)
                                    {
                                        tempNode = (BinaryExpressionDefinition)aElement.Value;

                                        ValidateBinaryExpression(tempNode, validationType, dbContext);

                                        var aResult = tempNode.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                        if (aResult.Success())
                                        {
                                            foreach (var dbo in aResult.Value.Select(new LevelKey(TypeOfAttribute, dbContext.DBTypeManager), null, false))
                                                yield return dbo;
                                        }
                                        else
                                        {
                                            throw new GraphDBException(aResult.Errors);
                                        }

                                    }

                                    #endregion

                                    #region merge partial results

                                    int countOfLists = partialResults.Count;

                                    switch (countOfLists)
                                    {
                                        case 0:

                                            //do nothing here

                                            break;

                                        case 1:

                                            if (partialResults[0].Count == 1)
                                            {
                                                _referencingGuid = partialResults[0][0];
                                            }
                                            else
                                            {
                                                throw new GraphDBException(new Error_ReferenceAssignment("Error while evaluating BinaryExpression on REF/REFERENCE. There are more than one references which is not possible in an REF insert."));
                                            }

                                            break;

                                        default:

                                            //doing some intersection of n lists :)

                                            List<ObjectUUID> tempGuidList = GetIntersection(partialResults);

                                            if (tempGuidList.Count == 1)
                                            {
                                                _referencingGuid = tempGuidList[0];
                                            }
                                            else
                                            {
                                                throw new GraphDBException(new Error_ReferenceAssignment("Error while evaluating BinaryExpression on REF/REFERENCE. There are more than one references which is not possible in an REF insert."));
                                            }

                                            break;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    throw new GraphDBException(new Error_UnknownDBError("Found an invalid TupleNode while analyzing a REF/REFERENCE insert"));
                                }
                            }
                            else
                            {
                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                            #endregion

                        }

                        break;

                    default:

                        //it is an expression

                        throw new GraphDBException(new Error_ReferenceAssignment(aTupleElement.Value.ToString()));
                }
            }

            #endregion


            #endregion

            if (_referencingGuid != null)
                yield return dbContext.DBObjectCache.LoadDBObjectStream(TypeOfAttribute, _referencingGuid);

        }

        #endregion

        #region GetEdge

        /// <summary>
        /// Get the content of the SetRef as an edge
        /// </summary>
        /// <param name="typeAttribute"></param>
        /// <param name="dbContext"></param>
        /// <param name="validationType"></param>
        /// <returns></returns>
        public Exceptional<ASingleReferenceEdgeType> GetEdge(TypeAttribute typeAttribute, DBContext dbContext, GraphDBType validationType)
        {
            var retvalue = typeAttribute.EdgeType.GetNewInstance() as ASingleReferenceEdgeType;

            if (retvalue == null)
            {
                return new Exceptional<ASingleReferenceEdgeType>(new Error_InvalidEdgeType(typeAttribute.EdgeType.GetType(), typeof(ASingleReferenceEdgeType)));
            }

            if (_IsREFUUID)
            {
                return TupleDefinition.GetAsUUIDSingleEdge(dbContext, typeAttribute);
            }
            else
            {
                var dbos = GetCorrespondigDBObjects(typeAttribute.GetDBType(dbContext.DBTypeManager), dbContext, validationType);

                if (dbos.CountIsGreater(1))
                {
                    return new Exceptional<ASingleReferenceEdgeType>(new Error_TooManyElementsForEdge(retvalue, dbos.ULongCount()));//"Error while evaluating BinaryExpression on REF/REFERENCE. There are more than one references which is not possible in an REF/REFERENCE insert."));
                }

                var dboToAdd = dbos.FirstOrDefault();
                if (dboToAdd == null)
                {
                    //NLOG: temporarily commented
                    //Logger.Warn("REF/REFERENCE is null because of an expression without any results! Skip adding this attribute.");
                    return new Exceptional<ASingleReferenceEdgeType>(new Error_ReferenceAssignment_EmptyValue(typeAttribute.Name));
                }
                else if (dboToAdd.Failed())
                {
                    return new Exceptional<ASingleReferenceEdgeType>(dboToAdd);
                }
                else
                {
                    retvalue.Set(dboToAdd.Value.ObjectUUID, typeAttribute.DBTypeUUID, Parameters);
                }

            }

            return new Exceptional<ASingleReferenceEdgeType>(retvalue);
        }

        #endregion

        #region private methods

        #region ValidateBinaryExpression

        private Exceptional ValidateBinaryExpression(BinaryExpressionDefinition aUniqueExpr, GraphDBType validationType, DBContext typeManager)
        {



            switch (aUniqueExpr.TypeOfBinaryExpression)
            {

                case TypesOfBinaryExpression.LeftComplex:
                    return ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager);

                case TypesOfBinaryExpression.RightComplex:
                    return ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager);

                case TypesOfBinaryExpression.Complex:
                    return new Exceptional()
                        .Push(ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager))
                        .Push(ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager));

                case TypesOfBinaryExpression.Atom:

                default:
                    return Exceptional.OK;

            }


        }

        private Exceptional ValidateBinaryExpressionInternal(AExpressionDefinition aUniqueExpr, GraphDBType validationType, DBContext dbContext)
        {
            if (aUniqueExpr is BinaryExpressionDefinition)
            {
                return (aUniqueExpr as BinaryExpressionDefinition).Validate(dbContext, validationType);
                //return ValidateBinaryExpression((BinaryExpressionDefinition)aUniqueExpr, validationType, dbContext);
            }
            else
            {
                var _potIdNode = aUniqueExpr as IDChainDefinition;

                if (_potIdNode != null)
                {
                    //var validationResult = _potIdNode.ValidateMe(validationType, typeManager);
                    var validationResult = _potIdNode.Validate(dbContext, false, validationType);
                    return validationResult;
                }
                else
                {
                    return Exceptional.OK;
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks if there is a valid tuple node. Valid tuple nodes in this case look like : (Name = 'Henning', Age = 10)
        /// </summary>
        /// <param name="tupleElementList">List of tuple elements</param>
        /// <param name="myAttributes">myAttributes of the type</param>
        /// <returns>True if valid or otherwise false</returns>
        private bool IsValidTupleNode(List<TupleElement> tupleElementList, GraphDBType myGraphType, DBContext dbContext)
        {
            foreach (TupleElement aTupleElement in tupleElementList)
            {
                if (aTupleElement.Value is BinaryExpressionDefinition)
                {
                    var validateResult = ((BinaryExpressionDefinition)aTupleElement.Value).Validate(dbContext, myGraphType);
                    if (validateResult.Failed())
                    {
                        throw new GraphDBException(validateResult.Errors);
                    }
                    //if (!IsValidBinaryExpressionNode((BinaryExpressionDefinition)aTupleElement.Value, myGraphType))
                    //{
                    //    return false;
                    //}
                }
                else
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// fast check if a given binary expression node is suitable for list integration
        /// </summary>
        /// <param name="aUniqueExpr">A BinaryExpressionNode.</param>
        /// <param name="myAttributes"></param>
        /// <returns></returns>
        private bool IsValidBinaryExpressionNode(BinaryExpressionDefinition aUniqueExpr, Dictionary<string, string> attributes)
        {
            switch (aUniqueExpr.TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    #region left complex

                    return CheckIDNode(aUniqueExpr.Left, attributes);

                    #endregion

                case TypesOfBinaryExpression.RightComplex:

                    #region right complex

                    return CheckIDNode(aUniqueExpr.Right, attributes);

                    #endregion


                case TypesOfBinaryExpression.Complex:

                    #region complex

                    #region Data

                    BinaryExpressionDefinition leftNode = null;
                    BinaryExpressionDefinition rightNode = null;

                    #endregion

                    #region get expr

                    #region left

                    if (aUniqueExpr.Left is BinaryExpressionDefinition)
                    {
                        leftNode = (BinaryExpressionDefinition)aUniqueExpr.Left;
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #region right

                    if (aUniqueExpr.Right is BinaryExpressionDefinition)
                    {
                        rightNode = (BinaryExpressionDefinition)aUniqueExpr.Right;
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #endregion

                    #region check

                    if ((leftNode != null) && (rightNode != null))
                    {
                        if (IsValidBinaryExpressionNode(leftNode, attributes) && IsValidBinaryExpressionNode(rightNode, attributes))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #endregion

                #region error cases

                case TypesOfBinaryExpression.Atom:

                default:

                    //in this kind of node it is not allowed to use Atom or complex expressions
                    return false;

                #endregion

            }

        }

        private bool CheckIDNode(AExpressionDefinition myPotentialIDNode, Dictionary<string, string> attributes)
        {
            if (myPotentialIDNode is IDChainDefinition)
            {
                var chainDefinition = (myPotentialIDNode as IDChainDefinition);
                if (chainDefinition.Level == 0)
                {
                    if (attributes.ContainsKey(chainDefinition.LastAttribute.Name))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return false;
            }
        }

        #region GetIntersection(myContainerList)

        /// <summary>
        /// Intersecting n Lists of Lists of String.
        /// </summary>
        /// <param name="containerList">The List that containts all the other lists</param>
        /// <returns>A list of String which contains the extract of the container list.</returns>
        private List<ObjectUUID> GetIntersection(List<List<ObjectUUID>> myContainerList)
        {

            IEnumerable<ObjectUUID> tempList = myContainerList[0];

            foreach (List<ObjectUUID> list in myContainerList)
                tempList = tempList.Intersect<ObjectUUID>(list, new ObjectUUIDEqualityComparer());

            return tempList.ToList<ObjectUUID>();

        }

        #endregion

        #endregion

    }
}
