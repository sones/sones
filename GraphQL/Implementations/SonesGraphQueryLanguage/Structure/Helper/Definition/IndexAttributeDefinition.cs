using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class IndexAttributeDefinition
    {

        #region Properties

        public IDChainDefinition IndexAttribute { get; private set; }
        public String OrderDirection { get; private set; }
        public String IndexType { get; private set; }

        #endregion

        #region Ctor

        public IndexAttributeDefinition(IDChainDefinition myIndexAttribute, string myIndexType, string myOrderDirection)
        {
            IndexAttribute = myIndexAttribute;
            IndexType = myIndexType;
            OrderDirection = myOrderDirection;
        }

        #endregion

    }
}
