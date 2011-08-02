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

namespace sones.GraphDB.TypeSystem
{
    public abstract class ATypePredefinition
    {
        #region Data

        // The name of the vertex type that is going to be created
        public readonly string TypeName;
        protected List<AAttributePredefinition> _attributes;
        private List<UniquePredefinition> _uniques;
        
        private int _properties = 0;
        private int _unknown = 0;

        public int AttributeCount 
        {
            get { return (_attributes == null) ? 0 : _attributes.Count; }
        }
        
        public int PropertyCount
        {
            get { return _properties; }
        }
        
        public int UnknownPropertyCount
        {
            get { return _unknown; }
        }

        /// <summary>
        /// The name of the vertex type this vertex types inherites from.
        /// </summary>
        public string SuperTypeName { get; private set; }

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
        /// Creates a new vertex type definition.
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type.</param>
        public ATypePredefinition(String myTypeName, String mySuperTypeName)
        {
            if (string.IsNullOrEmpty(myTypeName))
            {
                throw new ArgumentOutOfRangeException("myVertexTypeName", myTypeName);
            }

            TypeName = myTypeName;
            SuperTypeName = mySuperTypeName;
            IsSealed = false;
            IsAbstract = false;
            Comment = String.Empty;
        }

        #endregion

        #region fluent methods

        /// <summary>
        /// Sets the name of the vertex type this one inherits from
        /// </summary>
        /// <param name="myComment">The name of the super vertex type</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public ATypePredefinition SetSuperVertexTypeName(String mySuperVertexTypeName)
        {
            if (!string.IsNullOrEmpty(mySuperVertexTypeName))
            {
                SuperTypeName = mySuperVertexTypeName;
            }

            return this;
        }

        /// <summary>
        /// Adds an unknown property to the vertex type definition
        /// </summary>
        /// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public ATypePredefinition AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
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
        public ATypePredefinition AddProperty(PropertyPredefinition myPropertyDefinition)
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
        public ATypePredefinition AddUnique(UniquePredefinition myUniqueDefinition)
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
        public ATypePredefinition MarkAsSealed()
        {
            IsSealed = true;
            return this;
        }

        /// <summary>
        /// Marks the vertex type as abstract.
        /// </summary>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public ATypePredefinition MarkAsAbstract()
        {
            IsAbstract = true;
            return this;
        }

        /// <summary>
        /// Sets the comment of the vertex type.
        /// </summary>
        /// <param name="myComment">The comment.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public ATypePredefinition SetComment(String myComment)
        {
            Comment = myComment;

            return this;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Removes all unknown attributes.
        /// </summary>
        /// <remarks>This method is used internally.</remarks>
        public void ResetUnknown()
        {
            _attributes.RemoveAll(x => x is UnknownAttributePredefinition);
            _unknown = 0;
        }

        #endregion
    }
}
