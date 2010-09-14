/* <id name="GraphDB - less than operator" />
 * <copyright file="LessThanOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a less than operator.</summary>
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
    /// This class implements a less than operator.
    /// </summary>
    public class LessThanOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "<" }; } }
        public override String              ContraryOperationSymbol { get { return ">="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.LessThan; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public LessThanOperator()
        {

        }

        #endregion

        protected override Exceptional<bool> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) < 0);
        }

    }
}
