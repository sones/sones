using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Import
{
    public class CommentsNode : AStructureNode
    {

        public List<String> Comments { get; private set; }

        public CommentsNode()
        {
            Comments = new List<string>();
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                foreach (var child in (parseNode.ChildNodes[1].AstNode as TupleNode).Tuple)
                {
                    Comments.Add(child.Value.ToString());
                }
            }

        }

    }
}
