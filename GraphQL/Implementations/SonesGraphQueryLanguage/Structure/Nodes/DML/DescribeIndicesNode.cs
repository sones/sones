using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Node to get description of all indices on a type
    /// </summary>
    public sealed class DescribeIndicesNode : ADescrNode, IAstNodeInit
    {
        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeIndexDefinition; }
        }
        private DescribeIndexDefinition _DescribeIndexDefinition;

        #endregion

        #region AStructureNode

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            string type = parseNode.ChildNodes[1].ChildNodes[0].Token.ValueString;

            string indexName = "";

            if (parseNode.ChildNodes[1].ChildNodes.Count > 1)
            {
                indexName = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;
            }

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
