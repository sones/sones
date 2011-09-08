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
    /// The vertex attribute already exists in the type
    /// </summary>
    public sealed class VertexAttributeCastException : AGraphDBVertexAttributeException
    {
        public String AttributeName { get; private set; }
        public String TypeName { get; private set; }

        /// <summary>
        /// Creates a new VertexAttributeAlreadyExistsException exception
        /// </summary>
        /// <param name="myAttributeName">The attribute name</param>
        public VertexAttributeCastException(String myAttributeName, Type fromtype, Type totype, bool bFormat)
        {
            AttributeName = myAttributeName;
            if (bFormat)
            {
                _msg = String.Format("The attribute \"{0}\" cannot be casted from {1} to {2} due to wrong format of string!", AttributeName, fromtype, totype);
            }
            else
            {
                _msg = String.Format("The attribute \"{0}\" cannot be casted from {1} to {2}!", AttributeName, fromtype, totype);
            }
        }
    }
}