using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;

namespace sones.GraphDS.GraphDSRemoteClient.TypeManagement
{
    internal class RemoteIndexDefinition : IIndexDefinition
    {
        #region Data

        private IServiceToken _ServiceToken;
        private List<String> _IndexedProperties;

        #endregion


        #region Constructor

        internal RemoteIndexDefinition(ServiceIndexDefinition mySvcIndexDefinition, IServiceToken myServiceToken)
        {
            this._ServiceToken = myServiceToken;
            this.Name = mySvcIndexDefinition.Name;
            this.IndexTypeName = mySvcIndexDefinition.IndexTypeName;
            this.Edition = mySvcIndexDefinition.Edition;
            this.IsUserdefined = mySvcIndexDefinition.IsUserdefined;
            this._IndexedProperties = mySvcIndexDefinition.IndexedProperties;
            this.VertexType = new RemoteVertexType(mySvcIndexDefinition.VertexType, myServiceToken);
            this.ID = mySvcIndexDefinition.ID;
            this.IsRange = mySvcIndexDefinition.IsRange;
            this.IsVersioned = mySvcIndexDefinition.IsVersioned;
            this.SourceIndex = (mySvcIndexDefinition.SourceIndex == null) ? null : new RemoteIndexDefinition(mySvcIndexDefinition.SourceIndex, myServiceToken);
        }

        #endregion

        #region IIndexDefinition Members

        public string Name { get; internal set; }

        public string IndexTypeName { get; internal set; }

        public string Edition { get; internal set; }

        public bool IsUserdefined { get; internal set; }

        public IList<IPropertyDefinition> IndexedProperties
        {
            get
            {
                return _ServiceToken.VertexTypeService.GetPropertyDefinitionsByNameListByVertexType(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, VertexType.Name, _IndexedProperties).Select(x => (IPropertyDefinition)new RemotePropertyDefinition(x, _ServiceToken)).ToList();
            }
            internal set
            {
                _IndexedProperties = value.Select(x => x.Name).ToList();
            }
        }

        public IVertexType VertexType { get; internal set; }

        public long ID { get; internal set; }

        public bool IsRange { get; internal set; }

        public bool IsVersioned { get; internal set; }

        public IIndexDefinition SourceIndex { get; internal set; }

        #endregion
    }
}
