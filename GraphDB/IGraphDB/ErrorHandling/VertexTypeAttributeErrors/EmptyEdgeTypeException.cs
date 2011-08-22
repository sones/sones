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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type with an outgoing IncomingEdge with an empty IncomingEdge type should be added.
    /// </summary>
    public sealed class EmptyEdgeTypeException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// Creates an instance of EmptyEdgeTypeException.
        /// </summary>
        /// <param name="myPredefinition">The predefinition that causes the exception.</param>
        public EmptyEdgeTypeException(ATypePredefinition myPredefinition, String myOutgoingEdgeName)
        {
            Predefinition = myPredefinition;
            PropertyName = myOutgoingEdgeName;
            _msg = string.Format("The outgoing edge {0} on vertex type {1} is empty.", myOutgoingEdgeName, myPredefinition.TypeName);
        }

        /// <summary>
        /// The predefinition that causes the exception.
        /// </summary>
        public ATypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The outgoing IncomingEdge that causes the exception.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
