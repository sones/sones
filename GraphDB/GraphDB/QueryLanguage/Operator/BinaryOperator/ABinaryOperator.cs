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

/* <id name="sones GraphDB - abstract binary operatror class" />
 * <copyright file="ABinaryOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <develoepr>Daniel Kirstenpfad</deveoper>
 * <summary>This abstract class is responsible for operators
 * which are triggered by binary expression nodes.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.Operator;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.TypeManagement.PandoraTypes;


#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This abstract class is responsible for operators
    /// which are triggered by binary expression nodes.
    /// </summary>
    public abstract class ABinaryOperator
    {

        #region General Operator infos

        public abstract String[]            Symbol                      { get; }
        public abstract String              ContraryOperationSymbol     { get; }
        public abstract BinaryOperator      Label                       { get; } 
        public abstract TypesOfOperators    Type                        { get;}

        #endregion

        #region (public) Methods

        public abstract Exceptional<IOperationValue> SimpleOperation(IOperationValue left, IOperationValue right, TypesOfBinaryExpression typeOfBinExpr);

        public abstract Exceptional<IExpressionGraph> TypeOperation(object myLeftValueObject, object myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, Boolean aggregateAllowed = true);

        #endregion

        public List<String> GetAttributeList(List<String> aList)
        {

            if (aList.Count == 1)
            {
                return aList;
            }
            else
            {
                return aList.GetRange(1, aList.Count - 1);
            }

        }
        
        /// <summary>
        /// Loads an index from a corresponding type.
        /// </summary>
        /// <param name="myAttribute">The interesting attribute.</param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        /// <param name="myType">The type of the interesing attribute.</param>
        /// <returns></returns>
        protected IEnumerable<Tuple<GraphDBType, AttributeIndex>> GetIndex(TypeAttribute myAttribute, DBContext dbContext, GraphDBType myType, Object myExtraordinary)
        {

            #region INPUT EXCEPTIONS

            if (myType == null || myAttribute == null || dbContext == null)
            {
                throw new ArgumentNullException();
            }

            #endregion


            if (myExtraordinary == null)
            {
                #region eventual speedup

                //direct match of idx
                if (myType.HasAttributeIndices(myAttribute.UUID))
                {
                    yield return new Tuple<GraphDBType, AttributeIndex>(myType, myType.GetAttributeIndex(myAttribute.UUID, null).Value);
                }
                else
                {
                    //no direct match... lets try the subtypes
                    foreach (var aSubType in dbContext.DBTypeManager.GetAllSubtypes(myType, true))
                    {
                        if (aSubType.HasAttributeIndices(myAttribute.UUID))
                        {
                            yield return new Tuple<GraphDBType, AttributeIndex>(aSubType, aSubType.GetAttributeIndex(myAttribute.UUID, null).Value);
                        }
                        else
                        {
                            yield return new Tuple<GraphDBType, AttributeIndex>(aSubType, aSubType.GetUUIDIndex(dbContext.DBTypeManager));
                        }
                    }
                }

                #endregion
            }
            else
            {
                foreach (var aSubType in dbContext.DBTypeManager.GetAllSubtypes(myType, true))
                {
                    yield return new Tuple<GraphDBType, AttributeIndex>(aSubType, aSubType.GetUUIDIndex(dbContext.DBTypeManager));
                }
            }

            yield break;
        }

        #region Get tuple based on the operator (InRange allows other tuples than + or = ...)

        public virtual object GetValidTupleReloaded(TupleNode aTupleNode, DBContext dbContext)
        {
            switch (aTupleNode.KindOfTuple)
            {
                case KindOfTuple.Exclusive:

                    if (aTupleNode.Tuple.Count == 1)
                    {
                        return new AtomValue(PandoraTypeMapper.GetBaseObjectFromCSharpType(aTupleNode.Tuple[0].Value));
                    }
                    else
                    {
                        return CreateTupleValue(aTupleNode);
                    }

                default:

                    return CreateTupleValue(aTupleNode);

            }
        }

        protected object CreateTupleValue(TupleNode aTupleNode)
        {
            TupleValue result = new TupleValue(aTupleNode.KindOfTuple);

            foreach (var item in aTupleNode.Tuple)
            {
                result.Add(PandoraTypeMapper.GetPandoraObjectFromType(item.TypeOfValue, item.Value));
            }

            return result;
        }

        #region GetValidTuple(ParseTreeNode aTreeNode, DBContext dbContext)

        public virtual object GetValidTuple(ParseTreeNode aTreeNode, DBContext dbContext)
        {
            if (aTreeNode.ChildNodes.Count == 3)
            {
                return GetValidTuple(aTreeNode.ChildNodes[1], dbContext);
            }
            else
            {
                if (aTreeNode.AstNode is BinaryExpressionNode)
                {
                    return aTreeNode.AstNode;
                }
                else if (aTreeNode.AstNode is TupleNode && (aTreeNode.AstNode as TupleNode).Tuple.Count == 3)
                {
                    return (aTreeNode.AstNode as TupleNode).Tuple[0].Value;
                }
                else if (aTreeNode.AstNode == null)
                {
                    if (aTreeNode.Token == null)
                    {
                        throw new GraphDBException(new Error_InvalidTuple(aTreeNode.ToStringChildnodesIncluded()));
                    }
                    else
                    {
                        return GetAtomValue(aTreeNode.Token);
                    }
                }
                else if (aTreeNode.AstNode is UnaryExpressionNode)
                {
                    return GetBinaryExpressionNode((UnaryExpressionNode)aTreeNode.AstNode, dbContext);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        /// <summary>
        /// Creates a atom value from a token.
        /// </summary>
        /// <param name="token">The token that will be evaluated.</param>
        /// <returns>An AtomValue</returns>
        public virtual AtomValue GetAtomValue(Token token)
        {
            #region Data

            AtomValue result = null;

            #endregion

            result = new AtomValue(PandoraTypeMapper.ConvertPandora2CSharp(token.Terminal.GetType().Name), token.Value);

            return result;
        }

        public virtual BinaryExpressionNode GetBinaryExpressionNode(UnaryExpressionNode aTreeNode, DBContext dbContext)
        {
            BinaryExpressionNode retVal = new BinaryExpressionNode();

            try
            {
                retVal.Left = aTreeNode.Term;

                if (dbContext.DBPluginManager.GetBinaryOperator(aTreeNode.OperatorSymbol).Label == BinaryOperator.Subtraction)
                    retVal.Right = new AtomValue(new DBInt32(-1));
                else
                    retVal.Right = new AtomValue(new DBInt32(1));

                retVal.TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
                retVal.Operator = dbContext.DBPluginManager.GetBinaryOperator(BinaryOperator.Multiplication);
            }
            catch (GraphDBException e)
            {
                throw e;
            }

            return retVal;
        }
        
        #endregion

    }
}
