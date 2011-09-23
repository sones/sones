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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type has a property without a type.
    /// </summary>
    public sealed class EmptyPropertyTypeException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// Creates an instance of EmptyEdgeTypeException.
        /// </summary>
        /// <param name="myPredefinition">The predefinition that causes the exception.</param>
        /// <param name="myPropertyName"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public EmptyPropertyTypeException(VertexTypePredefinition myPredefinition, String myPropertyName, Exception innerException = null)
			: base(innerException)
        {
            Predefinition = myPredefinition;
            PropertyName = myPropertyName;
            _msg = string.Format("The property type {0} on vertex type {1} is empty.", myPropertyName, myPredefinition.VertexTypeName);
        }

        /// <summary>
        /// The predefinition that causes the exception.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The property that causes the exception.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
