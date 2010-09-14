/* <id name="GraphDB - greater or equals than operator" />
 * <copyright file="GreaterThanOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements a greater than operator.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Exceptions;

using sones.GraphDB.Structures.Enums;


using sones.Lib.ErrorHandling;


using sones.GraphDB.TypeManagement.BasicTypes;


#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a greater than operator.
    /// </summary>
    public class GreaterEqualsOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { ">=", "!<" }; } }
        public override String              ContraryOperationSymbol { get { return "<"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.GreaterEquals; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public GreaterEqualsOperator()
        {

        }

        #endregion

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) >= 0);
        }

    }
}
