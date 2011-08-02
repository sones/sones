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

namespace sones.GraphDB.TypeSystem
{
    public class IncomingEdgePredefinition: AAttributePredefinition
    {
        public const char TypeSeparator = '.';

        /// <summary>
        /// Creates a definition for an incoming edge.
        /// </summary>
        /// <param name="myEdgeName">The name of the IncomingEdge</param>
        public IncomingEdgePredefinition(String myEdgeName)
            : base(myEdgeName, "")
        {
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdge">An outgoing IncomingEdge pre-definition.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, OutgoingEdgePredefinition myOutgoingEdge)
        {
            if (myVertexType != null && myOutgoingEdge != null)
                AttributeType = Combine(myVertexType.TypeName, myOutgoingEdge.AttributeName);

            return this;
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="myVertexType">A vertex type pre-definition.</param>
        /// <param name="myOutgoingEdgeName">The name of the IncomingEdge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(VertexTypePredefinition myVertexType, String myOutgoingEdgeName)
        {
            if (myVertexType != null && myOutgoingEdgeName != null)
                AttributeType = Combine(myVertexType.TypeName, myOutgoingEdgeName);

            return this;            
        }

        /// <summary>
        /// Sets the outgoing edge, this incoming edge is the backward version for.
        /// </summary>
        /// <param name="VertexTypeName">The name of the vertex type that declares the outgoing IncomingEdge.</param>
        /// <param name="myOutgoingEdgeName">The name of the IncomingEdge on the vertex type.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public IncomingEdgePredefinition SetOutgoingEdge(String myVertexTypeName, String myOutgoingEdgeName)
        {
            if (myVertexTypeName != null && myOutgoingEdgeName != null)
                AttributeType = Combine(myVertexTypeName, myOutgoingEdgeName);

            return this;
        }

        public IncomingEdgePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        private string Combine(string myTargetType, string myTargetEdgeName)
        {
            return String.Join(TypeSeparator.ToString(), myTargetType, myTargetEdgeName);
        }


    }
}
