using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelByTypeNode : AStructureNode, IAstNodeInit
    {
        #region Data

        String _TypeName;

        #endregion

        #region constructors

        public SelByTypeNode()
        { }

        #endregion 

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _TypeName = parseNode.ChildNodes[1].Token.Text;
        }

        #endregion

        #region Accessor

        public String TypeName { get { return _TypeName; } }

        #endregion
    }
}
