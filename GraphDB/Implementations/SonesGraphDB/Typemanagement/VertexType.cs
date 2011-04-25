using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.Library.LanguageExtensions;
using sones.GraphDB.ErrorHandling;

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
        /// Stores whether attributes were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this vertex type has attributes.
        /// </summary>
        private bool? _hasAttributes;

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

            #endregion

        }

        #endregion

        #region IVertexType Members

        #region Vertex type properties

        long IVertexType.ID
        {
            get { return _id; }
        }

        string IVertexType.Name
        {
            get { return _name; }
        }

        IBehaviour IVertexType.Behaviour
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        string IVertexType.Comment
        {
            get { return _comment.Value; }
        }

        bool IVertexType.IsAbstract
        {
            get { return _isAbstract; }
        }

        bool IVertexType.IsSealed
        {
            get { return _isSealed; }
        }

        #endregion

        #region Inheritance

        bool IVertexType.HasParentVertexType
        {

            get { return _id != (long)BaseTypes.Vertex; }
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get { return _parent.Value; }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { return _hasChilds; }
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

        #region Attributes

        bool IVertexType.HasAttribute(string myAttributeName)
        {
            return GetAttribute(myAttributeName) != null;
        }

        IAttributeDefinition IVertexType.GetAttributeDefinition(string myAttributeName)
        {
            return GetAttribute(myAttributeName);
        }

        bool IVertexType.HasAttributes(bool myIncludeAncestorDefinitions)
        {
            if (!_hasAttributes.HasValue)
            {
                _hasAttributes = GetHasAttributes();
            }

            //Perf: Use of "short-circuit" evaluation. Do not exchange operators!
            return _hasAttributes.Value || (myIncludeAncestorDefinitions && _parent.Value.HasAttributes(true));
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions)
                ? _attributes.Value.Values.Union(_parent.Value.GetAttributeDefinitions(true))
                : _attributes.Value.Values;
        }

        public IAttributeDefinition GetAttributeDefinition(long myAttributeID)
        {
            return _attributes.Value.Values.FirstOrDefault(x => x.AttributeID == myAttributeID);
        }

        #endregion

        #region Property

        bool IVertexType.HasProperty(string myPropertyName)
        {
            return GetAttributeAsProperty(myPropertyName) != null;
        }

        IPropertyDefinition IVertexType.GetPropertyDefinition(string myPropertyName)
        {
            return GetAttributeAsProperty(myPropertyName);
        }

        bool IVertexType.HasProperties(bool myIncludeAncestorDefinitions)
        {
            if (!_hasProperties.HasValue)
            {
                _hasProperties = GetHasProperties();
            }

            return _hasProperties.Value || (myIncludeAncestorDefinitions && _parent.Value.HasProperties(true));
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return _attributes.Value.OfType<IPropertyDefinition>();
        }

        public IPropertyDefinition GetPropertyDefinition(long myPropertyID)
        {
            return GetAttributeDefinition(myPropertyID) as IPropertyDefinition;
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
                    return CreateIncomingEdgeDefinition(myVertex);
                case BaseTypes.OutgoingEdge:
                    return CreateOutgoingEdgeDefinition(myVertex);
                case BaseTypes.Property:
                    return CreatePropertyDefinition(myVertex);
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
        /// Transforms an IVertex in a property definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents a property definition.</param>
        /// <returns>A property definition.</returns>
        private static IPropertyDefinition CreatePropertyDefinition(IVertex myVertex)
        {
            var attributeID = GetAttributeID(myVertex);
            var baseType = GetBaseType(myVertex);
            var isMandatory = GetIsMandatory(myVertex);
            var multiplicity = GetMultiplicity(myVertex);
            var name = GetName(myVertex);

            return new PropertyDefinition
            {
                AttributeID = attributeID,
                BaseType = baseType,
                IsMandatory = isMandatory,
                Multiplicity = multiplicity,
                Name = name
            };
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

        #region Incoming Edges

        /// <summary>
        /// Transforms an IVertex in an incoming edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an incoming edge definition.</param>
        /// <returns>An incoming edge definition.</returns>
        private IIncomingEdgeDefinition CreateIncomingEdgeDefinition(IVertex myVertex)
        {
            var attributeID = GetAttributeID(myVertex);
            var name = GetName(myVertex);
            var related = GetRelatedOutgoingEdgeDefinition(myVertex);

            return new IncomingEdgeDefinition
            {
                AttributeID = attributeID,
                Name = name,
                RelatedEdgeDefinition = related
            };
        }

        /// <summary>
        /// Gets whether this vertex type has incoming edges.
        /// </summary>
        /// <returns>True, if this vertex type has incoming edges otherwise false.</returns>
        private bool GetHasIncomingEdges()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.IncomingEdge, (long)AttributeDefinitions.DefiningType);
        }

        /// <summary>
        /// Creates an outgoing edge definition from a vertex that represents an incoming edge definition.
        /// </summary>
        /// <param name="myVertex">A vertex that represents an incoming edge definition.</param>
        /// <returns>An outgoing edge definition.</returns>
        private IOutgoingEdgeDefinition GetRelatedOutgoingEdgeDefinition(IVertex myVertex)
        {
            var vertex = myVertex.GetOutgoingSingleEdge((long)AttributeDefinitions.RelatedEgde).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("Am incoming edge definition has no vertex that represents its related outgoing edge definition.");

            return CreateOutgoingEdgeDefinition(vertex);
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
            var attributeID = GetAttributeID(myOutgoingEdgeVertex);
            var edgeType = GetEdgeType(myOutgoingEdgeVertex);
            var name = GetName(myOutgoingEdgeVertex);
            var target = GetTargetVertexType(myOutgoingEdgeVertex);

            return new OutgoingEdgeDefinition
            {
                AttributeID = attributeID,
                EdgeType = edgeType,
                Name = name,
                SourceVertexType = this,
                TargetVertexType = target
            };
        }

        /// <summary>
        /// Gets the edge type of an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdge">A vertex that represents an outgoing edge.</param>
        /// <returns>The edge type of the outgoing edge.</returns>
        private IEdgeType GetEdgeType(IVertex myOutgoingEdge)
        {
            var vertex = myOutgoingEdge.GetOutgoingSingleEdge((long)AttributeDefinitions.EdgeType).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its edge type.");

            return new EdgeType(vertex);
        }

        /// <summary>
        /// Gets the target vertex of an outgoing edge.
        /// </summary>
        /// <param name="myOutgoingEdge">A vertex that represents an outgoing edge.</param>
        /// <returns>The target vertex type of the outgoing edge.</returns>
        private IVertexType GetTargetVertexType(IVertex myOutgoingEdge)
        {
            var vertex = myOutgoingEdge.GetOutgoingSingleEdge((long)AttributeDefinitions.Target).GetTargetVertex();

            if (vertex == null)
                throw new UnknownDBException("An outgoing edge has no vertex that represents its target vertex type.");

            if (vertex.VertexID == _id)
                return this;

            return new VertexType(vertex);
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

        private static IEnumerable<IIndexDefinition> GetIndices()
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<IUniqueDefinition> GetUniques()
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Vertex type

        private static PropertyMultiplicity GetMultiplicity(IVertex myVertex)
        {
            var multID = myVertex.GetProperty<Byte>((long)AttributeDefinitions.Multiplicity);

            if (!Enum.IsDefined(typeof(PropertyMultiplicity), multID))
                throw new UnknownDBException("The value for the property multiplicity is incorrect.");

            return (PropertyMultiplicity)multID;
        }

        private static bool GetIsMandatory(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>((long)AttributeDefinitions.IsMandatory);
        }

        private static Type GetBaseType(IVertex myVertex)
        {
            var typeID = myVertex.GetProperty<long>((long)AttributeDefinitions.Type);
            if (!Enum.IsDefined(typeof(BasicTypes), typeID))
                throw new NotImplementedException("User defined base types are not implemented yet");

            BasicTypes type = (BasicTypes)typeID;

            switch (type)
            {
                case BasicTypes.Boolean : return typeof(Boolean);
                case BasicTypes.Byte    : return typeof(Byte);
                case BasicTypes.Char    : return typeof(Char);
                case BasicTypes.DateTime: return typeof(DateTime);
                case BasicTypes.Double  : return typeof(Double);
                case BasicTypes.Int16   : return typeof(Int16);
                case BasicTypes.Int32   : return typeof(Int32);
                case BasicTypes.Int64   : return typeof(Int64);
                case BasicTypes.SByte   : return typeof(SByte);
                case BasicTypes.Single  : return typeof(Single);
                case BasicTypes.String  : return typeof(String);
                case BasicTypes.TimeSpan: return typeof(TimeSpan);
                case BasicTypes.UInt16  : return typeof(UInt16);
                case BasicTypes.UInt32  : return typeof(UInt32);
                case BasicTypes.UInt64  : return typeof(UInt64);
                default: throw new UnknownDBException("BasicTypes enumeration was modified, but this function was not adapted.");
            }
        }

        private static long GetAttributeID(IVertex myVertex)
        {
            return myVertex.GetProperty<long>((long)AttributeDefinitions.ID);
        }

        private long GetID()
        {
            return GetVertex().GetProperty<long>((long)AttributeDefinitions.ID);
        }

        private string GetName()
        {
            return GetName(GetVertex());
        }

        private static String GetName(IVertex myVertex)
        {
            return myVertex.GetPropertyAsString((long)AttributeDefinitions.Name);
        }

        private String GetComment()
        {
            return GetVertex().GetPropertyAsString((long)AttributeDefinitions.Comment);
        }

        private bool GetIsSealed()
        {
            return GetVertex().GetProperty<bool>((long)AttributeDefinitions.IsSealed);
        }

        private bool GetIsAbstract()
        {
            return GetVertex().GetProperty<bool>((long)AttributeDefinitions.IsAbstract);
        }

        #endregion

        #endregion
    }
}
