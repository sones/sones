using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDS.GraphDSRemoteClient.TypeManagement;

namespace sones.GraphDS.GraphDSRemoteClient.GraphElements
{
	internal class RemoteEdgeType : ARemoteBaseType, IEdgeType
	{
		#region Constructor

		internal RemoteEdgeType(ServiceEdgeType myServiceEdgeType, IServiceToken myServiceToken) : base(myServiceEdgeType, myServiceToken)
		{ }

		#endregion
		

		#region ARemoteBaseType

		protected override ARemoteBaseType RetrieveParentType()
		{
			return HasParentType ? new RemoteEdgeType(_ServiceToken.EdgeTypeService.ParentEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this._Name)), _ServiceToken) : null;
		}

		protected override IEnumerable<ARemoteBaseType> RetrieveChildrenTypes()
		{
			if (!HasChildTypes)
				return Enumerable.Empty<RemoteEdgeType>();

			var vertices = _ServiceToken.EdgeTypeService.ChildrenEdgeTypes(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this));

			return vertices.Select(vertex => new RemoteEdgeType(vertex, _ServiceToken)).ToArray();
		}

        protected override IDictionary<String, IAttributeDefinition> RetrieveAttributes()
        {
            throw new NotImplementedException();
        }

		#endregion


		#region IEdgeType

		public IEnumerable<IEdgeType> GetDescendantEdgeTypes()
		{
            return _ServiceToken.EdgeTypeService.GetDescendantEdgeTypes(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this)).Select(x => new RemoteEdgeType(x, _ServiceToken));
		}

		public IEnumerable<IEdgeType> GetDescendantEdgeTypesAndSelf()
		{
            return _ServiceToken.EdgeTypeService.GetDescendantEdgeTypesAndSelf(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this)).Select(x => new RemoteEdgeType(x, _ServiceToken));
		}

        public IEnumerable<IEdgeType> GetAncestorEdgeTypes()
		{
            return _ServiceToken.EdgeTypeService.GetAncestorEdgeTypes(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this)).Select(x => new RemoteEdgeType(x, _ServiceToken));
		}

        public IEnumerable<IEdgeType> GetAncestorEdgeTypesAndSelf()
		{
            return _ServiceToken.EdgeTypeService.GetAncestorEdgeTypesAndSelf(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this)).Select(x => new RemoteEdgeType(x, _ServiceToken));
		}

        public IEnumerable<IEdgeType> GetKinsmenEdgeTypes()
		{
            return _ServiceToken.EdgeTypeService.GetKinsmenEdgeTypes(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this)).Select(x => new RemoteEdgeType(x, _ServiceToken));
		}

        public IEnumerable<IEdgeType> GetKinsmenEdgeTypesAndSelf()
		{
            return _ServiceToken.EdgeTypeService.GetKinsmenEdgeTypesAndSelf(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this)).Select(x => new RemoteEdgeType(x, _ServiceToken));
		}

        public IEnumerable<IEdgeType> ChildrenEdgeTypes
		{
			get
            {
                return RetrieveChildrenTypes().Cast<IEdgeType>();
            }
		}

        public IEdgeType ParentEdgeType
		{
			get
            {
                return (IEdgeType)RetrieveParentType();
            }
		}

		#endregion


		#region IBaseType

		public override bool IsSealed
		{
			get
            {
                return _ServiceToken.EdgeTypeService.IsSealedByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this));
            }
		}

		public override bool HasParentType
		{
			get
            {
                return _ServiceToken.EdgeTypeService.HasParentTypeByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this));
            }
		}

		public override bool HasChildTypes
		{
			get
            {
                return _ServiceToken.EdgeTypeService.HasChildTypesByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this));
            }
		}

		public override bool IsAncestor(IBaseType myOtherType)
		{
            return (myOtherType is IEdgeType) ? _ServiceToken.EdgeTypeService.IsAncestorByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), new ServiceEdgeType((IEdgeType)myOtherType)) : false;
		}

		public override bool IsAncestorOrSelf(IBaseType myOtherType)
		{
            return (myOtherType is IEdgeType) ? _ServiceToken.EdgeTypeService.IsAncestorOrSelfByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), new ServiceEdgeType((IEdgeType)myOtherType)) : false;
		}

		public override bool IsDescendant(IBaseType myOtherType)
		{
            return (myOtherType is IEdgeType) ? _ServiceToken.EdgeTypeService.IsDescendantByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), new ServiceEdgeType((IEdgeType)myOtherType)) : false;
		}

		public override bool IsDescendantOrSelf(IBaseType myOtherType)
		{
            return (myOtherType is IEdgeType) ? _ServiceToken.EdgeTypeService.IsDescendantOrSelfByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), new ServiceEdgeType((IEdgeType)myOtherType)) : false;
		}

		public override IEnumerable<IBaseType> GetDescendantTypes()
		{
            return GetDescendantEdgeTypes();
		}

		public override IEnumerable<IBaseType> GetDescendantTypesAndSelf()
		{
            return GetDescendantEdgeTypesAndSelf();
		}

		public override IEnumerable<IBaseType> GetAncestorTypes()
		{
            return GetAncestorEdgeTypes();
		}

		public override IEnumerable<IBaseType> GetAncestorTypesAndSelf()
		{
            return GetAncestorEdgeTypesAndSelf();
		}

		public override IEnumerable<IBaseType> GetKinsmenTypes()
		{
            return GetKinsmenEdgeTypes();
		}

		public override IEnumerable<IBaseType> GetKinsmenTypesAndSelf()
		{
            return GetKinsmenEdgeTypesAndSelf();
		}

		public override IEnumerable<IBaseType> ChildrenTypes
		{
			get
            {
                return RetrieveChildrenTypes();
            }
		}

		public override IBaseType ParentType
		{
			get
            {
                return RetrieveParentType();
            }
		}

		public override bool HasAttribute(string myAttributeName)
		{
            return _ServiceToken.EdgeTypeService.HasAttributeByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myAttributeName);
		}

		public override IAttributeDefinition GetAttributeDefinition(string myAttributeName)
		{
           return ConvertHelper.ToAttributeDefinition(
               _ServiceToken.EdgeTypeService.GetAttributeDefinitionByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myAttributeName),
               _ServiceToken);
		}

		public override IAttributeDefinition GetAttributeDefinition(long myAttributeID)
		{
            return ConvertHelper.ToAttributeDefinition(
                _ServiceToken.EdgeTypeService.GetAttributeDefinitionByIDByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myAttributeID),
                _ServiceToken);
		}

		public override bool HasAttributes(bool myIncludeAncestorDefinitions)
		{
            return _ServiceToken.EdgeTypeService.HasAttributesByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myIncludeAncestorDefinitions);
		}

		public override IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
		{
            return _ServiceToken.EdgeTypeService.GetAttributeDefinitionsByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myIncludeAncestorDefinitions).Select(x => ConvertHelper.ToAttributeDefinition(x, _ServiceToken));
		}

		public override bool HasProperty(string myAttributeName)
		{
            return _ServiceToken.EdgeTypeService.HasProprtyByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myAttributeName);
		}

		public override IPropertyDefinition GetPropertyDefinition(string myPropertyName)
		{
            return new RemotePropertyDefinition(
                _ServiceToken.EdgeTypeService.GetPropertyDefinitionByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, this.Name, myPropertyName),
                _ServiceToken);
		}

		public override IPropertyDefinition GetPropertyDefinition(long myPropertyID)
		{
            return new RemotePropertyDefinition(
                _ServiceToken.EdgeTypeService.GetPropertyDefinitionByIDByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, this.Name, myPropertyID),
                _ServiceToken);
		}

		public override bool HasProperties(bool myIncludeAncestorDefinitions)
		{
            return _ServiceToken.EdgeTypeService.HasPropertiesByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, new ServiceEdgeType(this), myIncludeAncestorDefinitions);
		}

		public override IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
		{
            return _ServiceToken.EdgeTypeService.GetPropertyDefinitionsByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, this.Name, myIncludeAncestorDefinitions)
                .Select(x => new RemotePropertyDefinition(x, _ServiceToken));
		}

		public override IEnumerable<IPropertyDefinition> GetPropertyDefinitions(IEnumerable<string> myPropertyNames)
		{
            return _ServiceToken.EdgeTypeService.GetPropertyDefinitionsByNameListByEdgeType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, this.Name, myPropertyNames.ToList())
                .Select(x => new RemotePropertyDefinition(x, _ServiceToken));
		}

		#endregion
	}
}
