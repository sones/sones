using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.ErrorHandling;
using sones.Library.DataStructures;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class DumpTypeNode : AStructureNode, IAstNodeInit
    {
        #region data

        public DumpTypes DumpType { get; set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var _GraphQL = context.Parser.Language.Grammar;

            if (HasChildNodes(parseNode))
            {

                var _Terminal = parseNode.ChildNodes[0].Token.Terminal;

                if (_Terminal == _GraphQL.ToTerm("ALL"))
                {
                    DumpType = DumpTypes.GDDL | DumpTypes.GDML;
                }
                else if (_Terminal == _GraphQL.ToTerm("GDDL"))
                {
                    DumpType = DumpTypes.GDDL;
                }
                else if (_Terminal == _GraphQL.ToTerm("GDML"))
                {
                    DumpType = DumpTypes.GDML;
                }
                else
                {
                    throw new InvalidDumpTypeException(_Terminal.ToString(), "");
                }

            }
            else
            {
                DumpType = DumpTypes.GDDL | DumpTypes.GDML;
            }
        }

        #endregion

    }
}
