using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace sones.GraphDS.GraphDSRemoteClient.TypeManagement
{
    internal class RemoteBinaryPropertyDefinition : IBinaryPropertyDefinition
    {
        #region Data

        private IServiceToken _ServiceToken;

        #endregion

        #region Constructor
        
        internal RemoteBinaryPropertyDefinition(ServiceBinaryPropertyDefinition myServiceBinaryPropertyDefinition, IServiceToken myServiceToken)
        {
            this._ServiceToken = myServiceToken;
            this.ID = myServiceBinaryPropertyDefinition.ID;
            this.Name = myServiceBinaryPropertyDefinition.Name;
            this.IsUserDefined = myServiceBinaryPropertyDefinition.IsUserDefined;
            this.RelatedType = ConvertHelper.ToBaseType(myServiceBinaryPropertyDefinition.RelatedType, myServiceToken);
        }

        #endregion

        #region IAttributeDefinition Members

        public string Name { get; internal set; }

        public long ID { get; internal set; }

        public AttributeType Kind { get { return AttributeType.BinaryProperty; } }

        public IBaseType RelatedType { get; internal set; }

        public bool IsUserDefined { get; internal set; }

        #endregion

        #region IEquatable<IAttributeDefinition> Members

        public bool Equals(IAttributeDefinition myOther)
        {
            return myOther != null && myOther.ID == ID;
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
