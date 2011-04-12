using System;
using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    internal sealed class NormalEdgeType: TypeBase, IEdgeType
    {
        #region Data

        private static readonly IAttributeDefinition[] _Attributes = new IAttributeDefinition[]
        {
            //TODO
        };

        #endregion

        internal static readonly IEdgeType Instance = new NormalEdgeType();

        private NormalEdgeType() : base(_Attributes, null) { }

        #region IEdgeType Members

        string IEdgeType.Name
        {
            get { throw new NotImplementedException(); }
        }

        IBehaviour IEdgeType.Behaviour
        {
            get { throw new NotImplementedException(); }
        }

        string IEdgeType.Comment
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.IsSealed
        {
            get { throw new NotImplementedException(); }
        }

        bool IEdgeType.HasParentEdgeType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEdgeType IEdgeType.GetParentEdgeType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IEdgeType.HasChildEdgeTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnumerable<IEdgeType> IEdgeType.GetChildEdgeTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnumerable<IPropertyDefinition> IEdgeType.GetProperties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
