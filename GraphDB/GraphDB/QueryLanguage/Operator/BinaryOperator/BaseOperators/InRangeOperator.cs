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

/* <id name="sones GraphDB - in operator" />
 * <copyright file="InOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements an in operator.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Operator;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This class implements an in operator.
    /// </summary>
    class InRangeOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "INRANGE" }; } }
        public override String              ContraryOperationSymbol { get { return "OUTRANGE"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.InRange; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLowerLevels; } }


        #endregion

        #region Constructor

        public InRangeOperator()
        {

        }

        #endregion

        #region SimpleOperation Methods


        public override Exceptional<IOperationValue> SimpleOperation(IOperationValue left, IOperationValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (left is AtomValue && right is AtomValue)
                return SimpleOperation((AtomValue)left, (AtomValue)right);
            else if (left is AtomValue && right is TupleValue)
                return SimpleOperation((AtomValue)left, (TupleValue)right);
            // [1,3,5] IN 5 <-- makes no sence
            //else if (left is TupleValue && right is AtomValue)
            //    return SimpleOperation((TupleValue)left, (AtomValue)right);
            else if (left is TupleValue && right is TupleValue)
                return SimpleOperation((TupleValue)left, (TupleValue)right);

            return new Exceptional<IOperationValue>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion

        protected new Exceptional<IOperationValue> SimpleOperation(AtomValue left, AtomValue right)
        {

            #region Data

            AtomValue resultObject = null;

            #endregion

            var resultValue = Compare(left.Value, right.Value);
            if (resultValue.Failed)
                return new Exceptional<IOperationValue>(resultValue);


            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected new Exceptional<IOperationValue> SimpleOperation(AtomValue left, TupleValue right)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            foreach (ADBBaseObject val in right.Values)
            {
                var comp = Compare(left.Value, val);
                if (comp.Failed)
                    return new Exceptional<IOperationValue>(comp);

                if (!comp.Value)
                {
                    resultValue = false;
                    break;
                }
                else
                {
                    resultValue = true;
                }
            }

            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected new Exceptional<IOperationValue> SimpleOperation(TupleValue left, TupleValue right)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            foreach (ADBBaseObject leftVal in left.Values)
            {
                foreach (ADBBaseObject rightVal in right.Values)
                {
                    var comp = Compare(leftVal, rightVal);
                    if (comp.Failed)
                        return new Exceptional<IOperationValue>(comp);

                    if (!comp.Value)
                    {
                        resultValue = false;
                        break;
                    }
                    else
                    {
                        resultValue = true;
                    }
                }
                if ((Boolean)resultValue) break;
            }

            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref myLeft, ref myRight).Value)
            {
                return new Exceptional<bool>(new Error_DataTypeDoesNotMatch(myLeft.Type.ToString(), myRight.Type.ToString()));
            }
            return new Exceptional<bool>(myLeft.CompareTo(myRight) != 0);
        }

        public override object GetValidTupleReloaded(TupleNode aTupleNode, DBContext dbContext)
        {
            return CreateTupleValue(aTupleNode);
        }

        public override IEnumerable<ObjectUUID> IndexOperation(AttributeIndex myIndex, TupleValue myTuple, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager dbIndexManager)
        {
            var idxRef = myIndex.GetIndexReference(dbIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(idxRef.Errors);
            }

            if (myTuple.Values.Count != 2)
            {
                throw new GraphDBException(new Error_InvalidInRangeInterval(2, myTuple.Values.Count));
            }

            #region As soon as the index supports ranges use them!!

            //limits
            var fromKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], myTuple.Values[0], myIndex.IndexKeyDefinition);
            var toKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], myTuple.Values[1], myIndex.IndexKeyDefinition);

            switch (myTuple.KindOfTuple)
            {
                case KindOfTuple.Inclusive:
                    return idxRef.Value.InRange(fromKey, toKey, true, true);
                case KindOfTuple.LeftExclusive:
                    return idxRef.Value.InRange(fromKey, toKey, false, true);
                case KindOfTuple.RightExclusive:
                    return idxRef.Value.InRange(fromKey, toKey, true, false);
                case KindOfTuple.Exclusive:
                    return idxRef.Value.InRange(fromKey, toKey, false, false);
                default:
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }


            #endregion
        }

        public override bool IsValidIndexOperation(DataContainer data, DBContext myTypeManager, TypesOfBinaryExpression typeOfBinExpr)
        {
            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    if (data.Operands.Item1 is TupleValue)
                    {
                        return true;
                    }
                    else if (data.Operands.Item1 is AtomValue)
                    {
                        throw new GraphDBException(new Error_InvalidInRangeInterval(2, 1));
                    }
                    else
                    {
                        return false;
                    }

                case TypesOfBinaryExpression.RightComplex:
                case TypesOfBinaryExpression.Atom:
                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Unknown:
                default:

                    throw new GraphDBException(new Error_InvalidInRangeOperation("Only LeftComplex operation are allowed."));
            }
        }

        #region Get tuple based on the operator (InRange allows other tuples than + or = ...)

        public override object GetValidTuple(ParseTreeNode aTreeNode, DBContext dbContext)
        {
            if (aTreeNode.ChildNodes.Count == 1)
            {
                return GetValidTuple(aTreeNode.ChildNodes[0], dbContext);
            }
            else
            {
                if (aTreeNode.AstNode is TupleNode && (aTreeNode.AstNode as TupleNode).Tuple.Count == 2)
                {
                    return (aTreeNode.AstNode as TupleNode).Tuple[0].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

    }
}
