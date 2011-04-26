using System;
using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement
{
    internal class EdgeType: IEdgeType
    {
        private Library.PropertyHyperGraph.IVertex vertex;

        public EdgeType(Library.PropertyHyperGraph.IVertex vertex)
        {
            // TODO: Complete member initialization
            this.vertex = vertex;
        }

        #region IEdgeType Members

        bool IEdgeType.HasParentEdgeType
        {
            get { throw new NotImplementedException(); }
        }

        IEdgeType IEdgeType.GetParentEdgeType
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.HasChildEdgeTypes
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<IEdgeType> IEdgeType.GetChildEdgeTypes
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<IPropertyDefinition> IEdgeType.GetProperties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IBaseType Members

        long IBaseType.ID
        {
            get { throw new NotImplementedException(); }
        }

        string IBaseType.Name
        {
            get { throw new NotImplementedException(); }
        }

        IBehaviour IBaseType.Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        string IBaseType.Comment
        {
            get { throw new NotImplementedException(); }
        }

        bool IBaseType.IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        bool IBaseType.IsSealed
        {
            get { throw new NotImplementedException(); }
        }

        bool IBaseType.HasParentType
        {
            get { throw new NotImplementedException(); }
        }

        bool IBaseType.HasChildTypes
        {
            get { throw new NotImplementedException(); }
        }

        bool IBaseType.HasAttribute(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        IAttributeDefinition IBaseType.GetAttributeDefinition(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        IAttributeDefinition IBaseType.GetAttributeDefinition(long myAttributeID)
        {
            throw new NotImplementedException();
        }

        bool IBaseType.HasAttributes(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IAttributeDefinition> IBaseType.GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        bool IBaseType.HasProperty(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        IPropertyDefinition IBaseType.GetPropertyDefinition(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        IPropertyDefinition IBaseType.GetPropertyDefinition(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        bool IBaseType.HasProperties(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IPropertyDefinition> IBaseType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IPropertyDefinition> IBaseType.GetPropertyDefinitions(IEnumerable<string> myPropertyNames)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
