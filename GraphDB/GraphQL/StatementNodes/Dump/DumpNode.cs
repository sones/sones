
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.ImportExport;
using sones.GraphDB.Interfaces;
using sones.GraphDB.Structures.Enums;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Dump
{

    public class DumpNode : AStatement
    {

        private List<String> _TypesToDump;
        private DumpFormats _DumpFormat;
        private DumpTypes   _DumpType;
        private IDumpable   _DumpableGrammar;
        private String      _DumpDestination;

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Get the optional type list

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                _TypesToDump = ((parseNode.ChildNodes[1].ChildNodes[1].AstNode as TypeListNode).Types).Select(tlnode => tlnode.TypeName).ToList();
            }

            #endregion

            _DumpType           = (parseNode.ChildNodes[2].AstNode as DumpTypeNode).DumpType;
            _DumpFormat         = (parseNode.ChildNodes[3].AstNode as DumpFormatNode).DumpFormat;
            _DumpableGrammar    = context.Compiler.Language.Grammar as IDumpable;

            if (_DumpableGrammar == null)
            {
                throw new GraphDBException(new Error_NotADumpableGrammar(context.Compiler.Language.Grammar.GetType().ToString()));
            }

            if (parseNode.ChildNodes[4].HasChildNodes())
            {
                _DumpDestination = parseNode.ChildNodes[4].ChildNodes[1].Token.ValueString;
            }

        }

        public override String StatementName
        {
            get { return "DUMP"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var qresult = myIGraphDBSession.Export(_DumpFormat.ToString().ToUpper(), _DumpDestination, _DumpableGrammar, _TypesToDump, _DumpType);
            qresult.PushIExceptional(ParsingResult);
            return qresult;
        }

    }

}
