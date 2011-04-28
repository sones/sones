using System;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// A collection of all params for a EdgeType definition
    /// </summary>
    public sealed class EdgeTypeParamsNode : AStructureNode, IAstNodeInit
    {
        private List<EdgeTypeParamDefinition> _Parameters;
        public EdgeTypeParamDefinition[] Parameters
        {
            get
            {
                if (_Parameters == null)
                    return new EdgeTypeParamDefinition[0];
                return _Parameters.ToArray();
            }
        }

        public EdgeTypeParamsNode()
        {
            _Parameters = new List<EdgeTypeParamDefinition>();
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            foreach (var child in parseNode.ChildNodes)
            {
                _Parameters.Add((child.AstNode as EdgeTypeParamNode).EdgeTypeParamDefinition);

            }
        }

        #endregion
    }
}
