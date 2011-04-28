using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Node to get description of all aggregates
    /// </summary>
    public sealed class DescrAggrsNode : ADescrNode, IAstNodeInit
    {
        public DescrAggrsNode()
        { }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _DescrAggrDefinition = new DescribeAggregateDefinition();
        }

        #endregion

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescrAggrDefinition; }
        }

        private DescribeAggregateDefinition _DescrAggrDefinition;

        #endregion
    }
}
