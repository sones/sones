using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class DescribeEdgeNode : ADescrNode, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _DescribeEdgeDefinition = new DescribeEdgeDefinition(parseNode.ChildNodes[1].Token.ValueString);
        }

        #endregion

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeEdgeDefinition; }
        }
        private DescribeEdgeDefinition _DescribeEdgeDefinition;

        #endregion
    }
}
