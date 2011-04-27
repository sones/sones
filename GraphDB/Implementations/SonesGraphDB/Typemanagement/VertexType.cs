using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.Library.LanguageExtensions;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Manager.BaseGraph;

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class transforms an IVertex (came from FS) into a vertex type.
    /// </summary>
    /// <remarks>
    /// An object of this class caches the intermediate results for the live time of the object itself. 
    /// This means equal queries to the FS are only done one time per object.
    /// The object also does most of its queries lazy. This means the queries are executed only if the result is needed.
    /// </remarks>
    internal class VertexType: IVertexType
    {
        #region Constants

        /// <summary>
        /// This is the initialization count of the result list of a GetChildVertices method
        /// </summary>
        private const int ExpectedChildTypes = 50;

        /// <summary>
        /// This is the initialization count of the result list of a GetAttributes method
        /// </summary>
        private const int ExpectedAttributes = 50;

        #endregion

        #region Data

        /// <summary>
        /// Stores the FS vertex.
        /// </summary>
        private readonly IVertex _vertex;

        /// <summary>
        /// Stores the list of attributes indexed by the attribute name.
        /// </summary>
        private readonly Lazy<Dictionary<String, IAttributeDefinition>> _attributes;

        /// <summary>
        /// Stores the parent vertex type.
        /// </summary>
        private readonly Lazy<IVertexType> _parent;

        /// <summary>
        /// Stores the list of child vertex types.
        /// </summary>
        private readonly Lazy<List<IVertexType>> _childs;

        /// <summary>
        /// Stores the comment of the vertex type.
        /// </summary>
        private readonly Lazy<string> _comment;

        /// <summary>
        /// Stores the unique definitions of this vertex type.
        /// </summary>
        private readonly Lazy<IEnumerable<IUniqueDefinition>> _uniques;

        /// <summary>
        /// Stores the index definitions of this vertex type.
        /// </summary>
        private readonly Lazy<IEnumerable<IIndexDefinition>> _indices;

        /// <summary>
        /// Stores the ID of this vertex type.
        /// </summary>
        private readonly long _id;

        /// <summary>
        /// Stores the name of this vertex type.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Stores whether this vertex type can be a parent vertex type.
        /// </summary>
        private readonly bool _isSealed;

        /// <summary>
        /// Stores whether this vertex type can have vertices.
        /// </summary>
        private readonly bool _isAbstract;

        /// <summary>
        /// Stores whether this vertex type has child vertex types.
        /// </summary>
        private readonly bool _hasChilds;

        /// <summary>
        /// Stores whether incoming edges were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this vertex type has incoming edges.
        /// </summary>
        private bool? _hasIncomingEdges;

        /// <summary>
        /// Stores whether outgoing edges were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this vertex type has outgoing edges.
        /// </summary>
        private bool? _hasOutgoingEdges;

        /// <summary>
        /// Stores whether properties were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this vertex type has properties.
        /// </summary>
        private bool? _hasProperties;

        /// <summary>
        /// Stores whether binary properties were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this vertex type has binary properties.
        /// </summary>
        private bool? _hasBinaryProperties;

        /// <summary>
        /// Stores whether attributes were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this vertex type has attributes.
        /// </summary>
        private bool? _hasAttributes;

        /// <summary>
        /// Stores whether this type is user defined.
        /// </summary>
        private bool _IsUserDefined;

        #endregion

        #region c'tor

        /// <summary>
        /// Creates a new instance of VertexType.
        /// </summary>
        /// <param name="myVertex">An IVertex that represents the vertex type.</param>
        internal VertexType(IVertex myVertex)
        {
            #region checks

            myVertex.CheckNull("myVertex");

            #endregion

            #region set data

            _vertex = myVertex;

            #endregion

            #region set lazy stuff

            _parent = new Lazy<IVertexType>(GetParentType);

            _childs = new Lazy<List<IVertexType>>(GetChildTypes);

            _attributes = new Lazy<Dictionary<String, IAttributeDefinition>>(GetAttributes);

            _comment = new Lazy<String>(GetComment);

            _uniques = new Lazy<IEnumerable<IUniqueDefinition>>(GetUniques);

            _indices = new Lazy<IEnumerable<IIndexDefinition>>(GetIndices);

            #endregion

            #region set direct stuff

            _id = GetID();
            _name = GetName();
            _isSealed = GetIsSealed();
            _isAbstract = GetIsAbstract();
            _hasChilds = GetHasChilds();
            _IsUserDefined = GetIsUserDefined();
            #endregion

        }

        #endregion

        #region IBaseType Members

        bool IBaseType.IsUserDefined
        {
            get { return _IsUserDefined; }
        }

        #endregion

        #region IVertexType Members

        #region Inheritance

        IVertexType IVertexType.GetParentVertexType
        {
            get { return _parent.Value; }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes(bool myRecursive = true)
        {
            foreach (var aChildVertexType in _childs.Value)
            {
                yield return aChildVertexType;

                if (myRecursive)
                {
                    foreach (var aVertex in aChildVertexType.GetChildVertexTypes(myRecursive))
                    {
                        yield return aVertex;
                    }
                }
            }

            yield break;
        }

        #endregion

        #region Binary Properties

        bool IVertexType.HasBinaryProperty(string myEdgeName)
        {
            return GetAttributeAsBinaryProperty(myEdgeName) != null;
        }

        IBinaryPropertyDefinition IVertexType.GetBinaryPropertyDefinition(string myEdgeName)
        {
            return GetAttributeAsBinaryProperty(myEdgeName);
        }

        bool IVertexType.HasBinaryProperties(bool myIncludeAncestorDefinitions)
        {
            if (!_hasBinaryProperties.HasValue)
            {
                _hasBinaryProperties = GetHasBinaryProperties();
            }

            return _hasBinaryProperties.Value || (myIncludeAncestorDefinitions && _parent.Value.HasBinaryProperties(true));
        }

        IEnumerable<IBinaryPropertyDefinition> IVertexType.GetBinaryProperties(bool myIncludeAncestorDefinitions)
        {
            return _attributes.Value.OfType<IBinaryPropertyDefinition>();
        }

        #endregion

        #region Incoming Edge

        bool IVertexType.HasIncomingEdge(string myEdgeName)
        {
            return GetAttributeAsIncomingEdge(myEdgeName) != null;
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            return GetAttributeAsIncomingEdge(myEdgeName);
        }

        bool IVertexType.HasIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            if (!_hasIncomingEdges.HasValue)
            {
                _hasIncomingEdges = GetHasIncomingEdges();
            }

            return _hasIncomingEdges.Value || (myIncludeAncestorDefinitions && _parent.Value.HasIncomingEdges(true));
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return _attributes.Value.OfType<IIncomingEdgeDefinition>();
        }

        #endregion

        #region Outgoing Edge

        bool IVertexType.HasOutgoingEdge(string myEdgeName)
        {
            return GetAttributeAsOutgoingEdge(myEdgeName) != null;
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return GetAttributeAsOutgoingEdge(myEdgeName);
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeAncestorDefinitions)
        {
            if (!_hasOutgoingEdges.HasValue)
            {
                _hasOutgoingEdges = GetHasOutgoingEdges();
            }

            return _hasOutgoingEdges.Value || (myIncludeAncestorDefinitions && _parent.Value.HasOutgoingEdges(true));
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return _attributes.Value.OfType<IOutgoingEdgeDefinition>();
        }

        #endregion

        #region Index

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions)
                ? _uniques.Value.Union(_parent.Value.GetUniqueDefinitions(true))
                : _uniques.Value;
        }

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions)
                ? _indices.Value.Union(_parent.Value.GetIndexDefinitions(true))
                : _indices.Value;
        }

        #endregion

        #endregion

        #region IBaseType Members

        long IBaseType.ID
        {
            get { return _id; }
        }

        string IBaseType.Name
        {
            get { return _name; }
        }

        IBehaviour IBaseType.Behaviour
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IBaseType.Comment
        {
            get { return _comment.Value; }
        }

        bool IBaseType.IsAbstract
        {
            get { return _isAbstract; }
        }

        bool IBaseType.IsSealed
        {
            get { return _isSealed; }
        }

        #region Inheritance

        bool IBaseType.HasParentType
        {
            get { return _id != (long)BaseTypes.Vertex; }
        }

        bool IBaseType.HasChildTypes
        {
            get { return _hasChilds; }
        }


        #endregion

        #region Attributes

        bool IBaseType.HasAttribute(string myAttributeName)
        {
            return GetAttribute(myAttributeName) != null;
        }

        bool IBaseType.HasAttributes(bool myIncludeAncestorDefinitions)
        {
            if (!_hasAttributes.HasValue)
            {
                _hasAttributes = GetHasAttributes();
            }

            //Perf: Use of "short-circuit" evaluation. Do not exchange operators!
            return _hasAttributes.Value || (myIncludeAncestorDefinitions && _parent.Value.HasAttributes(true));
        }

        IAttributeDefinition IBaseType.GetAttributeDefinition(string myAttributeName)
        {
            return GetAttribute(myAttributeName);
        }

        IEnumerable<IAttributeDefinition> IBaseType.GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions)
                ? _attributes.Value.Values.Union(_parent.Value.GetAttributeDefinitions(true))
                : _attributes.Value.Values;
        }

        IAttributeDefinition IBaseType.GetAttributeDefinition(long myAttributeID)
        {
            return GetAttribute(myAttributeID);
        }

        #endregion

        #region Property

        bool IBaseType.HasProperty(string myPropertyName)
        {
            return GetAttributeAsProperty(myPropertyName) != null;
        }

        IPropertyDefinition IBaseType.GetPropertyDefinition(string myPropertyName)
        {
            return GetAttributeAsProperty(myPropertyName);
        }

        bool IBaseType.HasProperties(bool myIncludeAncestorDefinitions)
        {
            if (!_hasProperties.HasValue)
            {
                _hasProperties = GetHasProperties();
            }

            return _hasProperties.Value || (myIncludeAncestorDefinitions && _parent.Value.HasProperties(true));
        }

        IEnumerable<IPropertyDefinition> IBaseType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return _attributes.Value.OfType<IPropertyDefinition>();
        }

        IPropertyDefinition IBaseType.GetPropertyDefinition(long myPropertyID)
        {
            return GetAttribute(myPropertyID) as IPropertyDefinition;
        }

        IEnumerable<IPropertyDefinition> IBaseType.GetPropertyDefinitions(IEnumerable<string> myPropertyNames)
        {
            return myPropertyNames.Select(x => GetAttributeAsProperty(x));
        }

        #endregion

        #endregion

        #region private members

        #region GetVertex

        /// <summary>
        /// Gets the IVertex that represents this vertex type.
        /// </summary>
        /// <returns>An IVertex that represents this vertex type.</returns>
        private IVertex GetVertex()
        {
            return _vertex;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Transforms an IVertex in an attribute definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an attribute definition (Either property, incoming edge or outgoing edge).</param>
        /// <returns>An attribute definition.</returns>
        private IAttributeDefinition CreateAttributeDefinition(IVertex myVertex)
        {
            switch ((BaseTypes)myVertex.VertexTypeID)
            {
                case BaseTypes.IncomingEdge:
                    return BaseGraphStorageManager.CreateIncomingEdgeDefinition(myVertex);
                case BaseTypes.OutgoingEdge:
                    return BaseGraphStorageManager.CreateOutgoingEdgeDefinition(myVertex);
                case BaseTypes.Property:
                    return BaseGraphStorageManager.CreatePropertyDefinition(myVertex);
                default:
                    throw new UnknownDBException("The vertex does not represents an attribute");
            }
        }

        /// <summary>
        /// Creates a list of attribute definitions of the vertex type indexed by the attribute name.
        /// </summary>
        /// <returns>A dictionary of attribute name to attribute definition.</returns>
        private Dictionary<String, IAttributeDefinition> GetAttributes()
        {
            var vertices = GetVertex().GetIncomingVertices((long)BaseTypes.Attribute, (long)AttributeDefinitions.DefiningType);

            Dictionary<String, IAttributeDefinition> result = (vertices is ICollection)
                ? new Dictionary<string, IAttributeDefinition>((vertices as ICollection).Count)
                : new Dictionary<String, IAttributeDefinition>(ExpectedAttributes);

            foreach (var vertex in vertices)
            {
                if (vertex == null)
                    throw new UnknownDBException("An element in attributes list is NULL.");

                IAttributeDefinition def = CreateAttributeDefinition(vertex);
                result.Add(def.Name, def);
            }

            return result;
        }

        /// <summary>
        /// Gets whether this vertex type has attributes.
        /// </summary>
        /// <returns>True, if this vertex type has attribute otherwise false.</returns>
        private bool GetHasAttributes()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.Attribute, (long)AttributeDefinitions.DefiningType);
        }

        /// <summary>
        /// Gets the attribute with the given attribute name, if existing.
        /// </summary>
        /// <param name="myAttributeName">The attribute name of the attribute.</param>
        /// <returns>An attribute definition of the attribute with the given attribute name, if existing otherwise <c>NULL</c>.</returns>
        private IAttributeDefinition GetAttribute(string myAttributeName)
        {
            IAttributeDefinition result;
            _attributes.Value.TryGetValue(myAttributeName, out result);
            return result;
        }

        private IAttributeDefinition GetAttribute(long myAttributeID)
        {
            return _attributes.Value.Values.FirstOrDefault(x => x.AttributeID == myAttributeID);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether this vertex type has properties.
        /// </summary>
        /// <returns>True, if this vertex type has properties otherwise false.</returns>
        private bool GetHasProperties()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.Property, (long)AttributeDefinitions.DefiningType);
        }

        /// <summary>
        /// Gets the property with the given attribute name, if existing.
        /// </summary>
        /// <param name="myPropertyName">The attribute name of the property.</param>
        /// <returns>A property definition of the property with the given attribute name, if existing otherwise <c>NULL</c>.</returns>
        private IPropertyDefinition GetAttributeAsProperty(string myPropertyName)
        {
            return GetAttribute(myPropertyName) as IPropertyDefinition;
        }

        #endregion

        #region Binary Properties

        /// <summary>
        /// Gets whether this vertex type has binary properties.
        /// </summary>
        /// <returns>True, if this vertex type has binary properties otherwise false.</returns>
        private bool GetHasBinaryProperties()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.BinaryProperty, (long)AttributeDefinitions.DefiningType);
        }

        /// <summary>
        /// Gets the binary property with the given attribute name, if existing.
        /// </summary>
        /// <param name="myPropertyName">The attribute name of the binary property.</param>
        /// <returns>A binary property definition of the property with the given attribute name, if existing otherwise <c>NULL</c>.</returns>
        private IBinaryPropertyDefinition GetAttributeAsBinaryProperty(string myPropertyName)
        {
            return GetAttribute(myPropertyName) as IBinaryPropertyDefinition;
        }


        #endregion

        #region Incoming Edges

        /// <summary>
        /// Gets whether this vertex type has incoming edges.
        /// </summary>
        /// <returns>True, if this vertex type has incoming edges otherwise false.</returns>
        private bool GetHasIncomingEdges()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.DefiningType);
        }

        /// <summary>
        /// Gets the incoming edge with the given attribute name, if existing.
        /// </summary>
        /// <param name="myPropertyName">The attribute name of the incoming edge.</param>
        /// <returns>An incoming edge definition of the incoming edge with the given attribute name, if existing otherwise <c>NULL</c>.</returns>
        private IIncomingEdgeDefinition GetAttributeAsIncomingEdge(string myEdgeName)
        {
            return GetAttribute(myEdgeName) as IIncomingEdgeDefinition;
        }

        #endregion

        #region Outgoing Edges

        /// <summary>
        /// Transforms an IVertex in an outgoing edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an outgoing edge definition.</param>
        /// <returns>An outgoing edge definition.</returns>
        private IOutgoingEdgeDefinition CreateOutgoingEdgeDefinition(IVertex myOutgoingEdgeVertex)
        {
            return BaseGraphStorageManager.CreateOutgoingEdgeDefinition(myOutgoingEdgeVertex);
        }

        /// <summary>
        /// Gets whether this vertex type has outgoing edges.
        /// </summary>
        /// <returns>True, if this vertex type has outgoing edges otherwise false.</returns>
        private bool GetHasOutgoingEdges()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.OutgoingEdge, (long)AttributeDefinitions.DefiningType);
        }

        /// <summary>
        /// Gets the outgoing edge with the given attribute name, if existing.
        /// </summary>
        /// <param name="myEdgeName">The attribute name of the outgoing edge.</param>
        /// <returns>An outgoing edge definition of the outgoing edge with the given attribute name, if existing otherwise <c>NULL</c>.</returns>
        private IOutgoingEdgeDefinition GetAttributeAsOutgoingEdge(string myEdgeName)
        {
            return GetAttribute(myEdgeName) as IOutgoingEdgeDefinition;
        }

        #endregion

        #region Inheritance

        /// <summary>
        /// Gets whether this vertex type has child vertex types.
        /// </summary>
        /// <returns>True if this vertex type has child vertex types.</returns>
        private bool GetHasChilds()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.VertexType, (long)AttributeDefinitions.Parent);
        }

        /// <summary>
        /// Creates the parent vertex type of this vertex type.
        /// </summary>
        /// <returns>A new instance of the parent vertex type of this vertex type.</returns>
        private IVertexType GetParentType()
        {
            var vertex = GetParent();
            return (vertex == null) ? null : new VertexType(vertex);
        }

        /// <summary>
        /// Gets the vertex that represents the parent vertex type.
        /// </summary>
        /// <returns>An IVertex that represents the parent vertex type, if existing otherwise <c>NULL</c>.</returns>
        private IVertex GetParent()
        {
            return GetVertex().GetOutgoingSingleEdge((long)AttributeDefinitions.Parent).GetTargetVertex();
        }

        /// <summary>
        /// Creates the list of child vertex types of this vertex type.
        /// </summary>
        /// <returns>A possible empty list of child vertex types.</returns>
        private List<IVertexType> GetChildTypes()
        {
            var vertices = GetVertex().GetIncomingVertices((long)BaseTypes.VertexType, (long)AttributeDefinitions.Parent);

            //Perf: initialize the result list with a size
            List<IVertexType> result = (vertices is ICollection)
                ? new List<IVertexType>((vertices as ICollection).Count)
                : new List<IVertexType>(ExpectedChildTypes);

            result.AddRange(vertices.Select(vertex => new VertexType(vertex)));

            return result;
        }

        #endregion

        #region Index

        private IEnumerable<IUniqueDefinition> GetUniques()
        {
            if (GetVertex().HasOutgoingEdge((long)AttributeDefinitions.UniquenessDefinitions))
            {
                var edge = GetVertex().GetOutgoingHyperEdge((long)AttributeDefinitions.UniquenessDefinitions);
                var vertices = edge.GetTargetVertices();
                var indices = vertices.Select(x => BaseGraphStorageManager.CreateIndexDefinition(x, this)).ToArray();
            }
            return null;
        }


        private IEnumerable<IIndexDefinition> GetIndices()
        {
            if (GetVertex().HasIncomingVertices((long)BaseTypes.Index, (long)AttributeDefinitions.InIndices))
            {
                var vertices = GetVertex().GetIncomingVertices((long)BaseTypes.Index, (long)AttributeDefinitions.InIndices);
                var indices = vertices.Select(x => BaseGraphStorageManager.CreateIndexDefinition(x, this)).ToArray();
            }
            return null;
        }


        #endregion

        #region Vertex type

        private long GetID()
        {
            return BaseGraphStorageManager.GetID(GetVertex());
        }

        private string GetName()
        {
            return BaseGraphStorageManager.GetName(GetVertex());
        }

        private String GetComment()
        {
            return BaseGraphStorageManager.GetComment(GetVertex());
        }

        private bool GetIsSealed()
        {
            return GetVertex().GetProperty<bool>((long)AttributeDefinitions.IsSealed);
        }

        private bool GetIsAbstract()
        {
            return GetVertex().GetProperty<bool>((long)AttributeDefinitions.IsAbstract);
        }

        private bool GetIsUserDefined()
        {
            return BaseGraphStorageManager.GetIsUserDefined(GetVertex());
        }


        #endregion

        #endregion

    }
}
