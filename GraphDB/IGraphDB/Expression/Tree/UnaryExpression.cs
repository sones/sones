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
    /// An unary expression has a unary operator and a single expression
    /// </summary>
    public sealed class UnaryExpression : IExpression
    {
        #region data

        /// <summary>
        /// The unary operator
        /// </summary>
        public readonly UnaryLogicOperator Operator;

        /// <summary>
        /// The single expression
        /// </summary>
        public readonly IExpression Expression;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new unary expression
        /// </summary>
        /// <param name="myUnaryOperator">The unary operator</param>
        /// <param name="myUnaryExpression">The expression</param>
        public UnaryExpression(UnaryLogicOperator myUnaryOperator, IExpression myUnaryExpression)
        {
            Operator = myUnaryOperator;
            Expression = myUnaryExpression;
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Unary; }
        }

        #endregion
    }
}
