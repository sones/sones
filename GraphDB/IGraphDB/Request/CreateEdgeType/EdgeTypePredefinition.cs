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

using System.Linq;
using System.Collections.Generic;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition for an edge type.
    /// </summary>
    public class EdgeTypePredefinition
    {
        #region Data

        /// <summary>
        /// The name of the edge type that is going to be created.
        /// </summary>
        public readonly string EdgeTypeName;
        private List<AttributePredefinition> _attributes;
        private List<UniquePredefinition> _uniques;
        private List<IndexPredefinition> _indices;

        private int _properties = 0;
        private int _binaries = 0;
        private int _unknown = 0;

        public int AttributeCount 
        {
            get { return (_attributes == null) ? 0 : _attributes.Count; }
        }
        
        public int PropertyCount
        {
            get { return _properties; }
        }

        public int BinaryPropertyCount 
        {
            get { return _binaries; }
        }

        public int UnknownPropertyCount
        {
            get { return _unknown; }
        }

        /// <summary>
        /// The name of the edge type this edge type inherites from.
        /// </summary>
        public string SuperEdgeTypeName { get; private set; }

        /// <summary>
        /// The properties of the vertex type.
        /// </summary>
        public IEnumerable<PropertyPredefinition> Properties
        {
            get { return (_attributes== null) ? null : _attributes.OfType<PropertyPredefinition>(); }
        }

        /// <summary>
        /// the unknown attributes of this edge type.
        /// </summary>
        public IEnumerable<UnknownAttributePredefinition> UnknownAttributes
        {
            get { return (_attributes == null) ? null : _attributes.OfType<UnknownAttributePredefinition>(); }
        }

        /// <summary>
        /// The unique definitions of this vertex type.
        /// </summary>
        public IEnumerable<UniquePredefinition> Uniques
        {
            get { return (_uniques == null) ? null : _uniques.AsReadOnly(); }
        }

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
        /// Gets if the vertex type will be sealed.
        /// </summary>
        public bool IsSealed { get; private set; }

        /// <summary>
        /// Gets if the vertex type will be abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets the comment for this vertex type.
        /// </summary>
        public string Comment { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new edge type definition.
        /// </summary>
        /// <param name="myEdgeTypeName">The name of the edge type.</param>
        public EdgeTypePredefinition(String myEdgeTypeName)
        {
            if (string.IsNullOrEmpty(myEdgeTypeName))
            {
                throw new ArgumentOutOfRangeException("myVertexTypeName", myEdgeTypeName);
            }

            EdgeTypeName = myEdgeTypeName;
            SuperEdgeTypeName = "Edge";
            IsSealed = false;
            IsAbstract = false;
            Comment = String.Empty;

        }

        #endregion

        #region fluent methods


        /// <summary>
        /// Sets the name of the edge type this one inherits from
        /// </summary>
        /// <param name="mySuperEdgeTypeName">The name of the super edge type</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition SetSuperVertexTypeName(String mySuperEdgeTypeName)
        {
            if (!string.IsNullOrEmpty(mySuperEdgeTypeName))
            {
                SuperEdgeTypeName = mySuperEdgeTypeName;
            }

            return this;
        }

        /// <summary>
        /// Adds an unknown property to the edge type definition
        /// </summary>
        /// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
        {
            if (myUnknownPredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AttributePredefinition>();
                _attributes.Add(myUnknownPredefinition);
                _unknown++;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the edge type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddProperty(PropertyPredefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                _attributes = (_attributes) ?? new List<AttributePredefinition>();
                _attributes.Add(myPropertyDefinition);
                _properties++;
            }

            return this;
        }

        /// <summary>
        /// Adds a binary property predefinition to the edge type.
        /// </summary>
        /// <param name="myBinaryPropertyPredefinition">The binary property predefinition which is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddBinaryProperty(BinaryPropertyPredefinition myBinaryPropertyPredefinition)
        {
            if (myBinaryPropertyPredefinition != null)
            {
                _attributes = (_attributes) ?? new List<AttributePredefinition>();
                _attributes.Add(myBinaryPropertyPredefinition);
                _binaries++;
            }

            return this;
        }

        /// <summary>
        /// Adds a unique definition.
        /// </summary>
        /// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddUnique(UniquePredefinition myUniqueDefinition)
        {
            if (myUniqueDefinition != null)
            {
                _uniques = (_uniques) ?? new List<UniquePredefinition>();
                _uniques.Add(myUniqueDefinition);
            }

            return this;
        }

        /// <summary>
        /// Adds an index definition.
        /// </summary>
        /// <param name="myIndexDefinition">The index definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition AddIndex(IndexPredefinition myIndexDefinition)
        {
            if (myIndexDefinition != null)
            {
                _indices = (_indices) ?? new List<IndexPredefinition>();
                _indices.Add(myIndexDefinition);
            }

            return this;
        }

        /// <summary>
        /// Marks the edge type as sealed.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition MarkAsSealed()
        {
            IsSealed = true;
            return this;
        }

        /// <summary>
        /// Marks the edge type as abstract.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition MarkAsAbstract()
        {
            IsAbstract = true;
            return this;
        }

        /// <summary>
        /// Sets the comment of the edge type.
        /// </summary>
        /// <param name="myComment">The comment.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public EdgeTypePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        #endregion

        /// <summary>
        /// Removes all unknown attributes.
        /// </summary>
        /// <remarks>This method is used internally.</remarks>
        public void ResetUnknown()
        {
            _attributes.RemoveAll(x => x is UnknownAttributePredefinition);
            _unknown = 0;
        }
    }
}
