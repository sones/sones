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
using System.Linq;
using sones.GraphDB.TypeSystem;
using System.Collections.Generic;
namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for altering a edge type
    /// </summary>
    public sealed class RequestAlterEdgeType : IRequestAlterType
    {
        #region Data

        /// <summary>
        /// The name of the vertex type that is going to be altered
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Attributes which are to be added
        /// </summary>
        private List<AAttributePredefinition>   _toBeAddedAttributes;

        /// <summary>
        /// Attributes which are to be removed
        /// </summary>
        private List<String>                    _toBeRemovedProperties;
        private List<String>                    _toBeRemovedUnknownAttributes;
                        
        private Dictionary<String, String>      _toBeRenamedAttributes;

        #endregion

        #region counter

        public int AddPropertyCount 
        { 
            get; 
            private set; 
        }
        public int AddUnknownPropertyCount { 
            get; 
            private set; 
        }
        public int AddAttributeCount 
        { 
            get 
            { 
                return (_toBeAddedAttributes == null) ? 0 : _toBeAddedAttributes.Count; 
            } 
        }

        public int RemoveAttributeCount
        {
            get
            {
                return _toBeRemovedProperties.Count +
                       _toBeRemovedUnknownAttributes.Count;
            }
        }

        private int _renameAttribute = 0;

        public int RenameAttributeCount
        {
            get 
            { 
                return _renameAttribute; 
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new alter type request 
        /// </summary>
        /// <param name="myToBeAlteredType">The name of the type that should be altered.</param>
        public RequestAlterEdgeType(String myToBeAlteredType)
        {
            TypeName = myToBeAlteredType;
        }

        #endregion

        #region add

        /// <summary>
        /// Properties to be added to the altered type.
        /// </summary>
        public IEnumerable<PropertyPredefinition> ToBeAddedProperties
        {
            get 
            { 
                return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<PropertyPredefinition>(); 
            }
        }

        /// <summary>
        /// Unknown attributes to be added to the altered type.
        /// </summary>
        public IEnumerable<UnknownAttributePredefinition> ToBeAddedUnknownAttributes
        {
            get 
            { 
                return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<UnknownAttributePredefinition>(); 
            }
        }

        #endregion

        #region remove

        /// <summary>
        /// Properties to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedProperties
        {
            get 
            { 
                return _toBeRemovedProperties; 
            }
        }

        /// <summary>
        /// Incoming edges to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedUnknownAttributes
        {
            get 
            {
                return _toBeRemovedUnknownAttributes; 
            }
        }

        public void ClearToBeRemovedUnknownAttributes()
        {
            _toBeRemovedUnknownAttributes.Clear();
        }

        #endregion

        #region rename

        /// <summary>
        /// The renamed attributes
        /// </summary>
        public Dictionary<String, String> ToBeRenamedProperties
        {
            get 
            { 
                return _toBeRenamedAttributes; 
            }
        }

        #endregion

        /// <summary>
        /// Gets the altered comment for this type.
        /// </summary>
        public string AlteredComment 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the altered type name
        /// </summary>
        public string AlteredTypeName 
        { 
            get; 
            private set; 
        }

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get 
            { 
                return GraphDBAccessMode.TypeChange; 
            }
        }

        #endregion

        public void ResetUnknown()
        {
            _toBeAddedAttributes.RemoveAll(x => x is UnknownAttributePredefinition);
            AddUnknownPropertyCount = 0;
        }

        #region fluent methods

        #region add

        /// <summary>
        /// Adds an unknown property to the type definition
        /// </summary>
        /// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
        {
            if (myUnknownPredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AAttributePredefinition>();
                _toBeAddedAttributes.Add(myUnknownPredefinition);
                AddUnknownPropertyCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType AddProperty(PropertyPredefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AAttributePredefinition>();
                _toBeAddedAttributes.Add(myPropertyDefinition);
                AddPropertyCount++;
            }

            return this;
        }

        #endregion

        #region remove

        /// <summary>
        /// Removes a property to be removed
        /// </summary>
        /// <param name="myAttrName">The property name that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType RemoveUnknownAttribute(String myAttrName)
        {
            if (!String.IsNullOrWhiteSpace(myAttrName))
            {
                _toBeRemovedUnknownAttributes = (_toBeRemovedUnknownAttributes) ?? new List<String>();
                _toBeRemovedUnknownAttributes.Add(myAttrName);
            }

            return this;
        }

        /// <summary>
        /// Removes a property.
        /// </summary>
        /// <param name="myProperty">The property.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType RemoveProperty(String myProperty)
        {
            if (!String.IsNullOrWhiteSpace(myProperty))
            {
                _toBeRemovedProperties = (_toBeRemovedProperties) ?? new List<String>();
                _toBeRemovedProperties.Add(myProperty);
            }

            return this;
        }

        #endregion

        #region comment

        /// <summary>
        /// Sets the comment of the type.
        /// </summary>
        /// <param name="myComment">The comment.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType SetComment(String myComment)
        {
            AlteredComment = myComment;

            return this;
        }

        #endregion

        #region rename

        /// <summary>
        /// Renames an attribute
        /// </summary>
        /// <param name="myOldAttributeName">The old type name.</param>
        /// <param name="myNewAttributeName">The new type name.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType RenameAttribute(String myOldAttributeName, String myNewAttributeName)
        {
            if (!String.IsNullOrWhiteSpace(myOldAttributeName))
            {
                _toBeRenamedAttributes = (_toBeRenamedAttributes) ?? new Dictionary<String, String>();
                if (!_toBeRenamedAttributes.ContainsKey(myOldAttributeName))
                {
                    _toBeRenamedAttributes.Add(myOldAttributeName, myNewAttributeName);
                    _renameAttribute++;
                }
            }

            return this;
        }

        /// <summary>
        /// Renames the type
        /// </summary>
        /// <param name="myNewTypeName">The old type name.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterEdgeType RenameType(String myNewTypeName)
        {
            if (!String.IsNullOrWhiteSpace(myNewTypeName))
                AlteredTypeName = myNewTypeName;

            return this;
        }

        #endregion

        #endregion
    }
}