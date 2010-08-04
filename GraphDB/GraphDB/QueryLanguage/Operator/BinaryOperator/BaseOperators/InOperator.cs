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
    class InOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "IN" }; } }
        public override String              ContraryOperationSymbol { get { return "NOTIN"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.In; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public InOperator()
        {

        }

        #endregion

        #region SimpleOperation Methods


        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (left is ValueDefinition)
            {
                if (right is ValueDefinition)
                {
                    return SimpleOperation((ValueDefinition)left, (ValueDefinition)right, myTypeOfBinaryExpression);
                }
                else
                {
                    return SimpleOperation((ValueDefinition)left, (TupleDefinition)right, myTypeOfBinaryExpression);
                }
            }
            else
            {
                if (right is ValueDefinition)
                {
                    return SimpleOperation((TupleDefinition)left, (ValueDefinition)right, myTypeOfBinaryExpression);
                }
                else
                {
                    return SimpleOperation((TupleDefinition)left, (TupleDefinition)right, myTypeOfBinaryExpression);
                }
            }
        }

        #endregion

        protected Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, ValueDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            resultValue = Compare(left.Value, right.Value);

            resultObject = new ValueDefinition(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        public override AOperationDefinition GetValidTupleReloaded(TupleDefinition aTupleNode, DBContext dbContext)
        {
            return CreateTupleValue(aTupleNode);
        }

        protected Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, TupleDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {

            #region Data

            ValueDefinition resultObject = null;
            Object resultValue = false;

            #endregion

            resultValue = right.Contains(new TupleElement(left));

            resultObject = new ValueDefinition(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<AOperationDefinition>(resultObject);

        }

        protected Exceptional<AOperationDefinition> SimpleOperation(TupleDefinition left, TupleDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            Object resultValue = true;
            HashSet<ADBBaseObject> anotherValues = new HashSet<ADBBaseObject>();


            switch (myTypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    #region left complex

                    //foreach (var aRightValue in right)
                    //{
                    //    anotherValues.Add(GraphDBTypeMapper.GetPandoraObjectFromType(left.TypeOfOperatorResult, (aRightValue.Value as ValueDefinition).Value.Value));
                    //}

                    foreach (var aLeft in left)
                    {
                        if (!right.Contains(aLeft))
                        {
                            resultValue = false;
                            break;
                        }
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    #region right complex

                    //foreach (var aLeftValue in left.TupleValue.GetAllValues())
                    //{
                    //    anotherValues.Add(GraphDBTypeMapper.GetPandoraObjectFromType(right.TupleValue.TypeOfValue, aLeftValue));
                    //}

                    foreach (var aAnother in left)
                    {
                        if (!right.Contains(aAnother))
                        {
                            resultValue = false;
                            break;
                        }
                    }

                    #endregion

                    break;
                default:
                    break;
            }

            return new Exceptional<AOperationDefinition>(new ValueDefinition(TypesOfOperatorResult.Boolean, (object)resultValue));
        }

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref myLeft, ref myRight).Value)
            {
                return new Exceptional<Boolean>(new Error_DataTypeDoesNotMatch(myLeft.Type.ToString(), myRight.Type.ToString()));
            }
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) == 0);
        }


        public override IEnumerable<ObjectUUID> IndexSingleOperation(AAttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            IndexKey lookup = new IndexKey(myAttributeUUID, myOperationValue, myIndex.IndexKeyDefinition);
            var currentType = dbContext.DBTypeManager.GetTypeByUUID(myIndex.IndexRelatedTypeUUID);

            if (myIndex.Contains(lookup, currentType, dbContext))
            {
                foreach (var aUUID in myIndex.GetValues(lookup, currentType, dbContext))
                {
                    yield return aUUID;
                }
            }

            yield break;
        }

        public override IEnumerable<ObjectUUID> IndexOperation(AAttributeIndex myIndex, TupleDefinition myTuple, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            HashSet<ObjectUUID> interestingUUIDs = new HashSet<ObjectUUID>();
            IndexKey idxLookupKey = null;
            var currentType = dbContext.DBTypeManager.GetTypeByUUID(myIndex.IndexRelatedTypeUUID);

            var myOperationValues = myTuple;

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    if (!myIndex.IsListOfBaseObjectsIndex)
                    {
                        foreach (var aItem in myOperationValues)
                        {
                            idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], (aItem.Value as ValueDefinition).Value, myIndex.IndexKeyDefinition);
                            interestingUUIDs.UnionWith(myIndex.GetValues(idxLookupKey, currentType, dbContext));
                        }
                    }
                    else
                    {
                        #region In case the index is from a set or list of baseobjects we use this way to get the values

                        foreach (var aItem in myOperationValues)
                        {
                            idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], (aItem.Value as ValueDefinition).Value, myIndex.IndexKeyDefinition);
                            interestingUUIDs.UnionWith(myIndex.GetValues(idxLookupKey, currentType, dbContext));
                        }

                        /* What the hack is that??? - This is too slow for any usual usage of in operator! */

                        var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(myIndex.IndexRelatedTypeUUID);

                        foreach (var aKey in myIndex.GetKeys(currentType, dbContext).Where(item => !myOperationValues.Contains(new TupleElement(new ValueDefinition(item.IndexKeyValues[0])))))
                        {
                            foreach (var aMatch in myIndex.GetValues(aKey, indexRelatedType, dbContext).Intersect(interestingUUIDs))
                            {
                                interestingUUIDs.Remove(aMatch);
                            }
                        }

                        #endregion
                    }

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    var myEnumerator = myOperationValues.GetEnumerator();

                    if (myEnumerator.MoveNext())
                    {
                        idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], (myEnumerator.Current.Value as ValueDefinition).Value, myIndex.IndexKeyDefinition);

                        interestingUUIDs.UnionWith(myIndex.GetValues(idxLookupKey, currentType, dbContext));

                        while (myEnumerator.MoveNext())
                        {
                            idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], (myEnumerator.Current.Value as ValueDefinition).Value, myIndex.IndexKeyDefinition);
                            interestingUUIDs.IntersectWith(myIndex.GetValues(idxLookupKey, currentType, dbContext));
                        }
                    }

                    break;

                case TypesOfBinaryExpression.Atom:
                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Unknown:
                default:
                    break;
            }



            foreach (var aValidUUID in interestingUUIDs)
            {
                yield return aValidUUID;
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

                    if ((data.Operands.Item1 is ValueDefinition) || data.Operands.Item1 is TupleDefinition)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case TypesOfBinaryExpression.Complex:
                case TypesOfBinaryExpression.Atom:

                    return true;

                case TypesOfBinaryExpression.Unknown:                
                default:

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }
    }
}
