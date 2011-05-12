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

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The vertex attribute already exists in subtype
    /// </summary>
    public sealed class VertexAttributeExistsInSubtypeException : AGraphDBVertexAttributeException
    {
        public String AttributeName { get; private set; }
        public String SubtypeName { get; private set; }

        /// <summary>
        /// Creates a new VertexAttributeExistsInSubtypeException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        public VertexAttributeExistsInSubtypeException(String myAttributeName)
        {
            AttributeName = myAttributeName;
            _msg = String.Format("The attribute \"{0}\" already exists in subtype !", AttributeName);
        }

        /// <summary>
        /// Creates a new VertexAttributeExistsInSubtypeException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        /// <param name="mySubtypeName">The name of the subtype </param>
        public VertexAttributeExistsInSubtypeException(String myAttributeName, String mySubtypeName)
        {
            AttributeName = myAttributeName;
            SubtypeName = mySubtypeName;
            _msg = String.Format("The attribute \"{0}\" already exists in subtype \"{1}\"!", AttributeName, SubtypeName);
        }

    }
}
