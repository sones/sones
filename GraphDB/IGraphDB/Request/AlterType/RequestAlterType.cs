using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Request.CreateVertexTypes;

namespace sones.GraphDB.Request.AlterType
{
    /// <summary>
    /// Request to alter a vertex type
    /// </summary>
    public sealed class RequestAlterType : IRequest
    {
        #region Data

        /// <summary>
        /// The name of the vertex type that is going to be created
        /// </summary>
        public readonly string VertexTypeName;
        
        private List<AttributePredefinition> _toBeAddedAttributes;
        private List<UniquePredefinition> _uniques;
        private List<IndexPredefinition> _indices;

        private int _properties = 0;
        private int _incoming = 0;
        private int _outgoing = 0;
        private int _binaries = 0;
        private int _unknown = 0;

        public int PropertyCount
        {
            get { return _properties; }
        }

        public int IncomingEdgeCount
        {
            get { return _incoming; }
        }

        public int OutgoingEdgeCount
        {
            get { return _outgoing; }
        }

        public int AttributeCount
        {
            get { return (_toBeAddedAttributes == null) ? 0 : _toBeAddedAttributes.Count; }
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
        /// The unique definitions of this vertex type.
        /// </summary>
        public IEnumerable<UniquePredefinition> Uniques
        {
            get { return (_uniques == null) ? null : _uniques.AsReadOnly(); }
        }

        public IEnumerable<BinaryPropertyPredefinition> ToBeAddedBinaryProperties
        {
            get { return (_toBeAddedAttributes == null) ? null : _toBeAddedAttributes.OfType<BinaryPropertyPredefinition>(); }
        }

        /// <summary>
        /// The index definitions of this vertex type.
        /// </summary>
        public IEnumerable<IndexPredefinition> ToBeAddedIndices
        {
            get { return (_indices == null) ? null : _indices.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the comment for this vertex type.
        /// </summary>
        public string AlteredComment { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new alter type request 
        /// </summary>
        public RequestAlterType()
        { 
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.TypeChange; }
        }

        #endregion

        //#region fluent methods

        ///// <summary>
        ///// Adds an unknown property to the vertex type definition
        ///// </summary>
        ///// <param name="myUnknownPredefinition">The unknwown property definition that is going to be added</param>
        ///// <returns>The reference of the current object. (fluent interface).</returns>
        //public RequestAlterType AddUnknownAttribute(UnknownAttributePredefinition myUnknownPredefinition)
        //{
        //    if (myUnknownPredefinition != null)
        //    {
        //        _attributes = (_attributes) ?? new List<AttributePredefinition>();
        //        _attributes.Add(myUnknownPredefinition);
        //        _unknown++;
        //    }

        //    return this;
        //}

        ///// <summary>
        ///// Adds a property to the vertex type definition
        ///// </summary>
        ///// <param name="myPropertyDefinition">The property definition that is going to be added</param>
        ///// <returns>The reference of the current object. (fluent interface).</returns>
        //public RequestAlterType AddProperty(PropertyPredefinition myPropertyDefinition)
        //{
        //    if (myPropertyDefinition != null)
        //    {
        //        _attributes = (_attributes) ?? new List<AttributePredefinition>();
        //        _attributes.Add(myPropertyDefinition);
        //        _properties++;
        //    }

        //    return this;
        //}

        ///// <summary>
        ///// Adds an outgoing edge.
        ///// </summary>
        ///// <param name="myOutgoingEdgePredefinition">The definition of the outgoing IncomingEdge</param>
        ///// <returns>The reference of the current object. (fluent interface).</returns>
        //public RequestAlterType AddOutgoingEdge(OutgoingEdgePredefinition myOutgoingEdgePredefinition)
        //{
        //    if (myOutgoingEdgePredefinition != null)
        //    {
        //        _attributes = (_attributes) ?? new List<AttributePredefinition>();
        //        _attributes.Add(myOutgoingEdgePredefinition);
        //        _outgoing++;
        //    }

        //    return this;
        //}

        //public RequestAlterType AddBinaryProperty(BinaryPropertyPredefinition myBinaryPropertyPredefinition)
        //{
        //    if (myBinaryPropertyPredefinition != null)
        //    {
        //        _attributes = (_attributes) ?? new List<AttributePredefinition>();
        //        _attributes.Add(myBinaryPropertyPredefinition);
        //        _binaries++;
        //    }

        //    return this;
        //}

        //public RequestAlterType AddIncomingEdge(IncomingEdgePredefinition myIncomingEdgePredefinition)
        //{
        //    if (myIncomingEdgePredefinition != null)
        //    {
        //        _attributes = (_attributes) ?? new List<AttributePredefinition>();
        //        _attributes.Add(myIncomingEdgePredefinition);
        //        _incoming++;
        //    }

        //    return this;
        //}

        ///// <summary>
        ///// Adds a unique definition.
        ///// </summary>
        ///// <param name="myUniqueDefinition">The unique definition that is going to be added.</param>
        ///// <returns>The reference of the current object. (fluent interface).</returns>
        //public RequestAlterType AddUnique(UniquePredefinition myUniqueDefinition)
        //{
        //    if (myUniqueDefinition != null)
        //    {
        //        _uniques = (_uniques) ?? new List<UniquePredefinition>();
        //        _uniques.Add(myUniqueDefinition);
        //    }

        //    return this;
        //}

        ///// <summary>
        ///// Adds an index definition.
        ///// </summary>
        ///// <param name="myIndexDefinition">The index definition that is going to be added.</param>
        ///// <returns>The reference of the current object. (fluent interface).</returns>
        //public RequestAlterType AddIndex(IndexPredefinition myIndexDefinition)
        //{
        //    if (myIndexDefinition != null)
        //    {
        //        _indices = (_indices) ?? new List<IndexPredefinition>();
        //        _indices.Add(myIndexDefinition);
        //    }

        //    return this;
        //}

        ///// <summary>
        ///// Sets the comment of the vertex type.
        ///// </summary>
        ///// <param name="myComment">The comment.</param>
        ///// <returns>The reference of the current object. (fluent interface).</returns>
        //public RequestAlterType SetComment(String myComment)
        //{
        //    Comment = myComment;

        //    return this;
        //}

        //#endregion

    }
}
