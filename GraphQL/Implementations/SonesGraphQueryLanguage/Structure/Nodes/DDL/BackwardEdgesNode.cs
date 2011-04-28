using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// A list of single BackwardEdge definition nodes.
    /// </summary>
    public sealed class BackwardEdgesNode : AStructureNode, IAstNodeInit
    {
        #region Data

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<BackwardEdgeDefinition> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<BackwardEdgeDefinition> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public BackwardEdgesNode()
        {
            _BackwardEdgeInformation = new List<BackwardEdgeDefinition>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                foreach (var _ParseTreeNode in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode as BackwardEdgeNode != null)
                    {
                        _BackwardEdgeInformation.Add(((BackwardEdgeNode)_ParseTreeNode.AstNode).BackwardEdgeDefinition);
                    }
                }
            }

        }

        #endregion
    }
}
