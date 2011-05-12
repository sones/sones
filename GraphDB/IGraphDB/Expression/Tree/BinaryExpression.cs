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

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// A binary expression consists of two Expressions and a binary operator
    /// </summary>
    public sealed class BinaryExpression : IExpression
    {
        #region data

        /// <summary>
        /// The left side of the BinaryExpression
        /// </summary>
        public readonly IExpression Left;

        /// <summary>
        /// The binary operator
        /// </summary>
        public readonly BinaryOperator Operator;

        /// <summary>
        /// The right side of the BinaryExpression
        /// </summary>
        public readonly IExpression Right;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new binary expression
        /// </summary>
        /// <param name="myLeftExpression">The left side of the BinaryExpression</param>
        /// <param name="myBinaryOperator">The binary operator</param>
        /// <param name="myRightExpression">The right side of the BinaryExpression</param>
        public BinaryExpression (IExpression myLeftExpression, BinaryOperator myBinaryOperator, IExpression myRightExpression)
        {
            Left = myLeftExpression;
            Right = myRightExpression;
            Operator = myBinaryOperator;
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Binary; }
        }

        #endregion
    }
}
