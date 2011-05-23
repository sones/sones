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
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeManagement.Base;
using sones.Library.LanguageExtensions;

namespace sones.GraphDB.TypeManagement
{
    internal abstract class BaseType: IBaseType
    {
        protected Object LockObject = new Object();

        #region Data

        /// <summary>
        /// Stores the FS vertex.
        /// </summary>
        protected readonly IVertex Vertex;

        /// <summary>
        /// The attributes indexed by name.
        /// </summary>
        protected IDictionary<String, IAttributeDefinition> Attributes;

        private readonly long _id;
        private readonly string _name;
        private readonly bool _isSealed;
        private readonly bool _isAbstract;
        private readonly bool _isUserDefined;

        #endregion

        #region c'tor

        protected BaseType(IVertex myBaseTypeVertex)
        {
            #region checks

            myBaseTypeVertex.CheckNull("myVertex");

            #endregion

            #region assignment

            Vertex = myBaseTypeVertex;
            _id = Vertex.VertexID;
            _name = GetProperty<String>(AttributeDefinitions.BaseTypeDotName);
            _isSealed = GetProperty<bool>(AttributeDefinitions.BaseTypeDotIsSealed);
            _isAbstract = GetProperty<bool>(AttributeDefinitions.BaseTypeDotIsAbstract);
            _isUserDefined = GetProperty<bool>(AttributeDefinitions.BaseTypeDotIsUserDefined);

            #endregion
        }

        #endregion


        #region abstract methods

        public abstract bool HasParentType{ get; }

        public abstract bool HasChildTypes{ get; }

        protected abstract BaseType GetParentType();

        protected abstract IDictionary<String, IAttributeDefinition> RetrieveAttributes();


        #endregion

        #region IBaseType Members

        public long ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IBehaviour Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        public string Comment
        {
            get { return Vertex.Comment; }
        }

        public bool IsAbstract
        {
            get { return _isAbstract; }
        }

        public bool IsUserDefined
        {
            get { return _isUserDefined; }
        }

        public bool IsSealed
        {
            get { return _isSealed; }
        }
        public bool HasAttribute(string myAttributeName)
        {
            var result = GetAttributesPrivate().ContainsKey(myAttributeName);
            if (!result && HasParentType)
                result = GetParentType().HasAttribute(myAttributeName);

            return result;
        }

        public IAttributeDefinition GetAttributeDefinition(string myAttributeName)
        {
            IAttributeDefinition result;
            if (GetAttributesPrivate().TryGetValue(myAttributeName, out result) || !HasParentType)
                return result;


            return GetParentType().GetAttributeDefinition(myAttributeName);
        }

        public IAttributeDefinition GetAttributeDefinition(long myAttributeID)
        {
            var result = GetAttributesPrivate().Values.FirstOrDefault(_=>_.ID == myAttributeID);

            if (result == null)
                if (GetParentType() != null)
                    result = GetParentType().GetAttributeDefinition(myAttributeID);

            return result;
        }

        public bool HasAttributes(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions && HasParentType)
                ? GetAttributesPrivate().Count > 0 || GetParentType().HasAttributes(true)
                : GetAttributesPrivate().Count > 0 ;
        }

