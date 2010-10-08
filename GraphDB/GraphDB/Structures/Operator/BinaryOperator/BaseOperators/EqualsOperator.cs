/* <id name="GraphDB - equality operator" />
 * <copyright file="EqualityOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a equality operator.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a equality operator.
    /// </summary>
    public class EqualsOperator : ABinaryCompareOperator
    {

        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "=" }; } }
        public override String              ContraryOperationSymbol { get { return "!="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.Equal; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public EqualsOperator()
        {

        }

        #endregion

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) == 0);
        }

        public override IEnumerable<ObjectUUID> IndexSingleOperation(AAttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBContext dbContext)
        {
            var myIndeyKey = new IndexKey(myAttributeUUID, myOperationValue, myIndex.IndexKeyDefinition);
            var currentType = dbContext.DBTypeManager.GetTypeByUUID(myIndex.IndexRelatedTypeUUID);

            if (myIndex.Contains(myIndeyKey, currentType, dbContext))
            {
                foreach (var aUUID in myIndex.GetValues(myIndeyKey, currentType, dbContext))
                {
                    yield return aUUID;
                }
            }

            yield break;
        }

    }
}
