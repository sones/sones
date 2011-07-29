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

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// The definition for vertex types
    /// </summary>
    public sealed class VertexTypePredefinition : ATypePredefinition
    {
        #region Data

        private int _incoming = 0;
        private int _outgoing = 0;
        
        public int IncomingEdgeCount
        {
            get { return _incoming; }
        }

        public int OutgoingEdgeCount
        {
            get { return _outgoing; }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<OutgoingEdgePredefinition> OutgoingEdges
        {
            get { return (_attributes == null) ? null : _attributes.OfType<OutgoingEdgePredefinition>(); }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<IncomingEdgePredefinition> IncomingEdges
        {
            get { return (_attributes == null) ? null : _attributes.OfType<IncomingEdgePredefinition>(); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex type definition.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type.</param>
        public VertexTypePredefinition(String myTypeName)
            :base(myTypeName, "Vertex")
        { }

        #endregion

        #region fluent methods

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdgePredefinition">The definition of the outgoing IncomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AttributePredefinition>();
                _attributes.Add(myOutgoingEdgePredefinition);
                _outgoing++;
            }

            return this;
        }

        public VertexTypePredefinition AddIncomingEdge(IncomingEdgePredefinition myIncomingEdgePredefinition)
        {
            if (myIncomingEdgePredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AttributePredefinition>();
                _attributes.Add(myIncomingEdgePredefinition);
                _incoming++;
            }

            return this;
        }

        #endregion

    }
}