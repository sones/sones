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

/* <id name="PandoraDB – abstract structure node" />
 * <copyright file="AStructureNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Abstract class for all structure nodes.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.GraphDB.ObjectManagement;

using sones.Lib.DataStructures;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.ErrorHandling;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.Frameworks.Irony.Parsing;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// Abstract class for all structure nodes.
    /// </summary>
    public abstract class AStructureNode
    {



        protected IDNode GenerateIDNode(ATypeNode myTypeDefinition, SessionSettings mySessionToken)
        {
            #region INPUT exceptions

            if (myTypeDefinition == null)
            {
                throw new ArgumentNullException("The ATypeNode object is null.");
            }

            #endregion

            return new IDNode(myTypeDefinition.DBTypeStream, myTypeDefinition.Reference, mySessionToken);
        }



        protected GraphDBType ExtractDBTypeStreamFromTypeNode(Object myTypeNode)
        {
            #region input exceptions

            if (myTypeNode == null)
            {
                throw new ArgumentNullException();
            }

            #endregion

            if (myTypeNode is ATypeNode)
            {
                return ((ATypeNode)myTypeNode).DBTypeStream as GraphDBType;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected Exceptional<bool> ValidateBinaryExpression(BinaryExpressionNode aUniqueExpr, GraphDBType validationType, DBContext typeManager)
        {

            switch (aUniqueExpr.TypeOfBinaryExpression)
            {

                case TypesOfBinaryExpression.LeftComplex:
                    return ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager);

                case TypesOfBinaryExpression.RightComplex:
                    return ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager);

                case TypesOfBinaryExpression.Complex:
                    return new Exceptional<bool>(ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, typeManager).Value && ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, typeManager).Value);

                case TypesOfBinaryExpression.Atom:

                default:
                    return new Exceptional<bool>(true);

            }


        }

        private Exceptional<bool> ValidateBinaryExpressionInternal(Object aUniqueExpr, GraphDBType validationType, DBContext typeManager)
        {
            if (aUniqueExpr is BinaryExpressionNode)
            {
                return ValidateBinaryExpression((BinaryExpressionNode)aUniqueExpr, validationType, typeManager);
            }
            else
            {
                var _potIdNode = aUniqueExpr as IDNode;

                if (_potIdNode != null)
                {
                    var validationResult = _potIdNode.ValidateMe(validationType, typeManager);

                    if (validationResult.Failed)
                    {
                        return new Exceptional<bool>(false, validationResult);
                    }
                    else
                    {
                        return new Exceptional<bool>(true);
                    }
                }
                else
                {
                    return new Exceptional<bool>(true);
                }
            }
        }

        #region protected helper methods

        /// <summary>
        /// Checks if there is a valid tuple node. Valid tuple nodes in this case look like : (Name = 'Henning', Age = 10)
        /// </summary>
        /// <param name="tupleElementList">List of tuple elements</param>
        /// <param name="myAttributes">myAttributes of the type</param>
        /// <returns>True if valid or otherwise false</returns>
        protected bool IsValidTupleNode(List<TupleElement> tupleElementList, GraphDBType myPandoraType)
        {
            foreach (TupleElement aTupleElement in tupleElementList)
            {
                if (aTupleElement.Value is BinaryExpressionNode)
                {
                    if (!IsValidBinaryExpressionNode((BinaryExpressionNode)aTupleElement.Value, myPandoraType))
                    {
                        return false;
                    }
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
        protected bool IsValidBinaryExpressionNode(BinaryExpressionNode aUniqueExpr, GraphDBType myPandoraType)
        {

            switch (aUniqueExpr.TypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    #region left complex

                    return CheckIDNode(aUniqueExpr.Left);

                    #endregion

                case TypesOfBinaryExpression.RightComplex:

                    #region right complex

                    return CheckIDNode(aUniqueExpr.Right);

                    #endregion


                case TypesOfBinaryExpression.Complex:

                    #region complex

                    #region Data

                    BinaryExpressionNode leftNode = null;
                    BinaryExpressionNode rightNode = null;

                    #endregion

                    #region get expr

                    #region left

                    if (aUniqueExpr.Left is BinaryExpressionNode)
                    {
                        leftNode = (BinaryExpressionNode)aUniqueExpr.Left;
                    }
                    else
                    {
                        return false;
                    }

                    #endregion

                    #region right

                    if (aUniqueExpr.Right is BinaryExpressionNode)
                    {
                        rightNode = (BinaryExpressionNode)aUniqueExpr.Right;
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
                        if (IsValidBinaryExpressionNode(leftNode, myPandoraType) && IsValidBinaryExpressionNode(rightNode, myPandoraType))
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

        private bool CheckIDNode(Object aPossibleIDNode)
        {
            if (aPossibleIDNode is IDNode)
            {
                IDNode left = (IDNode)aPossibleIDNode;

                if (left.Level == 0)
                {
                    //case 1: IDNode is U.Name --> two edges ... trivial --> level 0
                    //case 2: IDNode is Name --> there are also two edges (have a closer look in IDNode.cs) --> level 0

                    //The IDNode checks if the attribute is correct.

                    return true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "IDNode expected but found: " + aPossibleIDNode.GetType()));
            }
        }

        /// <summary>
        /// Intersecting n Lists of Lists of String.
        /// </summary>
        /// <param name="containerList">The List that containts all the other lists</param>
        /// <returns>A list of String which contains the extract of the container list.</returns>
        protected HashSet<ObjectUUID> GetIntersection(HashSet<HashSet<ObjectUUID>> containerList)
        {

            //hack: ahzf removed this old implementation!

            //#region data

            //IEnumerable<ObjectUUID> tempList = containerList.First<HashSet<ObjectUUID>>();
            //GuidEqualityComparer comparer = new GuidEqualityComparer();

            //#endregion

            //#region process containerlist

            //foreach (var list in containerList)
            //{
            //    tempList = tempList.Intersect<ObjectUUID>(list, comparer);
            //}

            //#endregion

            //return tempList.ToList<ObjectUUID>();

            var tempList = containerList.First<HashSet<ObjectUUID>>();

            foreach (var list in containerList)
                tempList.IntersectWith(list);

            return tempList;

        }

        /// <summary>
        /// Returns all guids of a given index
        /// </summary>
        /// <param name="aIndex">An Index.</param>
        /// <returns>List of ObjectUUIDs</returns>
        protected HashSet<ObjectUUID> GetAllDBObjectGUIDsForIndex(IIndexObject<String, ObjectUUID> aIndex)
        {

            var listOfDBObjectGUIDs = new HashSet<ObjectUUID>();

            foreach (var _ObjectUUIDs in aIndex.Values())
                listOfDBObjectGUIDs.UnionWith(_ObjectUUIDs);

            return listOfDBObjectGUIDs;

        }

        #endregion

        #region public methods

        /// <summary>
        /// returns a list of guids which match the tupleNode of the ListOfDBObjects object.
        /// </summary>
        /// <param name="TypeOfAttribute">PandoraType of the attribute.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDatabase</param>
        /// <returns>A List of Guids.</returns>
        public Exceptional<ASetReferenceEdgeType> GetCorrespondigDBObjectGuidAsList(GraphDBType myType, DBContext dbContext, TupleNode _tupleNode, AEdgeType mySourceEdge, GraphDBType validationType)
        {
            #region data

            ASetReferenceEdgeType _referenceEdge = (ASetReferenceEdgeType)mySourceEdge.GetNewInstance();

            #endregion

            #region Evaluate tuple

            //ask guid-index of type
            if (_tupleNode != null)
            {
                foreach (TupleElement aTupleElement in _tupleNode.Tuple)
                {
                    switch (aTupleElement.TypeOfValue)
                    {
                        case TypesOfOperatorResult.String:
                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            /*
                            #region String

                            ObjectUUID Guid = (ObjectUUID)aTupleElement.Value;

                            IIndexObject<String, ObjectUUID> Index = (IIndexObject<String, ObjectUUID>)myType.GetUUIDIndex(dbContext.DBTypeManager).IndexReference;

                            if (Index.ContainsKey(Guid.ToString()) == Trinary.TRUE)
                            {
                                _referenceEdge.Add(Guid);
                            }
                            else
                            {
                                throw new GraphDBException(new Error_DBObjectDoesNotExistInIndex("The DBObject with UUID \"" + Guid + "\" does not exist in PandoraType \"" + myType.Name + "\"."));
                            }

                            break;

                            #endregion
                            */
                        case TypesOfOperatorResult.NotABasicType:

                            if (aTupleElement.Value is BinaryExpressionNode)
                            {
                                #region Binary Expression

                                BinaryExpressionNode aUniqueExpr = (BinaryExpressionNode)aTupleElement.Value;

                                if (ValidateBinaryExpression(aUniqueExpr, validationType, dbContext).Failed)
                                {
                                    return new Exceptional<ASetReferenceEdgeType>(new Error_InvalidBinaryExpression(aUniqueExpr));
                                }

                                if (IsValidBinaryExpressionNode(aUniqueExpr, myType))
                                {
                                    var _graphResult = aUniqueExpr.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                    if (_graphResult.Success)
                                    {
                                        _referenceEdge.AddRange(_graphResult.Value.SelectUUIDs(new LevelKey(validationType),null, true), aTupleElement.Parameters.ToArray());
                                    }
                                    else
                                    {
                                        return new Exceptional<ASetReferenceEdgeType>(_graphResult);
                                    }
                                }
                                else
                                {
                                    throw new GraphDBException(new Error_UnknownDBError("Found an invalid BinaryExpression while analyzing list of DBObjects."));
                                }

                                #endregion
                            }
                            else
                            {
                                #region tuple node

                                if (aTupleElement.Value is TupleNode)
                                {
                                    TupleNode aTupleNode = (TupleNode)aTupleElement.Value;

                                    if (IsValidTupleNode(aTupleNode.Tuple, myType))
                                    {
                                        #region get partial results

                                        HashSet<ObjectUUID> partialResults = new HashSet<ObjectUUID>();
                                        BinaryExpressionNode tempNode = null;

                                        foreach (TupleElement aElement in aTupleNode.Tuple)
                                        {
                                            tempNode = (BinaryExpressionNode)aElement.Value;

                                            if (ValidateBinaryExpression(tempNode, validationType, dbContext).Failed)
                                            {
                                                return new Exceptional<ASetReferenceEdgeType>(new Error_InvalidBinaryExpression(tempNode));
                                            }

                                            var tempGraphResult = tempNode.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                            if (tempGraphResult.Success)
                                            {
                                                partialResults.UnionWith(tempGraphResult.Value.SelectUUIDs(new LevelKey(validationType), null, true));
                                            }
                                            else
                                            {
                                                return new Exceptional<ASetReferenceEdgeType>(tempGraphResult);
                                            }
                                        }

                                        #endregion

                                        _referenceEdge.AddRange(partialResults, aTupleElement.Parameters.ToArray());
                                    }
                                    else
                                    {
                                        throw new GraphDBException(new Error_UnknownDBError("Found an invalid TupleNode while analyzing ListOfDBObjects"));
                                    }
                                }
                                else
                                {
                                    throw new GraphDBException(new Error_NotImplemented(new StackTrace(true), "Error while checking the elements of ListOfDBObjects. A tupleElement is not a BinaryExpression or a Tuple."));
                                    //throw new GraphDBException(new Error_SetOfAssignment("Error while checking the elements of ListOfDBObjects. A tupleElement is not a BinaryExpression or a Tuple."));
                                }

                                #endregion
                            }

                            break;

                        default:

                            //it is an expression

                            throw new NotImplementedException();
                    }
                }
            }

            #endregion

            return new Exceptional<ASetReferenceEdgeType>(_referenceEdge);
        }

        public GraphQL GetGraphQLGrammar(CompilerContext context)
        {
            return context.Compiler.Language.Grammar as GraphQL;
        }

        #endregion


    }
}
