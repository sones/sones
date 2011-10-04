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
using sones.GraphDB.Expression;

namespace sones.GraphDB.ErrorHandling.Expression
{
    /// <summary>
    /// An invalid expression occured
    /// </summary>
    public sealed class InvalidExpressionException : AGraphDBException
    {
        #region data

        /// <summary>
        /// The expression that has been declared as invalid
        /// </summary>
        public readonly IExpression InvalidExpression;

        String VertexTypeName;
        long VertexID;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new invalid expression exception
        /// </summary>
        /// <param name="myInvalidExpression">The expression that has been declared as invalid</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidExpressionException(IExpression myInvalidExpression, Exception innerException = null) : base(innerException)
        {
            InvalidExpression = myInvalidExpression;
        }

		/// <summary>
		/// Initializes a new instance of the InvalidExpressionException class using the specified vertex type name and vertex ID.
		/// </summary>
		/// <param name="myVertexTypeName"></param>
		/// <param name="myVertexID"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidExpressionException(String myVertexTypeName, long myVertexID, Exception innerException = null) : base(innerException)
        {
            VertexTypeName = myVertexTypeName;
            VertexID = myVertexID;
            InvalidExpression = null;
        }

        #endregion

        public override string ToString()
        {
            if(InvalidExpression != null)
                return String.Format("The expression {0} is invalid.", InvalidExpression);
            else
                return String.Format("The expression or VertexTypeName and VertexID must be set");
        }
    }
}
