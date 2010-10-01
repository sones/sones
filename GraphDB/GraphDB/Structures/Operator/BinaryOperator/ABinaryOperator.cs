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

/* <id name="GraphDB - abstract binary operatror class" />
 * <copyright file="ABinaryOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <develoepr>Daniel Kirstenpfad</deveoper>
 * <summary>This abstract class is responsible for operators
 * which are triggered by binary expression nodes.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;


using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Managers.Structures;


#endregion

namespace sones.GraphDB.Structures.Operators
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

        public abstract Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression typeOfBinExpr);

        public abstract Exceptional<IExpressionGraph> TypeOperation(AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, Boolean aggregateAllowed = true);

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
        protected IEnumerable<Tuple<GraphDBType, AAttributeIndex>> GetIndex(TypeAttribute myAttribute, DBContext dbContext, GraphDBType myType, Object myExtraordinary)
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
                    yield return new Tuple<GraphDBType, AAttributeIndex>(myType, myType.GetAttributeIndex(myAttribute.UUID, null).Value);
                }
                else
                {
                    //no direct match... lets try the subtypes
                    foreach (var aSubType in dbContext.DBTypeManager.GetAllSubtypes(myType, true))
                    {
                        if (aSubType.HasAttributeIndices(myAttribute.UUID))
                        {
                            yield return new Tuple<GraphDBType, AAttributeIndex>(aSubType, aSubType.GetAttributeIndex(myAttribute.UUID, null).Value);
                        }
                        else
                        {
                            yield return new Tuple<GraphDBType, AAttributeIndex>(aSubType, aSubType.GetUUIDIndex(dbContext.DBTypeManager));
                        }
                    }
                }

                #endregion
            }
            else
            {
                foreach (var aSubType in dbContext.DBTypeManager.GetAllSubtypes(myType, true))
                {
                    yield return new Tuple<GraphDBType, AAttributeIndex>(aSubType, aSubType.GetUUIDIndex(dbContext.DBTypeManager));
                }
            }

            yield break;
        }

        #region Get tuple based on the operator (InRange allows other tuples than + or = ...)

        public virtual AOperationDefinition GetValidTupleReloaded(TupleDefinition myTupleDefinition, DBContext dbContext)
        {
            switch (myTupleDefinition.KindOfTuple)
            {
                case KindOfTuple.Exclusive:

                    if (myTupleDefinition.TupleElements.Count == 1)
                    {
                        return myTupleDefinition.TupleElements[0].Value as ValueDefinition;
                    }
                    else
                    {
                        return myTupleDefinition;
                    }

                default:

                    return myTupleDefinition;

            }
        }

        protected AOperationDefinition CreateTupleValue(TupleDefinition myTupleDefinition)
        {
            //var result = new TupleDefinition(myTupleDefinition.KindOfTuple);

            //foreach (var item in myTupleDefinition)
            //{
            //    result.AddElement(new TupleElement((item.Value as ValueDefinition)));
            //}

            //return result;
            return myTupleDefinition;
        }

        //#region GetValidTuple(ParseTreeNode aTreeNode, DBContext dbContext)

        //public virtual object GetValidTuple(ParseTreeNode aTreeNode, DBContext dbContext)
        //{
        //    if (aTreeNode.ChildNodes.Count == 3)
        //    {
        //        return GetValidTuple(aTreeNode.ChildNodes[1], dbContext);
        //    }
        //    else
        //    {
        //        if (aTreeNode.AstNode is BinaryExpressionNode)
        //        {
        //            return aTreeNode.AstNode;
        //        }
        //        else if (aTreeNode.AstNode is TupleNode && (aTreeNode.AstNode as TupleNode).TupleDefinition.Count() == 3)
        //        {
        //            return (aTreeNode.AstNode as TupleNode).TupleDefinition.First().Value;
        //        }
        //        else if (aTreeNode.AstNode == null)
        //        {
        //            if (aTreeNode.Token == null)
        //            {
        //                throw new GraphDBException(new Error_InvalidTuple(aTreeNode.ToStringChildnodesIncluded()));
        //            }
        //            else
        //            {
        //                return GetValueDefinition(aTreeNode.Token);
        //            }
        //        }
        //        else if (aTreeNode.AstNode is UnaryExpressionNode)
        //        {
        //            return GetBinaryExpressionNode((UnaryExpressionNode)aTreeNode.AstNode, dbContext);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        //#endregion

        ///// <summary>
        ///// Creates a atom value from a token.
        ///// </summary>
        ///// <param name="token">The token that will be evaluated.</param>
        ///// <returns>An AtomValue</returns>
        //public virtual ValueDefinition GetValueDefinition(Token token)
        //{
        //    return new ValueDefinition(GraphDBTypeMapper.ConvertGraph2CSharp(token.Terminal.GetType().Name), token.Value);
        //}

        //public virtual BinaryExpressionNode GetBinaryExpressionNode(UnaryExpressionNode aTreeNode, DBContext dbContext)
        //{
        //    BinaryExpressionNode retVal = new BinaryExpressionNode();

        //    try
        //    {
        //        //retVal.Left = aTreeNode.Term;

        //        //if (dbContext.DBPluginManager.GetBinaryOperator(aTreeNode.OperatorSymbol).Label == BinaryOperator.Subtraction)
        //        //    retVal.Right = new AtomValue(new DBInt32(-1));
        //        //else
        //        //    retVal.Right = new AtomValue(new DBInt32(1));

        //        //retVal.TypeOfBinaryExpression = TypesOfBinaryExpression.LeftComplex;
        //        //retVal.Operator = dbContext.DBPluginManager.GetBinaryOperator(BinaryOperator.Multiplication);
        //    }
        //    catch (GraphDBException e)
        //    {
        //        throw e;
        //    }

        //    return retVal;
        //}
        
        #endregion

    }
}
