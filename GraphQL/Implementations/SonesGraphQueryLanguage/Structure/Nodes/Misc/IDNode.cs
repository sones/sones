using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    /// <summary>
    /// This node is requested in case of an ID statement. This might be something like U.Name or Name or U.$GUID or U.Friends.Name.
    /// It is necessary to execute an AType node (or TypeWrapper) in previous.
    /// </summary>
    public sealed class IDNode : AStructureNode, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
