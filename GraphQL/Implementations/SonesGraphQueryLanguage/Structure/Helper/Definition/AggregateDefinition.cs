using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class AggregateDefinition : ATermDefinition
    {

        #region Properties

        public ChainPartAggregateDefinition ChainPartAggregateDefinition { get; private set; }

        #endregion

        #region Ctor

        public AggregateDefinition(ChainPartAggregateDefinition myChainPartAggregateDefinition)
        {
            ChainPartAggregateDefinition = myChainPartAggregateDefinition;
        }

        #endregion

    }
}
