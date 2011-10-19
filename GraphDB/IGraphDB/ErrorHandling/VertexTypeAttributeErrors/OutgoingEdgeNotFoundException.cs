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

using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown, if an incoming edge is set to an nonexisting outgoing edge.
    /// </summary>
    public class OutgoingEdgeNotFoundException: AGraphDBVertexAttributeException
    {
        /// <summary>
        /// The predefinition, that contains the incoming edge.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The incoming edge that causes the exception.
        /// </summary>
        public IncomingEdgePredefinition IncomingEdge { get; private set; }

        /// <summary>
        /// Creates an instance of OutgoingEdgeNotFoundException.
        /// </summary>
        /// <param name="myPredefinition">
        /// The predefinition, that contains the incoming edge.
        /// </param>
        /// <param name="myIncomingEdge">
        /// The incoming edge that causes the exception.
        /// </param>
        public OutgoingEdgeNotFoundException(VertexTypePredefinition myPredefinition, IncomingEdgePredefinition myIncomingEdge, Exception innerException = null) : base(innerException)
        {
            Predefinition = myPredefinition;
            IncomingEdge = myIncomingEdge;
            _msg = string.Format("Vertextype {0} defines an incoming edge on a nonexisting outgoing edge ({1}).", myPredefinition.TypeName, myIncomingEdge.AttributeType);
        }
    }
}
