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

/* <id name="sones GraphDB – SetRefNode node" />
 * <copyright file="SetRefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of SetRefNode statement.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.Lib.Frameworks.Irony.Parsing;

using sones.Lib.DataStructures;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.Session;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an SetRefNode statement.
    /// </summary>
    class SetRefNode : AStructureNode, IAstNodeInit
    {
        #region Data

        TupleNode _tupleNode = null;
        private Boolean _IsREFUUID = false;
        public ADBBaseObject[] Params { get; set; }

        #endregion

        #region constructor

        public SetRefNode()
        {
            
        }

        #endregion

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var grammar = GetGraphQLGrammar(context);
            if (parseNode.ChildNodes[0].Term == grammar.S_REFUUID || parseNode.ChildNodes[0].Term == grammar.S_REFERENCEUUID)
            {
                _IsREFUUID = true;
            }

            _tupleNode = parseNode.ChildNodes[1].AstNode as TupleNode;

            if (_tupleNode == null)
            {
                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            if (parseNode.ChildNodes[2].AstNode is ParametersNode)
            {
                Params = (parseNode.ChildNodes[2].AstNode as ParametersNode).ParameterValues.ToArray();
            }
        }

        public Exceptional<ASingleReferenceEdgeType> GetEdge(TypeAttribute typeAttribute, DBContext dbContext, GraphDBType validationType)
        {
            var retvalue = typeAttribute.EdgeType.GetNewInstance() as ASingleReferenceEdgeType;

            if (retvalue == null)
            {
                return new Exceptional<ASingleReferenceEdgeType>(new Error_InvalidEdgeType(typeAttribute.EdgeType.GetType(), typeof(ASingleReferenceEdgeType)));
            }

            if (_IsREFUUID)
            {
                return _tupleNode.GetAsUUIDSingleEdge(dbContext, typeAttribute);
            }
            else
            {
                var dbos = GetCorrespondigDBObjects(typeAttribute.GetDBType(dbContext.DBTypeManager), dbContext, validationType);
                
                if (dbos.CountIsGreater(1))
                {
                    throw new GraphDBException(new Error_TooManyElementsForEdge(retvalue, dbos.ULongCount()));//"Error while evaluating BinaryExpression on REF/REFERENCE. There are more than one references which is not possible in an REF/REFERENCE insert."));
                }

                var dboToAdd = dbos.FirstOrDefault();
                if (dboToAdd == null)
                {
                    //NLOG: temporarily commented
                    //Logger.Warn("REF/REFERENCE is null because of an expression without any results! Skip adding this attribute.");
                    throw new GraphDBException(new Error_ReferenceAssignment_EmptyValue(typeAttribute.Name));
                }
                else if (dboToAdd.Failed)
                {
                    throw new GraphDBException(dboToAdd.Errors);
                }
                else
                {
                    retvalue.Set(dboToAdd.Value.ObjectUUID, Params);
                }

            }

            return new Exceptional<ASingleReferenceEdgeType>(retvalue);
        }

        /// <summary>
        /// returns a guid which matches the tupleNode of the setref object.
        /// </summary>
        /// <param name="TypeOfAttribute">PandoraType of the attribute.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDatabase</param>
        /// <returns>A Guid.</returns>
        public IEnumerable<Exceptional<DBObjectStream>> GetCorrespondigDBObjects(GraphDBType TypeOfAttribute, DBContext dbContext, GraphDBType validationType)
        {
            if (_tupleNode == null)
            {
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("Tuple"));
            }

            #region data

            ObjectUUID _referencingGuid = null;

            #endregion

            #region check types



            #region reference type

            //ask guid-index of type

            foreach (TupleElement aTupleElement in _tupleNode.Tuple)
            {
                switch (aTupleElement.TypeOfValue)
                {
                    case TypesOfOperatorResult.String:

                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                        /*
                        #region string

                        ObjectUUID Guid = (ObjectUUID)aTupleElement.Value;

                        var idxRef = TypeOfAttribute.GetUUIDIndex(dbContext.DBTypeManager).GetIndexReference(dbContext.DBIndexManager);
                        if (!idxRef.Success)
                        {
                            throw new GraphDBException(idxRef.Errors);
                        }

                        var Index = idxRef.Value;

                        if (Index.ContainsKey(Guid.ToString()) == Trinary.TRUE)
                        {
                            _referencingGuid = Guid;
                        }
                        else
                        {
                            throw new GraphDBException(new Error_DBObjectDoesNotExistInIndex("The DBObject with UUID \"" + Guid + "\" does not exist in PandoraType \"" + TypeOfAttribute.Name + "\"."));
                        }

                        #endregion

                        break;
                        
                         * */

                    case TypesOfOperatorResult.NotABasicType:

                        if (aTupleElement.Value is BinaryExpressionNode)
                        {
                            #region BinExp


                            #region data

                            BinaryExpressionNode aUniqueExpr = (BinaryExpressionNode)aTupleElement.Value;

                            #endregion

                            ValidateBinaryExpression(aUniqueExpr, TypeOfAttribute, dbContext);

                            if (IsValidBinaryExpressionNode(aUniqueExpr, TypeOfAttribute))
                            {
                                var aResult = aUniqueExpr.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                if (aResult.Success)
                                {
                                    foreach (var dbo in aResult.Value.Select(new LevelKey(TypeOfAttribute), null, false))
                                        yield return dbo;
                                }
                                else
                                {
                                    throw new GraphDBException(aResult.Errors);
                                }
                            }
                            else
                            {
                                throw new GraphDBException(new Error_UnknownDBError("Found an invalid BinaryExpression while analyzing a REF/REFERENCE insert."));
                            }

                            #endregion
                        }
                        else
                        {
                            #region Tuple

                            if (aTupleElement.Value is TupleNode)
                            {
                                TupleNode aTupleNode = (TupleNode)aTupleElement.Value;

                                if (IsValidTupleNode(aTupleNode.Tuple, TypeOfAttribute))
                                {
                                    #region get partial results

                                    List<List<ObjectUUID>> partialResults = new List<List<ObjectUUID>>();
                                    BinaryExpressionNode tempNode = null;

                                    foreach (TupleElement aElement in aTupleNode.Tuple)
                                    {
                                        tempNode = (BinaryExpressionNode)aElement.Value;

                                        ValidateBinaryExpression(tempNode, validationType, dbContext);

                                        var aResult = tempNode.Calculon(dbContext, new CommonUsageGraph(dbContext));

                                        if (aResult.Success)
                                        {
                                            foreach (var dbo in  aResult.Value.Select(new LevelKey(TypeOfAttribute), null, false))
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

        /// <summary>
        /// Checks if there is a valid tuple node. Valid tuple nodes in this case look like : (Name = 'Henning', Age = 10)
        /// </summary>
        /// <param name="tupleElementList">List of tuple elements</param>
        /// <param name="myAttributes">myAttributes of the type</param>
        /// <returns>True if valid or otherwise false</returns>
        private bool IsValidTupleNode(List<TupleElement> tupleElementList, Dictionary<string, string> attributes)
        {
            foreach (TupleElement aTupleElement in tupleElementList)
            {
                if (aTupleElement.Value is BinaryExpressionNode)
                {
                    if (!IsValidBinaryExpressionNode((BinaryExpressionNode)aTupleElement.Value, attributes))
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
        private bool IsValidBinaryExpressionNode(BinaryExpressionNode aUniqueExpr, Dictionary<string, string> attributes)
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

        private bool CheckIDNode(object myPotentialIDNode, Dictionary<string, string> attributes)
        {
            if (myPotentialIDNode is IDNode)
            {
                IDNode aIDNode = (IDNode)myPotentialIDNode;
                if (aIDNode.Level == 0)
                {
                    if (attributes.ContainsKey(aIDNode.LastAttribute.Name))
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


        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        #region accessors

        public TupleNode TupleNodeElement { get { return _tupleNode; } }

        #endregion

    }

}
