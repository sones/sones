using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Enums;
using sones.GraphQL.ErrorHandling;
using sones.Library.DataStructures;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class DumpFormatNode : AStructureNode, IAstNodeInit
    {
        #region properties

        public DumpFormats DumpFormat { get; set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var _GraphQL = context.Parser.Language.Grammar;

            if (HasChildNodes(parseNode))
            {

                var _Terminal = parseNode.ChildNodes[1].Token.Terminal;

                if (_Terminal == _GraphQL.ToTerm("GQL"))
                {
                    DumpFormat = DumpFormats.GQL;
                }
                else
                {
                    throw new InvalidDumpFormatException(_Terminal.ToString(), "");
                }

            }
            else
            {
                DumpFormat = DumpFormats.GQL;
            }
        }

        #endregion
    }
}
