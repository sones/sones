/* <id name="GraphDB - greater than operator" />
 * <copyright file="GreaterThanOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements a greater than operator.</summary>
 */

#region Usings

using System;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a greater than operator.
    /// </summary>
    public class GreaterThanOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { ">" }; } }
        public override String              ContraryOperationSymbol { get { return "<="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.GreaterThan; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public GreaterThanOperator()
        {

        }

        #endregion

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<bool>(myLeft.CompareTo(myRight) > 0);
        }

    }
}
