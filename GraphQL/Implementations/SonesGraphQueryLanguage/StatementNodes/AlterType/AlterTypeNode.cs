using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphQL.Result;
using sones.GraphQL.Structures.Enums;
using sones.GraphQL.StructureNodes;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structures.AlterType;


namespace sones.GraphQL.StatementNodes
{

    /// <summary>
    /// This node is requested in case of an Alter Type statement.
    /// Warnings: Warning_ObsoleteGQL
    /// </summary>
    class AlterTypeNode : AStatement
    {

        #region Properties - Statement information

        public override String StatementName { get { return "AlterType"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Data

        String _TypeName = ""; //the name of the type that should be altered
        List<AAlterTypeCommand> _AlterTypeCommand;

        #endregion

        #region GetContent

        /// <summary>
        /// Gets the content of a AlterTypeNode.
        /// </summary>
        /// <param name="myParsingContext">ParsingContext of Irony.</param>
        /// <param name="myParseTreeNode">The current ParseNode.</param>
        /// <param name="myTypeManager">The TypeManager of the GraphDB.</param>
        public override void GetContent(ParsingContext myParsingContext, ParseTreeNode myParseTreeNode)
        {

            //try
            //{
            //    _AlterTypeCommand = new List<AAlterTypeCommand>();

            //    _TypeName = myParseTreeNode.ChildNodes[2].Token.ValueString;

            //    #region Get the AlterTypeCommand

            //    foreach (var alterCmds in myParseTreeNode.ChildNodes[3].ChildNodes)
            //    {
            //        if (alterCmds.AstNode != null)
            //        {
            //            var alterCommand = (AlterCommandNode)alterCmds.AstNode;
            //            ParsingResult.PushIExceptional(alterCommand.ParsingResult);

            //            if (alterCommand.AlterTypeCommand != null)
            //            {
            //                _AlterTypeCommand.Add(alterCommand.AlterTypeCommand);
            //            }
            //        }
            //    }

            //    if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            //    {
            //        _AlterTypeCommand.Add(new AlterType_SetUnique() { UniqueAttributes = ((UniqueAttributesOptNode)myParseTreeNode.ChildNodes[4].AstNode).UniqueAttributes });
            //    }

            //    if (myParseTreeNode.ChildNodes[5].HasChildNodes())
            //    {
            //        _AlterTypeCommand.Add(new AlterType_SetMandatory() { MandatoryAttributes = ((MandatoryOptNode)myParseTreeNode.ChildNodes[5].AstNode).MandatoryAttribs });
            //    }

            //    #endregion

            //}

            //catch (Exception e)      //replace through specific GraphQLExceptions 
            //{
            //    throw new Exception(e.Message, e);
            //}

        }

        #endregion

        #region Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {

            //var qresult = myTransactionToken.AlterType(_TypeName, _AlterTypeCommand);
            return null; //qresult;
        }

        #endregion

    }

}
