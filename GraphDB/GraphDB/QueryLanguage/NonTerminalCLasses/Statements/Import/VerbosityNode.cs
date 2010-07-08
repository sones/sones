
#region Usings

using System;
using sones.GraphDB.ImportExport;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Import
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
