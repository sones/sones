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

                var _KeyTerm = parseNode.ChildNodes[0].Token.KeyTerm;

                if (_KeyTerm.Text.Equals("S_ALL"))
                {
                    DumpType = DumpTypes.GDDL | DumpTypes.GDML;
                }
                else if (_KeyTerm.Text.Equals("S_GDDL"))
                {
                    DumpType = DumpTypes.GDDL;
                }
                else if (_KeyTerm.Text.Equals("S_GDML"))
                {
                    DumpType = DumpTypes.GDML;
                }
                else
                {
                    throw new InvalidDumpTypeException(_KeyTerm.Text, "");
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
