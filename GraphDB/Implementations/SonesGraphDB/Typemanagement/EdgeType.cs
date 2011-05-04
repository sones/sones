using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.Library.LanguageExtensions;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Manager.BaseGraph;
using System.Collections;

namespace sones.GraphDB.TypeManagement
{
    internal class EdgeType: IEdgeType
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
        /// Stores the parent edge type.
        /// </summary>
        private readonly Lazy<IEdgeType> _parent;

        /// <summary>
        /// Stores the list of child edge types.
        /// </summary>
        private readonly Lazy<List<IEdgeType>> _childs;

        /// <summary>
        /// Stores the comment of the edge type.
        /// </summary>
        private readonly Lazy<string> _comment;

        /// <summary>
        /// Stores the ID of this edge type.
        /// </summary>
        private readonly long _id;

        /// <summary>
        /// Stores the name of this edge type.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Stores whether this edge type can be a parent edge type.
        /// </summary>
        private readonly bool _isSealed;

        /// <summary>
        /// Stores whether this edge type can have edges.
        /// </summary>
        private readonly bool _isAbstract;

        /// <summary>
        /// Stores whether this edge type has child edge types.
        /// </summary>
        private readonly bool _hasChilds;

        /// <summary>
        /// Stores whether properties were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this edge type has properties.
        /// </summary>
        private bool? _hasProperties;

        /// <summary>
        /// Stores whether attributes were queried (<see cref="Nullable.HasValue"/> equals true) and if true, whether this edge type has attributes.
        /// </summary>
        private bool? _hasAttributes;

        /// <summary>
        /// Stores whether this type is user defined.
        /// </summary>
        private bool _IsUserDefined;

        #endregion

        #region c'tor

        public EdgeType(IVertex myVertex)
        {
            #region checks

            myVertex.CheckNull("myVertex");

            #endregion

            #region set data

            _vertex = myVertex;

            #endregion

            #region set lazy stuff

            _parent = new Lazy<IEdgeType>(GetParentType);

            _childs = new Lazy<List<IEdgeType>>(GetChildTypes);

            _attributes = new Lazy<Dictionary<String, IAttributeDefinition>>(GetAttributes);

            _comment = new Lazy<String>(GetComment);

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

        #region private members 

        private IVertex GetVertex()
        {
            return _vertex;
        }

        private bool GetHasParentType()
        {
            return _id != (long)BaseTypes.Edge;
        }

        /// <summary>
        /// Creates the list of child edge types of this edge type.
        /// </summary>
        /// <returns>A possible empty list of child edge types.</returns>
        private List<IEdgeType> GetChildTypes()
        {
            var vertices = GetVertex().GetIncomingVertices((long)BaseTypes.EdgeType, (long)AttributeDefinitions.Parent);

            //Perf: initialize the result list with a size
            List<IEdgeType> result = (vertices is ICollection)
                ? new List<IEdgeType>((vertices as ICollection).Count)
                : new List<IEdgeType>(ExpectedChildTypes);

            result.AddRange(vertices.Select(vertex => new EdgeType(vertex)));

            return result;
        }

        /// <summary>
        /// Creates a list of attribute definitions of the vertex type indexed by the attribute name.
        /// </summary>
        /// <returns>A dictionary of attribute name to attribute definition.</returns>
        private Dictionary<String, IAttributeDefinition> GetAttributes()
        {
            return BaseGraphStorageManager.GetPropertiesFromFS(GetVertex(), this).Cast<IAttributeDefinition>().ToDictionary(x => x.Name);
        }

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

        private bool GetIsAbstract()
        {
            return BaseGraphStorageManager.GetIsAbstract(GetVertex());
        }

        /// <summary>
        /// Gets whether this edge type has child vertex types.
        /// </summary>
        /// <returns>True if this edge type has child vertex types.</returns>
        private bool GetHasChilds()
        {
            return GetVertex().HasIncomingVertices((long)BaseTypes.EdgeType, (long)AttributeDefinitions.Parent);
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
        /// Creates the parent edge type of this edge type.
        /// </summary>
        /// <returns>A new instance of the parent edge type of this edge type.</returns>
        private IEdgeType GetParentType()
        {
            var vertex = BaseGraphStorageManager.GetParent(GetVertex());
            return (vertex == null) ? null : new EdgeType(vertex);
        }

        private bool GetIsSealed()
        {
            return BaseGraphStorageManager.GetIsSealed(GetVertex());
        }

        private bool GetIsUserDefined()
        {
            return BaseGraphStorageManager.GetIsUserDefined(GetVertex());
        }

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

        /// <summary>
        /// Gets the attribute with the given attribute name, if existing.
        /// </summary>
        /// <param name="myAttributeName">The attribute name of the attribute.</param>
        /// <returns>An attribute definition of the attribute with the given attribute name, if existing otherwise <c>NULL</c>.</returns>
        private IAttributeDefinition GetAttribute(string myAttributeName)
        {
            IAttributeDefinition result;
            _attributes.Value.TryGetValue(myAttributeName, out result);
            if (result != null || !GetHasParentType())
                return result;

            return GetParentType().GetAttributeDefinition(myAttributeName);
        }

        private IAttributeDefinition GetAttribute(long myAttributeID)
        {
            var result = _attributes.Value.Values.FirstOrDefault(x => x.AttributeID == myAttributeID);
            if (result != null || !GetHasParentType())
                return result;

            return GetParentType().GetAttributeDefinition(myAttributeID);

        }



        #endregion



        #region IEdgeType Members

        IEdgeType IEdgeType.ParentEdgeType
        {
            get { return _parent.Value; }
        }

        IEnumerable<IEdgeType> IEdgeType.GetChildEdgeTypes(bool myRecursive = true, bool myIncludeSelf = false)
        {
            if (myIncludeSelf)
                yield return this;

            foreach (var aChildVertexType in _childs.Value)
            {
                yield return aChildVertexType;

                if (myRecursive)
                {
                    foreach (var aVertex in aChildVertexType.GetChildEdgeTypes(myRecursive))
                    {
                        yield return aVertex;
                    }
                }
            }

            yield break;
        }

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

        bool IBaseType.IsUserDefined
        {
            get { return _IsUserDefined; }
        }



        #region Inheritance

        bool IBaseType.HasParentType
        {
            get { return GetHasParentType(); }
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
            return (myIncludeAncestorDefinitions && GetHasParentType())
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

        #region IBaseType Members


        public bool IsAncestor(IBaseType myOtherType)
        {
            for (var current = this.GetParentType(); current != null; current = current.ParentEdgeType)
            {
                if (Equals(current, myOtherType))
                    return true;
            }
            return false;
        }

        public bool IsDescendant(IBaseType myOtherType)
        {
            if (myOtherType == null)
                return false;

            return myOtherType.IsAncestor(this);
        }

        public bool IsAncestorOrSelf(IBaseType myOtherType)
        {
            return Equals(myOtherType) || IsAncestor(myOtherType);
        }

        public bool IsDescendantOrSelf(IBaseType myOtherType)
        {
            return Equals(myOtherType) || IsDescendant(myOtherType);
        }

        #endregion


        #region IEquatable<IBaseType> Members

        public bool Equals(IBaseType other)
        {
            return (other != null) && this._id == other.ID;
        }

        #endregion

    }
}