        public IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            return (myIncludeAncestorDefinitions && HasParentType)
                ? GetAttributesPrivate().Values.Union(GetParentType().GetAttributeDefinitions(true))
                : GetAttributesPrivate().Values;
        }

        public bool HasProperty(string myAttributeName)
        {
            return HasTypedAttribute<IPropertyDefinition>(myAttributeName);
        }

        public IPropertyDefinition GetPropertyDefinition(string myPropertyName)
        {
            return GetTypedAttributeDefinition<IPropertyDefinition>(myPropertyName);
        }

        public IPropertyDefinition GetPropertyDefinition(long myPropertyID)
        {
            return GetTypedAttributeDefinition<IPropertyDefinition>(myPropertyID);
        }

        public bool HasProperties(bool myIncludeAncestorDefinitions)
        {
            return HasTypedAttributes<IPropertyDefinition>(myIncludeAncestorDefinitions);
        }

        public IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            return GetTypedAttributeDefinitions<IPropertyDefinition>(myIncludeAncestorDefinitions);
        }

        public IEnumerable<IPropertyDefinition> GetPropertyDefinitions(IEnumerable<string> myPropertyNames)
        {
            myPropertyNames.CheckNull("myPropertyNames");

            return myPropertyNames.Select(GetPropertyDefinition);
        }

        public bool IsAncestor(IBaseType myOtherType)
        {
            return myOtherType != null && myOtherType.IsDescendant(this);
        }

        public bool IsAncestorOrSelf(IBaseType myOtherType)
        {
            return Equals(myOtherType) || IsAncestor(myOtherType);
        }

        public bool IsDescendant(IBaseType myOtherType)
        {
            for (var current = GetParentType(); current != null; current = current.GetParentType())
            {
                if (current.Equals(myOtherType))
                    return true;
            }
            return false;
        }

        public bool IsDescendantOrSelf(IBaseType myOtherType)
        {
            return Equals(myOtherType) || IsDescendant(myOtherType);
        }

        #endregion

        #region IEquatable<IBaseType> Members

        public bool Equals(IBaseType other)
        {
            return (other != null) && _id == other.ID;
        }

        #endregion

        #region protected methods

        protected T GetProperty<T>(AttributeDefinitions myDefinition)
        {
            return Vertex.GetProperty<T>((long)myDefinition);
        }

        protected bool HasOutgoingEdge(AttributeDefinitions myDefinition)
        {
            return Vertex.HasOutgoingEdge((long)myDefinition);
        }

        protected ISingleEdge GetOutgoingSingleEdge(AttributeDefinitions myDefinition)
        {
            return Vertex.GetOutgoingSingleEdge((long)myDefinition);
        }

        protected IHyperEdge GetOutgoingHyperEdge(AttributeDefinitions myDefinition)
        {
            return Vertex.GetOutgoingHyperEdge((long)myDefinition);
        }

        protected IEnumerable<IVertex> GetIncomingVertices(BaseTypes myBaseType, AttributeDefinitions myDefinition)
        {
            return Vertex.GetIncomingVertices((long)myBaseType, (long)myDefinition);
        }

        protected bool HasIncomingVertices(BaseTypes myBaseType, AttributeDefinitions myDefinition)
        {
            return Vertex.HasIncomingVertices((long)myBaseType, (long)myDefinition);
        }

        protected bool HasTypedAttribute<T>(string myAttributeName)
        {
            return HasAttribute(myAttributeName) && GetAttributeDefinition(myAttributeName) is T;
        }

        protected T GetTypedAttributeDefinition<T>(string myPropertyName)
            where T: class
        {
            return GetAttributeDefinition(myPropertyName) as T;
        }

        protected T GetTypedAttributeDefinition<T>(long myPropertyID)
            where T: class
        {
            return GetAttributeDefinition(myPropertyID) as T;
        }

        protected IEnumerable<T> GetTypedAttributeDefinitions<T>(bool myIncludeAncestorDefinitions)
        {
            return GetAttributeDefinitions(myIncludeAncestorDefinitions).OfType<T>();
        }

        protected bool HasTypedAttributes<T>(bool myIncludeAncestorDefinitions)
        {
            var hasOwnProperties = GetAttributesPrivate().Values.Where(_=>_ is T).CountIsGreater(0);
            
            return (myIncludeAncestorDefinitions)
                ? hasOwnProperties || GetParentType().GetAttributeDefinitions(true).Where(_=>_ is T).CountIsGreater(0)
                : hasOwnProperties;
        }
        #endregion

        private IDictionary<String, IAttributeDefinition> GetAttributesPrivate()
        {
            if (Attributes == null)
                lock (LockObject)
                {
                    if (Attributes == null)
                        Attributes = RetrieveAttributes();
                }

            return Attributes;
        }
        
    }
}
