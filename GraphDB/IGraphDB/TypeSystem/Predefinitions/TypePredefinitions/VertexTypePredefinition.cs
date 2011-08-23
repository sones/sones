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

        private List<IndexPredefinition>    _indices;
        protected List<UniquePredefinition> _uniques;

        private int _binaries = 0;
        private int _incoming = 0;
        private int _outgoing = 0;

        public int BinaryPropertyCount
        {
            get { return _binaries; }
        }

        public int IncomingEdgeCount
        {
            get { return _incoming; }
        }

        public int OutgoingEdgeCount
        {
            get { return _outgoing; }
        }

        /// <summary>
        /// The binary properties of this vertex type.
        /// </summary>
        public IEnumerable<BinaryPropertyPredefinition> BinaryProperties
        {
            get { return (_attributes == null) ? null : _attributes.OfType<BinaryPropertyPredefinition>(); }
        }

        /// <summary>
        /// The index definitions of this vertex type.
        /// </summary>
        public IEnumerable<IndexPredefinition> Indices
        {
            get { return (_indices == null) ? null : _indices.AsReadOnly(); }
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

        /// <summary>
        /// The unique definitions of this vertex type.
        /// </summary>
        public IEnumerable<UniquePredefinition> Uniques
        {
            get { return (_uniques == null) ? null : _uniques.AsReadOnly(); }
        }
                
        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new vertex type definition.
        /// </summary>
        /// <param name="myTypeName">The name of the vertex type.</param>
        public VertexTypePredefinition(String myTypeName)
            :base(myTypeName, "Vertex")
        {
        }

        #endregion

        #region fluent methods of abstract member

        /// <summary>
        /// Sets the name of the type this one inherits from
        /// </summary>
        /// <param name="myComment">The name of the super type</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition SetSuperTypeName(String mySuperTypeName)
        {
            if (!string.IsNullOrEmpty(mySuperTypeName))
            {
                SuperTypeName = mySuperTypeName;
            }
            return this;
        }

        /// <summary>
        /// Adds an unknown property to the vertex type definition
        /// </summary>
        /// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
        {
            if (myUnknownPredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myUnknownPredefinition);
                _unknown++;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the vertex type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddProperty(PropertyPredefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myPropertyDefinition);
                _properties++;
            }

            return this;
        }

        /// <summary>
        /// Adds a unique definition.
        /// </summary>
        /// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddUnique(UniquePredefinition myUniqueDefinition)
        {
            if (myUniqueDefinition != null)
            {
                _uniques = (_uniques) ?? new List<UniquePredefinition>();
                _uniques.Add(myUniqueDefinition);
            }

            return this;
        }

        /// <summary>
        /// Marks the vertex type as sealed.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition MarkAsSealed()
        {
            IsSealed = true;
            return this;
        }

        /// <summary>
        /// Marks the vertex type as abstract.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition MarkAsAbstract()
        {
            IsAbstract = true;
            return this;
        }

        /// <summary>
        /// Sets the comment of the vertex type.
        /// </summary>
        /// <param name="myComment">The comment.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        #endregion

        #region fluent methods

        /// <summary>
        /// Adds a binary definition.
        /// </summary>
        /// <param name="myBinaryPropertyPredefinition">The property definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddBinaryProperty(BinaryPropertyPredefinition myBinaryPropertyPredefinition)
        {
            if (myBinaryPropertyPredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myBinaryPropertyPredefinition);
                _binaries++;
            }

            return this;
        }

        /// <summary>
        /// Adds an index definition.
        /// </summary>
        /// <param name="myIndexDefinition">The index definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddIndex(IndexPredefinition myIndexDefinition)
        {
            if (myIndexDefinition != null)
            {
                _indices = (_indices) ?? new List<IndexPredefinition>();
                _indices.Add(myIndexDefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdgePredefinition">The definition of the outgoing IncomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public VertexTypePredefinition AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myOutgoingEdgePredefinition);
                _outgoing++;
            }

            return this;
        }

        public VertexTypePredefinition AddIncomingEdge(IncomingEdgePredefinition myIncomingEdgePredefinition)
        {
            if (myIncomingEdgePredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AAttributePredefinition>();
                _attributes.Add(myIncomingEdgePredefinition);
                _incoming++;
            }

            return this;
        }

        #endregion

    }
}