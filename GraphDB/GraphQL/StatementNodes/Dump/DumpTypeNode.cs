
using sones.GraphDB.Exceptions;
using sones.GraphDB.ImportExport;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.GraphQL.StatementNodes.Dump
{

    public class DumpTypeNode : AStructureNode
    {

        public DumpTypes DumpType { get; set; }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            
            var _GraphQL = GetGraphQLGrammar(context);

            if (parseNode.HasChildNodes())
            {

                var _Terminal = parseNode.ChildNodes[0].Token.Terminal;

                if (_Terminal      == _GraphQL.S_ALL)
                {
                    DumpType = DumpTypes.GDDL | DumpTypes.GDML;
                }
                else if (_Terminal == _GraphQL.S_GDDL)
                {
                    DumpType = DumpTypes.GDDL;
                }
                else if (_Terminal == _GraphQL.S_GDML)
                {
                    DumpType = DumpTypes.GDML;
                }
                else
                {
                    throw new GraphDBException(new Errors.Error_InvalidDumpType(_Terminal.DisplayName));
                }

            }
            else
            {
                DumpType = DumpTypes.GDDL | DumpTypes.GDML;
            }

        }

    }

}
