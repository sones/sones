
using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

namespace sones.GraphDB.GraphQL.StatementNodes.Drop
{

    public class DropIndexNode : AStatement
    {

        #region Data

        String _IndexName = String.Empty;
        String _IndexEdition = null;
        String _TypeName = null;

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

            _TypeName = ((ATypeNode)parseNode.ChildNodes[1].AstNode).ReferenceAndType.TypeName;
       
            _IndexName = parseNode.ChildNodes[4].Token.ValueString;
            if (parseNode.ChildNodes[5].HasChildNodes())
            {
                _IndexEdition = parseNode.ChildNodes[5].ChildNodes[1].Token.ValueString;
            }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {            
            if (String.IsNullOrEmpty(_TypeName))
            {
                var aError = new Error_TypeDoesNotExist("");

                return new QueryResult(aError);
            }

            var qresult = graphDBSession.DropIndex(_TypeName, _IndexName, _IndexEdition);
            qresult.PushIExceptional(ParsingResult);
            return qresult;
        }

    }

}
