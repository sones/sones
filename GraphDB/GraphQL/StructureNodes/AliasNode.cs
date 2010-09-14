/*
 * SelectValueAssignmentNode
 * (c) Stefan Licht, 2009-2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Select;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{
    public class AliasNode
    {

        public String AliasId { get; private set; }

        public AliasNode()
        {
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            AliasId = parseNode.ChildNodes[2].Token.ValueString;

        }

    }

}
