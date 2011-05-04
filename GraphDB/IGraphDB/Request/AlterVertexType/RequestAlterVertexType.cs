using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Request.CreateVertexTypes;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Request to alter a vertex type
    /// </summary>
    public sealed class RequestAlterVertexType : IRequest
    {
        #region Data

        /// <summary>
        /// The name of the vertex type that is going to be created
        /// </summary>
        public readonly string VertexTypeName;
        
        private List<AttributePredefinition>    _toBeAddedAttributes;
        private List<IndexPredefinition>        _toBeAddedIndices;
        private List<UniquePredefinition>       _toBeAddedUniques;

        private List<String>                    _toBeRemovedAttributes;
        private List<String>                    _toBeRemovedIncomingEdges;
        private List<String>                    _toBeRemovedOutgoingEdges;
        private Dictionary<String, String>      _toBeRemovedIndices;
        private List<UniquePredefinition>       _toBeRemovedUniques;

        #region add counter
        private int _addProperties  = 0;
        private int _addIncoming    = 0;
        private int _addOutgoing    = 0;
        private int _addBinaries    = 0;
        private int _addUnknown     = 0;
        private int _addUnique      = 0;
        private int _addIndices     = 0;

        public int AddPropertyCount
        {
            get { return _addProperties; }
        }

        public int AddIncomingEdgeCount
        {
            get { return _addIncoming; }
        }

        public int AddOutgoingEdgeCount
        {
            get { return _addOutgoing; }
        }

        public int AddAttributeCount
        {
            get { return (_toBeAddedAttributes == null) ? 0 : _toBeAddedAttributes.Count; }
        }

        public int AddBinaryPropertyCount
        {
            get { return _addBinaries; }
        }

        public int AddUniquePropertyCount
        {
            get { return _addUnique; }
        }

        public int AddUnknownPropertyCount
        {
            get { return _addUnknown; }
        }

        public int AddIndicesPropertyCount
        {
            get { return _addIndices; }
        }
        #endregion

        #region remove counter
        private int _removeAttributes   = 0;
        private int _removeIncoming     = 0;
        private int _removeOutgoing     = 0;
        private int _removeBinaries     = 0;
        private int _removeUnique       = 0;
        private int _removeIndices      = 0;

        public int RemoveAttributeCount
        {
            get { return _removeAttributes; }
        }

        public int RemoveIncomingEdgeCount
        {
            get { return _removeIncoming; }
        }

        public int RemoveOutgoingEdgeCount
        {
            get { return _removeOutgoing; }
        }

        public int RemoveBinaryPropertyCount
        {
            get { return _removeBinaries; }
        }

        public int RemoveUniquePropertyCount
        {
            get { return _removeUnique; }
        }

        public int RemoveIndicesPropertyCount
        {
            get { return _removeIndices; }
        }
        #endregion

        #region add 
        /// <summary>
        /// The properties of the vertex type.
        /// </summary>
        public IEnumerable<PropertyPredefinition> ToBeAddedProperties
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<PropertyPredefinition>(); }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<OutgoingEdgePredefinition> ToBeAddedOutgoingEdges
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<OutgoingEdgePredefinition>(); }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<IncomingEdgePredefinition> ToBeAddedIncomingEdges
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<IncomingEdgePredefinition>(); }
        }

        /// <summary>
        /// the unknown attributes of this edge type.
        /// </summary>
        public IEnumerable<UnknownAttributePredefinition> ToBeAddedUnknownAttributes
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<UnknownAttributePredefinition>(); }
        }

        /// <summary>
        /// The index definitions of this vertex type.
        /// </summary>
        public IEnumerable<IndexPredefinition> ToBeAddedIndices
        {
            get { return (_toBeAddedIndices == null) ? null : _toBeAddedIndices.AsReadOnly(); }
        }

        /// <summary>
        /// The unique definitions of this vertex type.
        /// </summary>
        public IEnumerable<UniquePredefinition> ToBeAddedUniques
        {
            get { return (_toBeAddedUniques == null) ? null : _toBeAddedUniques.AsReadOnly(); }
        }

        public IEnumerable<BinaryPropertyPredefinition> ToBeAddedBinaryProperties
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<BinaryPropertyPredefinition>(); }
        }
        #endregion

        #region remove
        /// <summary>
        /// The properties of the vertex type.
        /// </summary>
        public IEnumerable<String> ToBeRemovedProperties
        {
            get { return (_toBeRemovedAttributes == null) ? null : _toBeRemovedAttributes.OfType<String>(); }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<long> ToBeRemovedOutgoingEdges
        {
            get { return (_toBeRemovedOutgoingEdges == null) ? null : _toBeRemovedOutgoingEdges.OfType<long>(); }
        }

        /// <summary>
        /// The outgoing edges of this vertex type.
        /// </summary>
        public IEnumerable<long> ToBeRemovedIncomingEdges
        {
            get { return (_toBeRemovedIncomingEdges == null) ? null : _toBeRemovedIncomingEdges.OfType<long>(); }
        }

        /// <summary>
        /// the unknown attributes of this edge type.
        /// </summary>
        public IEnumerable<UnknownAttributePredefinition> ToBeRemovedUnknownAttributes
        {
            get { return (_toBeRemovedAttributes == null) ? null : _toBeRemovedAttributes.OfType<UnknownAttributePredefinition>(); }
        }

        /// <summary>
        /// The index definitions of this vertex type.
        /// </summary>
        public Dictionary<String, String> ToBeRemovedIndices
        {
            get { return (_toBeRemovedIndices == null) ? null : _toBeRemovedIndices; }
        }
        
        /// <summary>
        /// The unique definitions of this vertex type.
        /// </summary>
        public IEnumerable<UniquePredefinition> ToBeRemovedUniques
        {
            get { return (_toBeRemovedUniques == null) ? null : _toBeRemovedUniques.AsReadOnly(); }
        }
        #endregion

        /// <summary>
        /// Gets the comment for this vertex type.
        /// </summary>
        public string AlteredComment { get; private set; }

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
                _addUnknown++;
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
                _addProperties++;
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdgePredefinition">The definition of the outgoing IncomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        {
            if (myOutgoingEdgePredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myOutgoingEdgePredefinition);
                _addOutgoing++;
            }

            return this;
        }

        public RequestAlterVertexType AddBinaryProperty(BinaryPropertyPredefinition myBinaryPropertyPredefinition)
        {
            if (myBinaryPropertyPredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myBinaryPropertyPredefinition);
                _addBinaries++;
            }

            return this;
        }

        public RequestAlterVertexType AddIncomingEdge(IncomingEdgePredefinition myIncomingEdgePredefinition)
        {
            if (myIncomingEdgePredefinition != null)
            {
                _toBeAddedAttributes = (_toBeAddedAttributes) ?? new List<AttributePredefinition>();
                _toBeAddedAttributes.Add(myIncomingEdgePredefinition);
                _addIncoming++;
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
                _addUnique++;
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
                _addIndices++;
            }

            return this;
        } 
        #endregion

        #region remove
        /// <summary>
        /// Adds an property to be removed
        /// </summary>
        /// <param name="myUnknownName">The unknwown property name that is going to be added</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveAttribute(String myAttrName)
        {
            if (String.IsNullOrWhiteSpace(myAttrName))
            {
                _toBeRemovedAttributes = (_toBeRemovedAttributes) ?? new List<String>();
                _toBeRemovedAttributes.Add(myAttrName);
                _removeAttributes++;
            }

            return this;
        }

        /// <summary>
        /// Adds an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdgeID">The id of the outgoing IncomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveOutgoingEdge(String myOutgoingEdgeID)
        {
            if (myOutgoingEdgeID != null)
            {
                _toBeRemovedOutgoingEdges = (_toBeRemovedOutgoingEdges) ?? new List<String>();
                _toBeRemovedOutgoingEdges.Add(myOutgoingEdgeID);
                _removeOutgoing++;
            }

            return this;
        }

        /// <summary>
        /// Adds an incoming edge.
        /// </summary>
        /// <param name="myIncomingEdgeID">The id of the incomingEdge</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveIncomingEdge(String myIncomingEdgeID)
        {
            if (myIncomingEdgeID != null)
            {
                _toBeRemovedIncomingEdges = (_toBeRemovedIncomingEdges) ?? new List<String>();
                _toBeRemovedIncomingEdges.Add(myIncomingEdgeID);
                _removeIncoming++;
            }

            return this;
        }

        /// <summary>
        /// Adds a unique definition.
        /// </summary>
        /// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveUnique(UniquePredefinition myUniqueDefinition)
        {
            if (myUniqueDefinition != null)
            {
                _toBeRemovedUniques = (_toBeRemovedUniques) ?? new List<UniquePredefinition>();
                _toBeRemovedUniques.Add(myUniqueDefinition);
                _removeUnique++;
            }

            return this;
        }

        /// <summary>
        /// Adds an index definition.
        /// </summary>
        /// <param name="myIndexDefinition">The index definition that is going to be added.</param>
        /// <returns>The reference of the current object. (fluent interface).</returns>
        public RequestAlterVertexType RemoveIndex(String myIndexName, String myEdition)
        {
            if (String.IsNullOrWhiteSpace(myIndexName))
            {
                _toBeRemovedIndices = (_toBeRemovedIndices) ?? new Dictionary<String, String>();
                if (!_toBeRemovedIndices.ContainsKey(myIndexName))
                {
                    _toBeRemovedIndices.Add(myIndexName, myEdition);
                    _removeIndices++;
                }
            }

            return this;
        } 
        #endregion

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
    }
}
