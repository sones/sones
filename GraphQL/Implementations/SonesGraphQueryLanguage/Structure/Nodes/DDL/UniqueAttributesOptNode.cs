using System;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class UniqueAttributesOptNode : AStructureNode, IAstNodeInit
    {
        private List<String> _UniqueAttributes;
        public List<String> UniqueAttributes
        {
            get { return _UniqueAttributes; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                _UniqueAttributes = new List<String>();

                if (HasChildNodes(parseNode.ChildNodes[1]))
                {
                    _UniqueAttributes = (from c in parseNode.ChildNodes[1].ChildNodes select c.Token.ValueString).ToList();
                }

            }
        }

        #endregion
    }
}
