using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// One single BackwardEdge definition node.
    /// </summary>
    public sealed class BackwardEdgeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public BackwardEdgeDefinition BackwardEdgeDefinition { get; private set; }

        /// <summary>
        /// The destination type of the backwardedge
        /// </summary>
        private String _TypeName;

        /// <summary>
        /// the destination attribute on the TypeName
        /// </summary>
        private String _TypeAttributeName;

        /// <summary>
        /// The real new name of the attribute
        /// </summary>
        private String _AttributeName;

        #endregion

        #region constructor

        public BackwardEdgeNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region Extract type and attribute

            //if (parseNode.ChildNodes.Count != 4)
            //    throw new Exception("This is not a [Type].[Attribute] definition: " + parseNode.ChildNodes[0].ToString());

            _TypeName = parseNode.ChildNodes[0].Token.ValueString;
            _TypeAttributeName = parseNode.ChildNodes[2].Token.ValueString;

            #endregion

            _AttributeName = parseNode.ChildNodes[3].Token.ValueString;

            BackwardEdgeDefinition = new BackwardEdgeDefinition(_AttributeName, _TypeName, _TypeAttributeName);

        }

        #endregion
    }
}
