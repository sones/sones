
#region Usings

using sones.GraphDB.Exceptions;
using sones.GraphDB.ImportExport;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Dump
{

    public class DumpFormatNode : AStructureNode
    {

        public DumpFormats DumpFormat { get; set; }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            
            var _GraphQL = GetGraphQLGrammar(context);

            if (parseNode.HasChildNodes())
            {

                var _Terminal = parseNode.ChildNodes[1].Token.Terminal;

                if (_Terminal == _GraphQL.S_GQL)
                {
                    DumpFormat = DumpFormats.GQL;
                }
                else
                {
                    throw new GraphDBException(new Errors.Error_InvalidDumpFormat(_Terminal.DisplayName));
                }

            }
            else
            {
                DumpFormat = DumpFormats.GQL;
            }

        }

    }

}
