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

/* <id name="PandoraDB - in operator" />
 * <copyright file="InOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This class implements an in operator.
    /// </summary>
    class NotInOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "NOTIN", "NIN", "NOT_IN", "!IN" }; } }
        public override String              ContraryOperationSymbol { get { return "IN"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.NotIn; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLowerLevels; } }


        #endregion

        #region Constructor

        public NotInOperator()
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

        public override object GetValidTupleReloaded(TupleNode aTupleNode, DBContext dbContext)
        {
            return CreateTupleValue(aTupleNode);
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


        public override IEnumerable<ObjectUUID> IndexSingleOperation(AttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager indexManager)
        {
            var idxRef = myIndex.GetIndexReference(indexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(idxRef.Errors);
            }
            var idxRefVal = idxRef.Value;
            
            IndexKey lookup = new IndexKey(myAttributeUUID, myOperationValue, myIndex.IndexKeyDefinition);

            if (idxRefVal.ContainsKey(lookup))
            {
                var interestingUUIDs = idxRefVal[lookup];

                foreach (var aIndexValue in idxRefVal.GetIDictionary().Select(kv => kv.Value))
                {
                    foreach (var aUUID in aIndexValue)
                    {
                        if (!interestingUUIDs.Contains(aUUID))
                        {
                            yield return aUUID;
                        }
                    }
                }
            }
            else
            {
                foreach (var aIndexValue in idxRefVal.GetIDictionary().Select(kv => kv.Value))
                {
                    foreach (var aUUID in aIndexValue)
                    {
                        yield return aUUID;
                    }
                }
            }

            yield break;
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
                    else
                    {
                        return false;
                    }

                case TypesOfBinaryExpression.RightComplex:

                    if (data.Operands.Item1 is AtomValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case TypesOfBinaryExpression.Atom:
                case TypesOfBinaryExpression.Complex:

                    return true;

                case TypesOfBinaryExpression.Unknown:
                default:

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }
    }
}
