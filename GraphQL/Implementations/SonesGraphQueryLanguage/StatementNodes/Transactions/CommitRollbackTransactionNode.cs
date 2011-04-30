using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using System.Collections.Generic;
using System.Diagnostics;

namespace sones.GraphQL.StatementNodes.Transactions
{
    public enum CommandType
    {
        Commit,
        Rollback
    }

    public sealed class CommitRollbackTransactionNode : AStatement, IAstNodeInit
    {
        #region Properties

        public String Name { get; private set; }
        public Boolean ASync { get; private set; }
        public CommandType Command_Type { get; private set; }

        #endregion

        #region Constructor

        public CommitRollbackTransactionNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            try
            {
                if (parseNode.ChildNodes != null && parseNode.ChildNodes.Count != 0)
                {
                    Command_Type = (CommandType)Enum.Parse(typeof(CommandType), parseNode.ChildNodes[0].ChildNodes[0].Token.Text, true);

                    //in the case we have some optional parameters
                    if (parseNode.ChildNodes[2].ChildNodes != null && parseNode.ChildNodes[2].ChildNodes.Count != 0)
                    {
                        GetAttributes(parseNode.ChildNodes[2], 0);
                    }
                }
            }
            catch (ASonesException e)
            {
                throw e;
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "CommitRollbackTransaction"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            var _ReturnValues = new Dictionary<String, Object>();

            if (Command_Type == CommandType.Commit)
            {
                myGraphDB.CommitTransaction(mySecurityToken, myTransactionToken);
            }

            else
            {
                myGraphDB.RollbackTransaction(mySecurityToken, myTransactionToken);
            }

            _ReturnValues.Add("UUID", myTransactionToken.ID);
            _ReturnValues.Add("ExecutedCommand", Command_Type);
            _ReturnValues.Add("Name", Name == null ? "" : Name);
            _ReturnValues.Add("ASync", ASync);

            return new QueryResult(myQuery, "GQL", Convert.ToUInt64(sw.ElapsedMilliseconds), ResultType.Successful, new List<IVertexView> { new VertexView(_ReturnValues, null) } );
        }

        #endregion

        #region private helper methods

        /// <summary>
        /// get values for name and async
        /// </summary>
        /// <param name="myNode">the child node that contain the values</param>
        /// <param name="myCurrentChildNode">the current child node</param>
        private void GetAttributes(ParseTreeNode myNode, Int32 myCurrentChildNode)
        {
            if (myCurrentChildNode < myNode.ChildNodes.Count)
            {
                if (myNode.ChildNodes[myCurrentChildNode].ChildNodes != null && myNode.ChildNodes[myCurrentChildNode].ChildNodes.Count != 0)
                {
                    if (myNode.ChildNodes[myCurrentChildNode].ChildNodes[0].Token.Text.ToUpper() == SonesGQLConstants.TRANSACTION_NAME)
                        Name = myNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString;
                }
                else
                {
                    if (myNode.ChildNodes[myCurrentChildNode].Token.Text.ToUpper() == SonesGQLConstants.TRANSACTION_COMROLLASYNC)
                        ASync = true;
                }

                GetAttributes(myNode, myCurrentChildNode + 1);
            }
        }

        #endregion



    }
}
