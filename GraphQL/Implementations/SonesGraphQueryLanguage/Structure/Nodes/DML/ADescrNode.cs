using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Nodes;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.GQL.Structure.Nodes.DML
{
    public abstract class ADescrNode : AStructureNode
    {
        public abstract ADescribeDefinition DescribeDefinition { get; }
    }
}
