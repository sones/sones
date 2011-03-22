using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphQL.StructureNodes;
using Irony.Parsing;
using Irony.Ast;


namespace sones.GraphQL.StructureNodes
{

    public class UniqueAttributesOptNode : AStructureNode, IAstNodeInit
    {

        private List<String> _UniqueAttributes;
        public List<String> UniqueAttributes
        {
            get { return _UniqueAttributes; }
        }

        #region GetContent(myCompilerContext, myParseTreeNode)

        private void GetContent(ParsingContext myParsingContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.HasChildNodes())
            {

                _UniqueAttributes = new List<String>();

                if (myParseTreeNode.ChildNodes[1].HasChildNodes())
                {
                    _UniqueAttributes = (from c in myParseTreeNode.ChildNodes[1].ChildNodes select c.Token.ValueString).ToList();
                }

            }

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext myParsingContext, ParseTreeNode parseNode)
        {
            GetContent(myParsingContext, parseNode);
        }

        #endregion

    }

}
