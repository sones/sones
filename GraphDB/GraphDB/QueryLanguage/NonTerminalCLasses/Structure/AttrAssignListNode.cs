/*
 * AttrAssignListNode
 * (c) Stefan Licht, 2009-2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Managers;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class AttrAssignListNode
    {

        public List<AAttributeAssignOrUpdate> AttributeAssigns { get; private set; }

        public AttrAssignListNode()
        {
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            AttributeAssigns = new List<AAttributeAssignOrUpdate>();
            foreach (ParseTreeNode aAttributeAssignment in parseNode.ChildNodes)
            {

                AttributeAssigns.Add((aAttributeAssignment.AstNode as AttributeAssignNode).AttributeValue);
                
            }
        }

    }
}
