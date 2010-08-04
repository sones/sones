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
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Managers.Structures;

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


        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (left is ValueDefinition && right is ValueDefinition)
                return SimpleOperation((ValueDefinition)left, (ValueDefinition)right);
            else if (left is ValueDefinition && right is TupleDefinition)
                return SimpleOperation((ValueDefinition)left, (TupleDefinition)right);
            // [1,3,5] IN 5 <-- makes no sence
            //else if (left is TupleValue && right is AtomValue)
            //    return SimpleOperation((TupleValue)left, (AtomValue)right);
            else if (left is TupleDefinition && right is TupleDefinition)
                return SimpleOperation((TupleDefinition)left, (TupleDefinition)right);

            return new Exceptional<AOperationDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion

        protected new Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, ValueDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;

            #endregion

            var resultValue = Compare(left.Value, right.Value);
            if (resultValue.Failed)
                return new Exceptional<AOperationDefinition>(resultValue);


            resultObject = new ValueDefinition(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        public override AOperationDefinition GetValidTupleReloaded(TupleDefinition aTupleNode, DBContext dbContext)
        {
            return CreateTupleValue(aTupleNode);
        }

        protected new Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, TupleDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            foreach (var val in right)
            {
                var comp = Compare(left.Value, (val.Value as ValueDefinition).Value);
                if (comp.Failed)
                    return new Exceptional<AOperationDefinition>(comp);

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

            resultObject = new ValueDefinition(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected new Exceptional<AOperationDefinition> SimpleOperation(TupleDefinition left, TupleDefinition right)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            foreach (var leftVal in left)
            {
                foreach (var rightVal in right)
                {
                    var comp = Compare((leftVal.Value as ValueDefinition).Value, (rightVal.Value as ValueDefinition).Value);
                    if (comp.Failed)
                        return new Exceptional<AOperationDefinition>(comp);

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

            resultObject = new ValueDefinition(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref myLeft, ref myRight).Value)
            {
                return new Exceptional<bool>(new Error_DataTypeDoesNotMatch(myLeft.Type.ToString(), myRight.Type.ToString()));
            }
            return new Exceptional<bool>(myLeft.CompareTo(myRight) != 0);
        }


        public override IEnumerable<ObjectUUID> IndexSingleOperation(AAttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            IndexKey lookup = new IndexKey(myAttributeUUID, myOperationValue, myIndex.IndexKeyDefinition);
            var currentType = dbContext.DBTypeManager.GetTypeByUUID(myIndex.IndexRelatedTypeUUID);

            if (myIndex.Contains(lookup, currentType, dbContext))
            {
                var interestingUUIDs = myIndex.GetValues(lookup, currentType, dbContext);

                foreach (var aIndexValue in myIndex.GetAllValues(currentType, dbContext))
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
                foreach (var aIndexValue in myIndex.GetKeyValues(currentType, dbContext).Select(kv => kv.Value))
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

                    if (data.Operands.Item1 is TupleDefinition)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case TypesOfBinaryExpression.RightComplex:

                    if (data.Operands.Item1 is ValueDefinition)
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
