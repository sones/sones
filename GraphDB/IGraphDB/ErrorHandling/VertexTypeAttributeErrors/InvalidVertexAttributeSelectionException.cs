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
    /// The selected vertex attribute is not valid
    /// </summary>
    public sealed class InvalidVertexAttributeSelectionException : AGraphDBVertexAttributeException
    {
        public String SelectedVertexAttribute { get; private set; }

        /// <summary>
        /// Creates a new InvalidVertexAttributeSelectionException exception
        /// </summary>
        /// <param name="mySelectedVertexAttribute">The current selected vertex attribute</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidVertexAttributeSelectionException(String mySelectedVertexAttribute, Exception innerException = null) : base(innerException)
        {
            SelectedVertexAttribute = mySelectedVertexAttribute;
            _msg = String.Format("The selected vertex attribute \"{0}\" is not valid!", SelectedVertexAttribute);
        }

    }
}
