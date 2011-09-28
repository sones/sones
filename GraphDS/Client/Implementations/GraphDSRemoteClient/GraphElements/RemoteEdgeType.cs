using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    public class RemoteEdgeType : ARemoteBaseType, IEdgeType
    {
        private EdgeTypeService _EdgeTypeService;

        internal RemoteEdgeType(ServiceEdgeType myServiceEdgeType, EdgeTypeService myEdgeTypeService) : base(myServiceEdgeType)
        {
            _EdgeTypeService = myEdgeTypeService;
        }

        public IEnumerable<IEdgeType> GetDescendantEdgeTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEdgeType> GetDescendantEdgeTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEdgeType> GetAncestorEdgeTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEdgeType> GetAncestorEdgeTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEdgeType> GetKinsmenEdgeTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEdgeType> GetKinsmenEdgeTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEdgeType> ChildrenEdgeTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IEdgeType ParentEdgeType
        {
            get { throw new NotImplementedException(); }
        }

        public bool Equals(IBaseType other)
        {
            throw new NotImplementedException();
        }

        public bool HasParentType
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasChildTypes
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAncestor(IBaseType myOtherType)
        {
            throw new NotImplementedException();
        }

        public bool IsAncestorOrSelf(IBaseType myOtherType)
        {
            throw new NotImplementedException();
        }

        public bool IsDescendant(IBaseType myOtherType)
        {
            throw new NotImplementedException();
        }

        public bool IsDescendantOrSelf(IBaseType myOtherType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> GetDescendantTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> GetDescendantTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> GetAncestorTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> GetAncestorTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> GetKinsmenTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> GetKinsmenTypesAndSelf()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseType> ChildrenTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IBaseType ParentType
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasAttribute(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        public IAttributeDefinition GetAttributeDefinition(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        public IAttributeDefinition GetAttributeDefinition(long myAttributeID)
        {
            throw new NotImplementedException();
        }

        public bool HasAttributes(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        public IPropertyDefinition GetPropertyDefinition(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public IPropertyDefinition GetPropertyDefinition(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperties(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPropertyDefinition> GetPropertyDefinitions(IEnumerable<string> myPropertyNames)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IBaseType other)
        {
            throw new NotImplementedException();
        }
    }
}
