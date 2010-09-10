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


        public override Exceptional<IOperationValue> SimpleOperation(IOperationValue left, IOperationValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (left is AtomValue)
            {
                if (right is AtomValue)
                {
                    return SimpleOperation((AtomValue)left, (AtomValue)right, myTypeOfBinaryExpression);
                }
                else
                {
                    return SimpleOperation((AtomValue)left, (TupleValue)right, myTypeOfBinaryExpression);
                }
            }
            else
            {
                if (right is AtomValue)
                {
                    return SimpleOperation((TupleValue)left, (AtomValue)right, myTypeOfBinaryExpression);
                }
                else
                {
                    return SimpleOperation((TupleValue)left, (TupleValue)right, myTypeOfBinaryExpression);
                }
            }
        }

        #endregion

        protected Exceptional<IOperationValue> SimpleOperation(AtomValue left, AtomValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            resultValue = Compare(left.Value, right.Value);

            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        public override object GetValidTupleReloaded(TupleNode aTupleNode, DBContext dbContext)
        {
            return CreateTupleValue(aTupleNode);
        }

        protected Exceptional<IOperationValue> SimpleOperation(AtomValue left, TupleValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {

            #region Data

            AtomValue resultObject = null;
            Object resultValue = false;

            #endregion

            foreach (ADBBaseObject val in right.Values)
            {
                if (Compare(left.Value, val).Value)
                {
                    resultValue = true;
                    break;
                }
                else
                {
                    resultValue = false;
                }
            }

            resultObject = new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue);

            return new Exceptional<IOperationValue>(resultObject);

        }

        protected Exceptional<IOperationValue> SimpleOperation(TupleValue left, TupleValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            Object resultValue = true;
            HashSet<ADBBaseObject> anotherValues = new HashSet<ADBBaseObject>();


            switch (myTypeOfBinaryExpression)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    #region left complex

                    foreach (var aRightValue in right.GetAllValues())
                    {
                        anotherValues.Add(GraphDBTypeMapper.GetPandoraObjectFromType(left.TypeOfValue, aRightValue));
                    }

                    foreach (var aLeft in left.GetAllValues())
                    {
                        if (!anotherValues.Contains(aLeft))
                        {
                            resultValue = false;
                            break;
                        }
                    }

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    #region right complex

                    foreach (var aLeftValue in left.GetAllValues())
                    {
                        anotherValues.Add(GraphDBTypeMapper.GetPandoraObjectFromType(right.TypeOfValue, aLeftValue));
                    }

                    foreach (var aAnother in anotherValues)
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

            return new Exceptional<IOperationValue>(new AtomValue(TypesOfOperatorResult.Boolean, (object)resultValue));
        }

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref myLeft, ref myRight).Value)
            {
                return new Exceptional<Boolean>(new Error_DataTypeDoesNotMatch(myLeft.Type.ToString(), myRight.Type.ToString()));
            }
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) == 0);
        }


        public override IEnumerable<ObjectUUID> IndexSingleOperation(AttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager indexManager)
        {

            var idxRef = myIndex.GetIndexReference(indexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(idxRef.Errors);
            }

            IndexKey lookup = new IndexKey(myAttributeUUID, myOperationValue, myIndex.IndexKeyDefinition);

            if (idxRef.Value.ContainsKey(lookup))
            {
                foreach (var aUUID in idxRef.Value[lookup])
                {
                    yield return aUUID;
                }
            }

            yield break;
        }

        public override IEnumerable<ObjectUUID> IndexOperation(AttributeIndex myIndex, TupleValue myTuple, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager indexManager)
        {

            var idxRef = myIndex.GetIndexReference(indexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(idxRef.Errors);
            }

            HashSet<ObjectUUID> interestingUUIDs = new HashSet<ObjectUUID>();
            IndexKey idxLookupKey;

            var myOperationValues = myTuple.GetAllValues();

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.LeftComplex:

                    if (!myIndex.IsListOfBaseObjectsIndex)
                    {
                        foreach (var aItem in myOperationValues)
                        {
                            idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], aItem, myIndex.IndexKeyDefinition);
                            interestingUUIDs.UnionWith(idxRef.Value[idxLookupKey]);
                        }
                    }
                    else
                    {
                        #region In case the index is from a set or list of baseobjects we use this way to get the values

                        foreach (var aItem in myOperationValues)
                        {
                            idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], aItem, myIndex.IndexKeyDefinition);
                            interestingUUIDs.UnionWith(idxRef.Value[idxLookupKey]);
                        }

                        /* What the heck is that??? - This is too slow for any usual usage of in operator! */
                        foreach (var aKey in idxRef.Value.Keys().Where(item => !myOperationValues.Contains(item.IndexKeyValues[0])))
                        {
                            foreach (var aMatch in idxRef.Value[aKey].Intersect(interestingUUIDs))
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
                        idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], myEnumerator.Current, myIndex.IndexKeyDefinition);

                        interestingUUIDs.UnionWith(idxRef.Value[idxLookupKey]);

                        while (myEnumerator.MoveNext())
                        {
                            idxLookupKey = new IndexKey(myIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs[0], myEnumerator.Current, myIndex.IndexKeyDefinition);
                            interestingUUIDs.IntersectWith(idxRef.Value[idxLookupKey]);
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

                    if (data.Operands.Item1 is TupleValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case TypesOfBinaryExpression.RightComplex:

                    if ((data.Operands.Item1 is AtomValue) || data.Operands.Item1 is TupleValue)
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
