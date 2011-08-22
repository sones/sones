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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a predefinition has an incoming IncomingEdge or outgoing IncomingEdge pointing to a nonexisting vertex type.
    /// </summary>
    public class TargetVertexTypeNotFoundException : AGraphDBVertexAttributeException
    {
        /// <summary>
        /// The Predefinition that contains the edges.
        /// </summary>
        public VertexTypePredefinition Predefinition { get; private set; }

        /// <summary>
        /// The vertex type name, that was not found.
        /// </summary>
        public string TargetVertexTypeName { get; private set; }

        /// <summary>
        /// The list of edges that causes the exception.
        /// </summary>
        public IEnumerable<String> Edges { get; private set; }

        /// <summary>
        /// Creates a new instance of TargetVertexTypeNotFoundException.
        /// </summary>
        /// <param name="myPredefinition">
        /// The Predefinition that contains the edges.
        /// </param>
        /// <param name="myTargetVertexTypeName">
        /// The vertex type name, that was not found.
        /// </param>
        /// <param name="myEdges">
        /// The list of edges that causes the exception.
        /// </param>
        public TargetVertexTypeNotFoundException(VertexTypePredefinition myPredefinition, string myTargetVertexTypeName, IEnumerable<String> myEdges)
        {
            this.Predefinition = myPredefinition;
            this.TargetVertexTypeName = myTargetVertexTypeName;
            this.Edges = myEdges;
            _msg = string.Format("The outgoing edges ({0}) on vertex type {1} does point to a not existing target type {2}.", String.Join(",", myEdges), myPredefinition.TypeName, myTargetVertexTypeName);
        }
    }
}
