using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class DumpNode : AStatement, IAstNodeInit
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
