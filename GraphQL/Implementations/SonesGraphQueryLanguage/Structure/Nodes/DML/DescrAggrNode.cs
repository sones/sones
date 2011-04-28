using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class DescrAggrNode : ADescrNode, IAstNodeInit
    {
        public DescrAggrNode()
        { }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes != null && parseNode.ChildNodes.Count != 0)
            {
                _DescrAggrDefinition = new DescribeAggregateDefinition(parseNode.ChildNodes[1].Token.ValueString.ToUpper());
            }
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
