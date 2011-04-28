using System;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class MandatoryOptNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private List<string> _MandAttribs;
        
        #endregion

        #region Accessor
        public List<string> MandatoryAttribs
        {
            get { return _MandAttribs; }
        }
        #endregion

        #region constructor
        public MandatoryOptNode()
        {
            _MandAttribs = new List<string>();
        }
        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {

            if (HasChildNodes(parseNode))
            {
                if (HasChildNodes(parseNode.ChildNodes[1]))
                {
                    _MandAttribs = (from Attr in parseNode.ChildNodes[1].ChildNodes select Attr.Token.ValueString).ToList();
                }
            }

        }

        #endregion
    }
}
