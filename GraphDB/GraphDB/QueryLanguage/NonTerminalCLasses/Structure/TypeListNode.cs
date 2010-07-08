using System;
using System.Collections.Generic;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class TypeListNode : AStructureNode
    {
        
        #region Properties

        public List<String> Types { get; private set; }

        #endregion

        public TypeListNode()
        {
            Types = new List<String>();
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes() && parseNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var child in parseNode.ChildNodes[1].ChildNodes)
                {
                    Types.Add((child.AstNode as ATypeNode).DBTypeStream.Name);
                }
            } // Otherwise this node is empty

        }

    }
}
