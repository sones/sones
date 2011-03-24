using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class EdgeTypeParamsNode : AStructureNode, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
