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
    public sealed class IncomingEdgesNode : AStructureNode, IAstNodeInit
    {
        #region Data

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<IncomingEdgeDefinition> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<IncomingEdgeDefinition> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public IncomingEdgesNode()
        {
            _BackwardEdgeInformation = new List<IncomingEdgeDefinition>();
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                foreach (var _ParseTreeNode in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode as IncomingEdgeNode != null)
                    {
                        _BackwardEdgeInformation.Add(((IncomingEdgeNode)_ParseTreeNode.AstNode).BackwardEdgeDefinition);
                    }
                }
            }

        }

        #endregion
    }
}
