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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Request to alter a vertex type
    /// </summary>
    public sealed class RequestAlterVertexType : IRequest
    {
        #region Data

        /// <summary>
        /// The name of the vertex type that is going to be altered
        /// </summary>
        public readonly string VertexTypeName;
        
        /// <summary>
        /// Attributes which are to be added
        /// </summary>
        private List<AttributePredefinition>    _toBeAddedAttributes;
        private List<IndexPredefinition>        _toBeAddedIndices;
        private List<UniquePredefinition>       _toBeAddedUniques;
        private List<MandatoryPredefinition>    _toBeAddedMandatories;

        /// <summary>
        /// Attributes which are to be removed
        /// </summary>
        private List<String>                    _toBeRemovedProperties;
        private List<String>                    _toBeRemovedBinaryProperties;
        private List<String>                    _toBeRemovedIncomingEdges;
        private List<String>                    _toBeRemovedOutgoingEdges;
        private List<String>                    _toBeRemovedUnknownAttributes;
        private Dictionary<String, String>      _toBeRemovedIndices;
        private List<String>                    _toBeRemovedUniques;
        private List<String>                    _toBeRemovedMandatories;

        private Dictionary<String, String>      _toBeRenamedAttributes;


        #region add counter

        public int AddPropertyCount { get; private set; }
        public int AddIncomingEdgeCount { get; private set; }
        public int AddOutgoingEdgeCount { get; private set; }
        public int AddBinaryPropertyCount { get; private set; }
        public int AddUnknownPropertyCount { get; private set; }
        public int AddUniqueCount { get; private set; }
        public int AddMandatoryCount { get; private set; }
        public int AddIndicesCount { get; private set; }
        public int AddAttributeCount { get { return (_toBeAddedAttributes == null) ? 0 : _toBeAddedAttributes.Count; } }

        #endregion

        #region remove counter

        public int RemoveAttributeCount
        {
            get
            {
                return _toBeRemovedProperties.Count +
                       _toBeRemovedBinaryProperties.Count +
                       _toBeRemovedIncomingEdges.Count +
                       _toBeRemovedOutgoingEdges.Count +
                       _toBeRemovedUnknownAttributes.Count;
            }
        }

        #endregion

        #region rename counter

        private int _renameAttribute     = 0;
        
        public int RenameAttributeCount
        {
            get { return _renameAttribute; }
        }

        #endregion

        #region add
        /// <summary>
        /// Properties to be added to the altered type.
        /// </summary>
        public IEnumerable<PropertyPredefinition> ToBeAddedProperties
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<PropertyPredefinition>(); }
        }

        /// <summary>
        /// OutgoingEdges to be added to the altered type.
        /// </summary>
        public IEnumerable<OutgoingEdgePredefinition> ToBeAddedOutgoingEdges
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<OutgoingEdgePredefinition>(); }
        }

        /// <summary>
        /// IncomingEdges to be added to the altered type.
        /// </summary>
        public IEnumerable<IncomingEdgePredefinition> ToBeAddedIncomingEdges
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<IncomingEdgePredefinition>(); }
        }

        /// <summary>
        /// Unknown attributes to be added to the altered type.
        /// </summary>
        public IEnumerable<UnknownAttributePredefinition> ToBeAddedUnknownAttributes
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<UnknownAttributePredefinition>(); }
        }

        /// <summary>
        /// Indices to be added to the altered type.
        /// </summary>
        public IEnumerable<IndexPredefinition> ToBeAddedIndices
        {
            get { return (_toBeAddedIndices == null) ? null : _toBeAddedIndices.AsReadOnly(); }
        }

        /// <summary>
        /// Unique attributes to be added to the altered type.
        /// </summary>
        public IEnumerable<UniquePredefinition> ToBeAddedUniques
        {
            get { return (_toBeAddedUniques == null) ? null : _toBeAddedUniques.AsReadOnly(); }
        }

        /// <summary>
        /// Mandatory attributes to be added to the altered type.
        /// </summary>
        public IEnumerable<MandatoryPredefinition> ToBeAddedMandatories
        {
            get { return (_toBeAddedMandatories == null) ? null : _toBeAddedMandatories.AsReadOnly(); }
        }

        /// <summary>
        /// Binary properties to be added to the altered type.
        /// </summary>
        public IEnumerable<BinaryPropertyPredefinition> ToBeAddedBinaryProperties
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<BinaryPropertyPredefinition>(); }
        }
        #endregion

        #region remove

        /// <summary>
        /// Properties to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedProperties
        {
            get { return _toBeRemovedProperties; }
        }

        /// <summary>
        /// Outgoing edges to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedOutgoingEdges
        {
            get { return _toBeRemovedOutgoingEdges; }
        }

        public IEnumerable<String> ToBeRemovedBinaryProperties
        {
            get { return _toBeRemovedBinaryProperties; }
        }

        /// <summary>
        /// Incoming edges to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedIncomingEdges
        {
            get { return _toBeRemovedIncomingEdges; }
        }

        /// <summary>
        /// Incoming edges to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedUnknownAttributes
        {
            get { return _toBeRemovedUnknownAttributes; }
        }

        public void ClearToBeRemovedUnknownAttributes()
        {
            _toBeRemovedUnknownAttributes.Clear();
        }

        /// <summary>
        /// Indices to be removed from the altered type.
        /// </summary>
        public Dictionary<String, String> ToBeRemovedIndices
        {
            get { return _toBeRemovedIndices; }
        }
        
        /// <summary>
        /// Unique attributes to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedUniques
        {
            get { return _toBeRemovedUniques; }
        }

        /// <summary>
        /// Mandatory attributes to be removed from the altered type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedMandatories
        {
            get { return _toBeRemovedMandatories; }
        }

        #endregion

        #region rename

        /// <summary>
        /// The renamed attributes
        /// </summary>
        public Dictionary<String, String> ToBeRenamedProperties
        {
            get { return _toBeRenamedAttributes; }
        }

        #endregion

        /// <summary>
        /// Gets the altered comment for this vertex type.
        /// </summary>
        public string AlteredComment { get; private set; }

        /// <summary>
        /// Gets the altered vertex type name
        /// </summary>
        public string AlteredVertexTypeName { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new alter type request 
        /// </summary>
        /// <param name="myToBeAlteredVertexType">The name of the vertex type that should be altered.</param>
        public RequestAlterVertexType(String myToBeAlteredVertexType)
        {
            VertexTypeName = myToBeAlteredVertexType;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion

        #region fluent methods

        #region add
        /// <summary>
        /// Adds an unknown property to the vertex type definition
        /// </summary>
        /// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
        {
            if (myUnknownPredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myUnknownPredefinition);
                AddUnknownPropertyCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds a property to the vertex type definition
        /// </summary>
        /// <param name="myPropertyDefinition">The property definition that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddProperty(PropertyPredefinition myPropertyDefinition)
        {
            if (myPropertyDefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myPropertyDefinition);
                AddPropertyCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdgePredefinition">The definition of the outgoing IncomingEdge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myOutgoingEdgePredefinition);
                AddOutgoingEdgeCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds a binary property.
        /// </summary>
        /// <param name="myBinaryPropertyPredefinition">The defintition of the binary property.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddBinaryProperty(BinaryPropertyPredefinition myBinaryPropertyPredefinition)
        {
            if (myBinaryPropertyPredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myBinaryPropertyPredefinition);
                AddBinaryPropertyCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds a incoming edge.
        /// </summary>
        /// <param name="myIncomingEdgePredefinition">The definition of the incoming edge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddIncomingEdge(IncomingEdgePredefinition myIncomingEdgePredefinition)
        {
            if (myIncomingEdgePredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myIncomingEdgePredefinition);
                AddIncomingEdgeCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds a unique definition.
        /// </summary>
        /// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddUnique(UniquePredefinition myUniqueDefinition)
        {
            if (myUniqueDefinition != null)
            {
                _toBeAddedUniques = (_toBeAddedUniques) ?? new List<UniquePredefinition>();
                _toBeAddedUniques.Add(myUniqueDefinition);
                AddUniqueCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds a mandatory definition.
        /// </summary>
        /// <param name="myMandatoryDefinition">The mandatory definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddMandatory(MandatoryPredefinition myMandatoryDefinition)
        {
            if (myMandatoryDefinition != null)
            {
                _toBeAddedMandatories = (_toBeAddedMandatories) ?? new List<MandatoryPredefinition>();
                _toBeAddedMandatories.Add(myMandatoryDefinition);
                AddMandatoryCount++;
            }

            return this;
        }

        /// <summary>
        /// Adds an index definition.
        /// </summary>
        /// <param name="myIndexDefinition">The index definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddIndex(IndexPredefinition myIndexDefinition)
        {
            if (myIndexDefinition != null)
            {
                _toBeAddedIndices = (_toBeAddedIndices) ?? new List<IndexPredefinition>();
                _toBeAddedIndices.Add(myIndexDefinition);
                AddIndicesCount++;
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
        public RequestAlterVertexType RemoveUnknownAttribute(String myAttrName)
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
        public RequestAlterVertexType RemoveProperty(String myProperty)
        {
            if (!String.IsNullOrWhiteSpace(myProperty))
            {
                _toBeRemovedProperties = (_toBeRemovedProperties) ?? new List<String>();
                _toBeRemovedProperties.Add(myProperty);
            }

            return this;
        }

        /// <summary>
        /// Removes a binary property.
        /// </summary>
        /// <param name="myBinaryProperty">The binary property.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveBinaryProperty(String myBinaryProperty)
        {
            if (!String.IsNullOrWhiteSpace(myBinaryProperty))
            {
                _toBeRemovedBinaryProperties = (_toBeRemovedBinaryProperties) ?? new List<String>();
                _toBeRemovedBinaryProperties.Add(myBinaryProperty);
            }

            return this;
        }

        /// <summary>
        /// Removes an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdge">The outgoing IncomingEdge.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveOutgoingEdge(String myOutgoingEdge)
        {
            if (!String.IsNullOrWhiteSpace(myOutgoingEdge))
            {
                _toBeRemovedOutgoingEdges = (_toBeRemovedOutgoingEdges) ?? new List<String>();
                _toBeRemovedOutgoingEdges.Add(myOutgoingEdge);
            }

            return this;
        }

        /// <summary>
        /// Removes an incoming edge.
        /// </summary>
        /// <param name="myIncomingEdge">The incomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveIncomingEdge(String myIncomingEdge)
        {
            if (!String.IsNullOrWhiteSpace(myIncomingEdge))
            {
                _toBeRemovedIncomingEdges = (_toBeRemovedIncomingEdges) ?? new List<String>();
                _toBeRemovedIncomingEdges.Add(myIncomingEdge);
            }

            return this;
        }

        /// <summary>
        /// Removes a unique definition.
        /// </summary>
        /// <param name="myUnique">The name of the unique property.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveUnique(String myUnique)
        {
            if (!String.IsNullOrWhiteSpace(myUnique))
            {
                _toBeRemovedUniques = (_toBeRemovedUniques) ?? new List<String>();
                _toBeRemovedUniques.Add(myUnique);
            }

            return this;
        }

        /// <summary>
        /// Removes a mandatory constraint.
        /// </summary>
        /// <param name="myMandatory">The name of the property</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveMandatory(String myMandatory)
        {
            if (!String.IsNullOrWhiteSpace(myMandatory))
            {
                _toBeRemovedMandatories = (_toBeRemovedMandatories) ?? new List<String>();
                _toBeRemovedMandatories.Add(myMandatory);
            }

            return this;
        }

        /// <summary>
        /// Removes an index definition.
        /// </summary>
        /// <param name="myIndexName">The index name that is going to be added.</param>
        /// <param name="myEdition">The index name that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveIndex(String myIndexName, String myEdition)
        {
            if (!String.IsNullOrWhiteSpace(myIndexName))
            {
                _toBeRemovedIndices = (_toBeRemovedIndices) ?? new Dictionary<String, String>();
                if (!_toBeRemovedIndices.ContainsKey(myIndexName))
                {
                    _toBeRemovedIndices.Add(myIndexName, myEdition);
                }
            }

            return this;
        } 

        #endregion

        #region comment

        /// <summary>
        /// Sets the comment of the vertex type.
        /// </summary>
        /// <param name="myComment">The comment.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType SetComment(String myComment)
        {
            AlteredComment = myComment;

            return this;
        }

        #endregion

        #region rename

        /// <summary>
        /// Renames an attribute
        /// </summary>
        /// <param name="myOldAttributeName">The old vertex type name.</param>
        /// <param name="myNewAttributeName">The new vertex type name.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RenameAttribute(String myOldAttributeName, String myNewAttributeName)
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
        /// Renames the vertex type
        /// </summary>
        /// <param name="myNewVertexTypeName">The old vertex type name.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RenameVertexType(String myNewVertexTypeName)
        {
            if (!String.IsNullOrWhiteSpace(myNewVertexTypeName))
            {
                AlteredVertexTypeName = myNewVertexTypeName;
            }

            return this;
        }

        #endregion

        #endregion

        public void ResetUnknown()
        {
            _toBeAddedAttributes.RemoveAll(x => x is UnknownAttributePredefinition);
            AddUnknownPropertyCount = 0;
        }
    }
}
