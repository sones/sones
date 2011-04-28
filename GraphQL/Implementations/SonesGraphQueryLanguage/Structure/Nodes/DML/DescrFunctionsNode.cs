using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Node to get description of all functions
    /// </summary>
    public sealed class DescrFunctionsNode : ADescrNode, IAstNodeInit
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
            _DescribeFuncDefinition = new DescribeFuncDefinition();
        }

        #endregion
    }
}
