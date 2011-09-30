using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using GraphDSRemoteClient.TypeManagement;

namespace GraphDSRemoteClient.GraphElements
{
    internal abstract class ARemoteBaseType : IBaseType
    {
        #region Data

        protected long _ID;
        protected String _Name;
        protected String _Comment;
        protected Boolean _IsUserDefined;
        protected IServiceToken _ServiceToken;

        #endregion


        #region Constructor

        internal ARemoteBaseType(ServiceBaseType myServiceBaseType, IServiceToken myServiceToken)
        {
            _ServiceToken = myServiceToken;
            _ID = myServiceBaseType.ID;
            _Name = myServiceBaseType.Name;
            _Comment = myServiceBaseType.Comment;
            _IsUserDefined = myServiceBaseType.IsUserDefined;
        }

        #endregion


        #region abstract methods

        protected abstract ARemoteBaseType RetrieveParentType();

        protected abstract IEnumerable<ARemoteBaseType> RetrieveChildrenTypes();

        protected abstract IDictionary<String, IAttributeDefinition> RetrieveAttributes();

        #endregion


        #region IBaseType

        public long ID
        {
            get { return _ID; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public IBehaviour Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        public string Comment
        {
            get { return _Comment; }
        }

        public bool IsUserDefined
        {
            get { return _IsUserDefined; }
        }

        public abstract bool IsSealed { get; }

        public abstract bool HasParentType { get; }

        public abstract bool HasChildTypes { get; }

        public abstract bool IsAncestor(IBaseType myOtherType);

        public abstract bool IsAncestorOrSelf(IBaseType myOtherType);

        public abstract bool IsDescendant(IBaseType myOtherType);

        public abstract bool IsDescendantOrSelf(IBaseType myOtherType);

        public abstract IEnumerable<IBaseType> GetDescendantTypes();

        public abstract IEnumerable<IBaseType> GetDescendantTypesAndSelf();

        public abstract IEnumerable<IBaseType> GetAncestorTypes();

        public abstract IEnumerable<IBaseType> GetAncestorTypesAndSelf();

        public abstract IEnumerable<IBaseType> GetKinsmenTypes();

        public abstract IEnumerable<IBaseType> GetKinsmenTypesAndSelf();

        public abstract IEnumerable<IBaseType> ChildrenTypes { get; }

        public abstract IBaseType ParentType { get; }

        public abstract bool HasAttribute(string myAttributeName);

        public abstract IAttributeDefinition GetAttributeDefinition(string myAttributeName);

        public abstract IAttributeDefinition GetAttributeDefinition(long myAttributeID);

        public abstract bool HasAttributes(bool myIncludeAncestorDefinitions);

        public abstract IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestorDefinitions);

        public abstract bool HasProperty(string myAttributeName);

        public abstract IPropertyDefinition GetPropertyDefinition(string myPropertyName);

        public abstract IPropertyDefinition GetPropertyDefinition(long myPropertyID);

        public abstract bool HasProperties(bool myIncludeAncestorDefinitions);

        public abstract IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestorDefinitions);

        public abstract IEnumerable<IPropertyDefinition> GetPropertyDefinitions(IEnumerable<string> myPropertyNames);

        bool IEquatable<IBaseType>.Equals(IBaseType other)
        {
            return (other != null) && _ID == other.ID;
        }

        #endregion

    }
}
