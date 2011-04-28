using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.GQL.Structure.Nodes.DML;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Node to get index description of a given type
    /// </summary>
    public sealed class DescribeIndexNode : ADescrNode, IAstNodeInit
    {
        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeIndexDefinition; }
        }
        private DescribeIndexDefinition _DescribeIndexDefinition;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            string type = parseNode.ChildNodes[1].ChildNodes[0].Token.ValueString;
            string indexName = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;
            string edition = null;
            if (parseNode.ChildNodes[2].ChildNodes != null && parseNode.ChildNodes[2].ChildNodes.Count != 0)
            {
                edition = parseNode.ChildNodes[2].ChildNodes[0].Token.ValueString;
            }

            _DescribeIndexDefinition = new DescribeIndexDefinition(type, indexName, edition);
        }

        #endregion
    }
}
