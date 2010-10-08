/* <id name="GraphDB - not equals operator" />
 * <copyright file="NotEqualsOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a not equals operator.</summary>
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
    /// This class implements a not equals operator.
    /// </summary>
    public class NotEqualsOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "!=", "<>" }; } }
        public override String              ContraryOperationSymbol { get { return "="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.NotEqual; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLowerLevels; } }

        #endregion

        #region Constructor

        public NotEqualsOperator()
        {

        }

        #endregion

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) != 0);
        }

    }
}
