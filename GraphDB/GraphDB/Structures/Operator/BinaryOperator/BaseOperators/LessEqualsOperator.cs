/* <id name="GraphDB - less or equals than operator" />
 * <copyright file="LessThanOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements a less than operator.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;

using sones.GraphDB.Structures.Enums;


using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;


#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a less than operator.
    /// </summary>
    public class LessEqualsOperator : ABinaryCompareOperator
    {

        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "<=", "!>" }; } }
        public override String              ContraryOperationSymbol { get { return ">"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.LessEquals; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public LessEqualsOperator()
        {

        }

        #endregion

        protected override Exceptional<bool> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<bool>(myLeft.CompareTo(myRight) <= 0);
        }

    }
}
