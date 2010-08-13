#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.GraphQL.StructureNodes;

#endregion

namespace sones.GraphDB.GraphQL.Structure
{
    public class SelByTypeNode : AStructureNode
    {
        #region Data

        String _TypeName;

        #endregion

        #region constructors

        public SelByTypeNode()
        {}

        #endregion 

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _TypeName = parseNode.ChildNodes[1].Token.Text;
        }

        #region Accessor

        public String TypeName { get { return _TypeName; } }

        #endregion
    }
}
