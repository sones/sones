using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;

namespace sones.GraphDS.GraphDSRemoteClient.TypeManagement
{
    internal class RemotePropertyDefinition : IPropertyDefinition
    {
        #region Data

        private IServiceToken _ServiceToken;
        private List<String> _InIndices;

        #endregion

        internal RemotePropertyDefinition(ServicePropertyDefinition mySvcPropertyDefinition, IServiceToken myServiceToken)
        {
            _ServiceToken = myServiceToken;
            this.IsMandatory = mySvcPropertyDefinition.IsMandatory;
            this.IsUserDefined = mySvcPropertyDefinition.IsUserDefined;
            this.IsUserDefinedType = mySvcPropertyDefinition.IsUserDefinedType;
            this.BaseType = Type.GetType(mySvcPropertyDefinition.BaseType);
            switch (mySvcPropertyDefinition.Multiplicity)
            {
                case ServicePropertyMultiplicity.Single:
                    this.Multiplicity = PropertyMultiplicity.Single;
                    break;
                case ServicePropertyMultiplicity.Set:
                    this.Multiplicity = PropertyMultiplicity.Set;
                    break;
                case ServicePropertyMultiplicity.List:
                    this.Multiplicity = PropertyMultiplicity.List;
                    break;
            }
            this._InIndices = mySvcPropertyDefinition.InIndices;
            this.DefaultValue = (IComparable)mySvcPropertyDefinition.DefaultValue;
            this.ID = mySvcPropertyDefinition.ID;
            this.Name = mySvcPropertyDefinition.Name;
            this.RelatedType = ConvertHelper.ToBaseType(mySvcPropertyDefinition.RelatedType, _ServiceToken);
            
        }

        #region IPropertyDefinition Members

        public bool IsMandatory { get; internal set; }

        public bool IsUserDefinedType { get; internal set; }

        public Type BaseType { get; internal set; }

        public PropertyMultiplicity Multiplicity { get; internal set; }

        public IEnumerable<IIndexDefinition> InIndices
        {
            get
            {
                return _ServiceToken.GraphDSService.DescribeIndicesByNames(_ServiceToken.SecurityToken, _ServiceToken.TransactionToken, RelatedType.Name, _InIndices).Select(x => new RemoteIndexDefinition(x, _ServiceToken));
            }
            internal set
            {
                _InIndices = value.Select(x => x.Name).ToList();
            }
        }

        public IComparable DefaultValue { get; internal set; }

        public bool IsUserDefined { get; internal set; }

        #endregion


        #region IAttributeDefinition Members

        public long ID { get; internal set; }

        public string Name { get; internal set; }

        public AttributeType Kind { get { return AttributeType.Property; } }

        public IBaseType RelatedType { get; internal set; }

        #endregion


        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null
                && myOther.ID == ID;
        }

        #endregion

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAttributeDefinition);
        }
    }
}
