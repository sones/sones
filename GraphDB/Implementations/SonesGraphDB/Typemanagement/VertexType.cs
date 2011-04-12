using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.BaseTypes;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.TypeManagement
{
    internal class VertexType: IVertexType
    {
        #region Constants

        /// <summary>
        /// This is the initialization count of the myResult list of a GetChildVertices method
        /// </summary>
        private const int ExpectedChildTypes = 50;

        /// <summary>
        /// This is the initialization count of the myResult list of a GetChildVertices method
        /// </summary>
        private const int ExpectedAttributes = 50;

        #endregion

        #region Data

        private readonly IVertex _vertex;
        private readonly Lazy<Dictionary<String, IAttributeDefinition>> _attributes;
        private readonly Lazy<IVertexType> _parent;
        private readonly Lazy<List<IVertexType>> _childs;
        private readonly Lazy<string> _comment;
        private readonly Lazy<IEnumerable<IUniqueDefinition>> _uniques;
        private readonly Lazy<IEnumerable<IIndexDefinition>> _indices;

        private readonly long _id;
        private readonly string _name;
        private readonly bool _isSealed;
        private readonly bool _isAbstract;
        private readonly bool _hasChilds;
        private bool? _hasIncomingEdges;
        private bool? _hasOutgoingEdges;
        private bool? _hasProperties;
        private bool? _hasAttributes;

        #endregion

        #region c'tor

        internal VertexType(IVertex myVertex)
        {
            //_Expr = new BinaryExpression(new ConstantExpression(_ID), BinaryOperator.Equals, new AttributeExpression("VertexType", "ID"));
            _vertex = myVertex;

            _parent = new Lazy<IVertexType>(() => GetParentType(myVertex));

            _childs = new Lazy<List<IVertexType>>(() => GetChildTypes(myVertex));

            _attributes = new Lazy<Dictionary<String, IAttributeDefinition>>(() => GetAttributes(myVertex));

            _comment = new Lazy<String>(() => GetComment(myVertex));

            _uniques = new Lazy<IEnumerable<IUniqueDefinition>>(()=> GetUniques(myVertex));

            _indices = new Lazy<IEnumerable<IIndexDefinition>>(() => GetIndices(myVertex));

            _id = GetID(myVertex);
            _name = GetName(myVertex);
            _isSealed = GetIsSealed(myVertex);
            _isAbstract = GetIsAbstract(myVertex);
            _hasChilds = GetHasChilds(myVertex);
            
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
            // All vertices are at least inherit from Vertex
            // Vertex itself has its own implementation class
            get { return true; }
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get { return _parent.Value; }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get { return _hasChilds; }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get { return _childs.Value; }
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
                _hasAttributes = GetHasAttributes(GetVertex());
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
                _hasProperties = GetHasProperties(GetVertex());
            }

            return _hasProperties.Value || (myIncludeAncestorDefinitions && _parent.Value.HasProperties(true));
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return _attributes.Value.OfType<IPropertyDefinition>();
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
                _hasIncomingEdges = GetHasIncomingEdges(GetVertex());
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
                _hasOutgoingEdges = GetHasOutgoingEdges(GetVertex());
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

        private IVertex GetVertex()
        {
            return _vertex;
        }

        #endregion

        #region Attributes

        #region static

        private IAttributeDefinition CreateAttributeDefinition(IVertex vertex)
        {
            switch ((BaseVertexType)vertex.VertexTypeID)
            {
                case BaseVertexType.IncomingEdge:
                    return CreateIncomingEdgeDefinition(vertex);
                case BaseVertexType.OutgoingEdge:
                    return CreateOutgoingEdgeDefinition(vertex);
                case BaseVertexType.Property:
                    return CreatePropertyDefinition(vertex);
                default:
                    //TODO: better exception needed here
                    throw new Exception();
            }
        }

        private Dictionary<String, IAttributeDefinition> GetAttributes(IVertex myVertex)
        {
            var vertices = myVertex.GetIncomingVertices((long)BaseVertexType.Attribute, AttributeDefinitions.DefiningTypeOnAttribute.AttributeID);

            Dictionary<String, IAttributeDefinition> result = (vertices is ICollection)
                ? new Dictionary<string, IAttributeDefinition>((vertices as ICollection).Count)
                : new Dictionary<String, IAttributeDefinition>(ExpectedAttributes);

            foreach (var vertex in vertices)
            {
                IAttributeDefinition def = CreateAttributeDefinition(vertex);
                result.Add(def.Name, def);
            }

            return result;
        }

        private static bool GetHasAttributes(IVertex myVertex)
        {
            return myVertex.HasIncomingEdge((long)BaseVertexType.Attribute, AttributeDefinitions.DefiningTypeOnAttribute.AttributeID);
        }

        #endregion

        private IAttributeDefinition GetAttribute(string myAttributeName)
        {
            IAttributeDefinition result;
            _attributes.Value.TryGetValue(myAttributeName, out result);
            return result;
        }

        #endregion

        #region Properties

        #region static

        private static bool GetHasProperties(IVertex myVertex)
        {
            return myVertex.HasIncomingEdge((long)BaseVertexType.Property, AttributeDefinitions.DefiningTypeOnAttribute.AttributeID);
        }

        private static IPropertyDefinition CreatePropertyDefinition(IVertex myVertex)
        {
            var attributeID = GetAttributeID(myVertex);
            var baseType = GetBaseType(myVertex);
            var isMandatory = GetIsMandatory(myVertex);
            var multiplicity = GetMultiplicity(baseType);
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

        #endregion

        private IPropertyDefinition GetAttributeAsProperty(string myPropertyName)
        {
            return GetAttribute(myPropertyName) as IPropertyDefinition;
        }

        #endregion

        #region Incoming Edges

        #region static 

        private IIncomingEdgeDefinition CreateIncomingEdgeDefinition(IVertex myVertex)
        {
            var attributeID = GetAttributeID(myVertex);
            var name = GetName(myVertex);
            var related = GetRelatetOutgoingEdgeDefinition(myVertex);

            return new IncomingEdgeDefinition
            {
                AttributeID = attributeID,
                Name = name,
                RelatedEdgeDefinition = related
            };
        }

        private static bool GetHasIncomingEdges(IVertex myVertex)
        {
            return myVertex.HasIncomingEdge((long)BaseVertexType.IncomingEdge, AttributeDefinitions.DefiningTypeOnAttribute.AttributeID);
        }

        private IOutgoingEdgeDefinition GetRelatetOutgoingEdgeDefinition(IVertex myVertex)
        {
            return CreateOutgoingEdgeDefinition(myVertex.GetOutgoingSingleEdge(AttributeDefinitions.RelatedEdgeOnIncomingEdge.AttributeID).GetTargetVertex());
        }

        #endregion

        private IIncomingEdgeDefinition GetAttributeAsIncomingEdge(string myEdgeName)
        {
            return GetAttribute(myEdgeName) as IIncomingEdgeDefinition;
        }

        #endregion

        #region Outgoing Edges

        #region static 

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

        private static IEdgeType GetEdgeType(IVertex myVertex)
        {
            var vertex = myVertex.GetOutgoingSingleEdge(AttributeDefinitions.EdgeTypeOnOutgoingEdge.AttributeID).GetTargetVertex();
            return new EdgeType(vertex);
        }

        private IVertexType GetTargetVertexType(IVertex myOutgoingEdgeVertex)
        {
            var vertex = myOutgoingEdgeVertex.GetOutgoingSingleEdge(AttributeDefinitions.TargetOnOutgoingEdge.AttributeID).GetTargetVertex();
            if (vertex.VertexID == _id)
                return this;

            return new VertexType(vertex);
        }

        private static bool GetHasOutgoingEdges(IVertex myVertex)
        {
            return myVertex.HasIncomingEdge((long)BaseVertexType.OutgoingEdge, AttributeDefinitions.DefiningTypeOnAttribute.AttributeID);
        }

        #endregion

        private IOutgoingEdgeDefinition GetAttributeAsOutgoingEdge(string myEdgeName)
        {
            return GetAttribute(myEdgeName) as IOutgoingEdgeDefinition;
        }

        #endregion

        #region Inheritance

        private static bool GetHasChilds(IVertex myVertex)
        {
            return myVertex.HasIncomingEdge((long)BaseVertexType.VertexType, AttributeDefinitions.ParentOnVertexType.AttributeID);
        }

        private static IVertexType GetParentType(IVertex myVertex)
        {
            return new VertexType(GetParent(myVertex));
        }

        private static IVertex GetParent(IVertex myVertex)
        {
            return myVertex.GetOutgoingSingleEdge(AttributeDefinitions.ParentOnVertexType.AttributeID).GetTargetVertex();
        }

        private static List<IVertexType> GetChildTypes(IVertex myVertex)
        {
            var vertices = myVertex.GetIncomingVertices((long)BaseVertexType.VertexType, AttributeDefinitions.ParentOnVertexType.AttributeID);

            //Perf: initialize the myResult list with a size
            List<IVertexType> result = (vertices is ICollection)
                ? new List<IVertexType>((vertices as ICollection).Count)
                : new List<IVertexType>(ExpectedChildTypes);

            result.AddRange(vertices.Select(vertex => new VertexType(vertex)));

            return result;
        }

        #endregion

        #region Index

        private static IEnumerable<IIndexDefinition> GetIndices(IVertex myVertex)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<IUniqueDefinition> GetUniques(IVertex myVertex)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Vertex type

        private static TypesOfMultiplicity GetMultiplicity(Type myBaseType)
        {
            if (myBaseType.GetInterface("ISet", true) != null)
                return TypesOfMultiplicity.Set;

            if (myBaseType.GetInterface("IList", true) != null)
                return TypesOfMultiplicity.List;

            return TypesOfMultiplicity.Single;
        }

        private static bool GetIsMandatory(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>(AttributeDefinitions.IsMandatoryOnProperty.AttributeID);
        }

        private static Type GetBaseType(IVertex myVertex)
        {
            throw new NotImplementedException();
        }

        private static long GetAttributeID(IVertex myVertex)
        {
            return myVertex.GetProperty<long>(AttributeDefinitions.ID.AttributeID);
        }

        private static long GetID(IVertex myVertex)
        {
            return myVertex.GetProperty<long>(AttributeDefinitions.ID.AttributeID);
        }

        private static String GetName(IVertex myVertex)
        {
            return myVertex.GetPropertyAsString(AttributeDefinitions.Name.AttributeID);
        }

        private static String GetComment(IVertex myVertex)
        {
            return myVertex.GetPropertyAsString(AttributeDefinitions.Comment.AttributeID);
        }

        private static bool GetIsSealed(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>(AttributeDefinitions.IsSealedOnBaseType.AttributeID);
        }

        private static bool GetIsAbstract(IVertex myVertex)
        {
            return myVertex.GetProperty<bool>(AttributeDefinitions.IsAbstractOnBaseType.AttributeID);
        }

        #endregion

        #endregion

    }
}
