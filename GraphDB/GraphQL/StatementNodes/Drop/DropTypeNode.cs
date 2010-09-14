
using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;

using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

namespace sones.GraphDB.GraphQL.StatementNodes.Drop
{

    public class DropTypeNode : AStatement
    {

        #region Data

        String _TypeName = ""; //the name of the type that should be dropped

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "DropType"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            #region get Name

            _TypeName = parseNode.ChildNodes[2].Token.ValueString;

            #endregion
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.DropType(_TypeName);
            qresult.PushIExceptional(ParsingResult);
            return qresult;
        }

    }

}
