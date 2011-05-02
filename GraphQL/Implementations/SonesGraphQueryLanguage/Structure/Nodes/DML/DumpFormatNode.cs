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

                var _KeyTerm = parseNode.ChildNodes[1].Token.KeyTerm;

                if (_KeyTerm.Text.Equals("S_GQL"))
                {
                    DumpFormat = DumpFormats.GQL;
                }
                else
                {
                    throw new InvalidDumpFormatException(_KeyTerm.Text, "");
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
