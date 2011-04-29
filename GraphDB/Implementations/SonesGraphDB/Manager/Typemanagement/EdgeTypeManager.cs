using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.Base;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IEdgeTypeManager
    {
        #region IEdgeTypeManager Members

        private class Test: IEdgeType
        {

            #region IEdgeType Members

            public bool HasParentEdgeType
            {
                get { return false; }
            }

            public IEdgeType GetParentEdgeType
            {
                get { return null; }
            }

            public bool HasChildEdgeTypes
            {
                get { return false; }
            }

            public IEnumerable<IEdgeType> GetChildEdgeTypes
            {
                get { return Enumerable.Empty<IEdgeType>(); }
            }

            public IEnumerable<IPropertyDefinition> GetProperties
            {
                get { return Enumerable.Empty<IPropertyDefinition>(); }
            }

            #endregion

            #region IBaseType Members

            public long ID
            {
                get { return (long)BaseTypes.Edge; }
            }

            public string Name
            {
                get { return BaseTypes.Edge.ToString(); }
            }

            public IBehaviour Behaviour
            {
                get { return null; }
            }

            public string Comment
            {
                get { return String.Empty; }
            }

            public bool IsAbstract
            {
                get { return false; }
            }

            public bool IsUserDefined
            {
                get { return false; }
            }

            public bool IsSealed
            {
                get { return false; }
            }

            public bool HasParentType
            {
                get { return false; }
            }

            public bool HasChildTypes
            {
                get { return false; }
            }

            public bool HasAttribute(string myAttributeName)
            {
                return false;
            }

            public IAttributeDefinition GetAttributeDefinition(string myAttributeName)
            {
                return null;
            }

            public IAttributeDefinition GetAttributeDefinition(long myAttributeID)
            {
                return null;
            }

            public bool HasAttributes(bool myIncludeAncestorDefinitions)
            {
                return false;
            }

            public IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
            {
                return Enumerable.Empty<IAttributeDefinition>();
            }

            public bool HasProperty(string myAttributeName)
            {
                return false;
            }

            public IPropertyDefinition GetPropertyDefinition(string myPropertyName)
            {
                return null;
            }

            public IPropertyDefinition GetPropertyDefinition(long myPropertyID)
            {
                return null;
            }

            public bool HasProperties(bool myIncludeAncestorDefinitions)
            {
                return false;
            }

            public IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
            {
                return Enumerable.Empty<IPropertyDefinition>();
            }

            public IEnumerable<IPropertyDefinition> GetPropertyDefinitions(IEnumerable<string> myPropertyNames)
            {
                return Enumerable.Empty<IPropertyDefinition>();
            }

            #endregion
        }
        public IEdgeType GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return new Test();
        }

        public IEdgeType GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return new Test();
        }

        public IEnumerable<IEdgeType> GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanAddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public IEdgeType AddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanAddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public IEdgeType AddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveEdgeType(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdgeType(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanUpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanUpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStorageUsingManager Members

        public void Load(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public void Create(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
