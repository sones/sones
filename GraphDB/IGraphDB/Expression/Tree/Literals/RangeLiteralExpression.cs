/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Expression.Tree.Literals;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// A constant expression for range-definition
    /// </summary>
    public sealed class RangeLiteralExpression : IExpression
    {
        #region data

        /// <summary>
        /// A constant expression... sth like 13 or "Alice"
        /// The lower limit for the range
        /// </summary>
        public readonly IComparable Lower;

        /// <summary>
        /// A constant expression... sth like 13 or "Alice"
        /// The upper limit for the range
        /// </summary>
        public readonly IComparable Upper;

        /// <summary>
        /// Include the upper and lower boarder?
        /// </summary>
        public readonly Boolean IncludeBorders;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new constant range expression
        /// </summary>
        /// <param name="myLower">The lower limit for the range</param>
        /// <param name="myUpper">The upper limit for the range</param>
        /// <param name="myIncludeBorder">Include the upper and lower boarder?</param>
        public RangeLiteralExpression(IComparable myLower, IComparable myUpper, Boolean myIncludeBorder = true)
        {
            Lower = myLower;
            Upper = myUpper;
            IncludeBorders = myIncludeBorder;
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Constant; }
        }

        #endregion
    }
}
