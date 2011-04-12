using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class CreateIndexNode : AStatement, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { throw new NotImplementedException(); }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { throw new NotImplementedException(); }
        }

        public override QueryResult Execute()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
