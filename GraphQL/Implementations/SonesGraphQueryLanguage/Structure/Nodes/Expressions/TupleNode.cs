using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;

namespace sones.GraphQL.Structure.Nodes.Expressions
{

    /// <summary>
    /// This node is requested in case of an tuple statement.
    /// </summary>
    public sealed class TupleNode : AStructureNode, IAstNodeInit
    {

        #region IAstNodeInit Members

        public void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
