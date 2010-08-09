/*
 * AttrAssignListNode
 * (c) Stefan Licht, 2009-2010
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
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
