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
using System.Linq;
using System.Text;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The given kind of attribute does not match
    /// </summary>
    public sealed class InvalidVertexAttributeKindException : AGraphQLVertexAttributeException
    {
        #region data

        public String[] ExpectedKindsOfType { get; private set; }
        public String CurrentKindsOfType { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new InvalidVertexAttributeKindException exception
        /// </summary>
        public InvalidVertexAttributeKindException()
        {
            ExpectedKindsOfType = new String[0];
            _msg = "The given kind does not match the expected";
        }

        /// <summary>
        /// Creates a new InvalidVertexAttributeKindException exception
        /// </summary>
        /// <param name="myCurrentKindsOfType">The current kind of type</param>
        /// <param name="myExpectedKindsOfType">A List of expected kind of types</param>
        public InvalidVertexAttributeKindException(String myCurrentKindsOfType, params String[] myExpectedKindsOfType)
            : this()
        {
            ExpectedKindsOfType = myExpectedKindsOfType;
            CurrentKindsOfType = myCurrentKindsOfType;

            _msg = String.Format("The given kind \"{0}\" does not match the expected \"{1}\"", CurrentKindsOfType,
                ExpectedKindsOfType.Aggregate<String, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }

        #endregion

    }
}
