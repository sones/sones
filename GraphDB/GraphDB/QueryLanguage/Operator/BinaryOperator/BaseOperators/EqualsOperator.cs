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

/* <id name="PandoraDB - equality operator" />
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
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This class implements a equality operator.
    /// </summary>
    class EqualsOperator : ABinaryCompareOperator
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

        public override IEnumerable<ObjectUUID> IndexSingleOperation(AttributeIndex myIndex, ADBBaseObject myOperationValue, AttributeUUID myAttributeUUID, TypesOfBinaryExpression typeOfBinExpr, DBIndexManager dbIndexManager)
        {
            var myIndeyKey = new IndexKey(myAttributeUUID, myOperationValue, myIndex.IndexKeyDefinition);
            var idxRef = myIndex.GetIndexReference(dbIndexManager);

            if (idxRef.Value.ContainsKey(myIndeyKey))
            {
                foreach (var aUUID in idxRef.Value[myIndeyKey])
                {
                    yield return aUUID;
                }
            }

            yield break;
        }

    }
}
