using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.GQL.Structure.Nodes.DML;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class DescrFuncNode : ADescrNode, IAstNodeInit
    {
        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeFuncDefinition; }
        }
        private DescribeFuncDefinition _DescribeFuncDefinition;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _DescribeFuncDefinition = new DescribeFuncDefinition(parseNode.ChildNodes[1].Token.ValueString.ToUpper());
        }

        #endregion
    }
}
