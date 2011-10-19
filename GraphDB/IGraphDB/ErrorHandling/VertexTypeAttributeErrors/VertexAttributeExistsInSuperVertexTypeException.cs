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
    /// The vertex attribute already exists in supertype
    /// </summary>
    public sealed class VertexAttributeExistsInSuperVertexTypeException : AGraphDBVertexAttributeException
    {
        #region data        

        public String VertexAttributeName { get; private set; }
        public String VertexSupertypeName { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new VertexAttributeExistsInSuperVertexTypeException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public VertexAttributeExistsInSuperVertexTypeException(String myVertexAttributeName, Exception innerException = null) : base(innerException)
        {
            VertexAttributeName = myVertexAttributeName;
        }

        /// <summary>
        /// Create a new VertexAttributeExistsInSuperVertexTypeException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        /// <param name="myVertexSupertypeName">The name of the vertex supertype</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public VertexAttributeExistsInSuperVertexTypeException(String myVertexAttributeName, String myVertexSupertypeName, Exception innerException = null) : base(innerException)
        {
            VertexAttributeName = myVertexAttributeName;
            VertexSupertypeName = myVertexSupertypeName;

            _msg = String.Format("The vertex attribute \"{0}\" already exists in supertype \"{1}\"!", VertexAttributeName, VertexSupertypeName);
        }

        #endregion

    }
}
