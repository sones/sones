
#region Usings

using System;

using sones.GraphDB.ImportExport;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Import
{

    public class VerbosityNode : AStructureNode
    {

        public VerbosityTypes VerbosityType { get; private set; }

        public VerbosityNode()
        {
            VerbosityType = VerbosityTypes.Errors;
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var verbosityType = VerbosityType;

            if (parseNode.HasChildNodes() && Enum.TryParse<VerbosityTypes>(parseNode.ChildNodes[1].Token.Text, true, out verbosityType))
            {
                VerbosityType = verbosityType;
            }

        }

    }

}
