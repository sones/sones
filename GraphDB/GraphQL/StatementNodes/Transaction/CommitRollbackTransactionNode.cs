/* <id name="GraphDB – CommitRollbackTransactionNode" />
 * <copyright file="CommitRollbackTransactionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Transactions;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Transaction
{

    public enum CommandType
    { 
        Commit,
        Rollback
    }
    
    public class CommitRollbackTransactionNode : AStatement
    {

        #region Properties

        public String Name { get; private set; }
        public Boolean ASync { get; private set; }
        public CommandType CommandType { get; private set; }

        #endregion

        #region Constructor

        public CommitRollbackTransactionNode()
        { }

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
                if (myNode.ChildNodes[myCurrentChildNode].HasChildNodes())
                {
                    if (myNode.ChildNodes[myCurrentChildNode].ChildNodes[0].Token.Text.ToUpper() == DBConstants.TRANSACTION_NAME)
                        Name = myNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString;
                }
                else
                {
                    if (myNode.ChildNodes[myCurrentChildNode].Token.Text.ToUpper() == DBConstants.TRANSACTION_COMROLLASYNC)
                        ASync = true;
                }
            
                GetAttributes(myNode, myCurrentChildNode + 1);    
            }        
        }

        #endregion

        #region AStatement

        public override string StatementName
        {
            get { return "CommitRollbackTransaction"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            try
            {
                if (parseNode.HasChildNodes())
                {
                    CommandType = (CommandType)Enum.Parse(typeof(CommandType), parseNode.ChildNodes[0].ChildNodes[0].Token.Text, true);
                    
                    //in the case we have some optional parameters
                    if (parseNode.ChildNodes[2].HasChildNodes())
                    {
                        GetAttributes(parseNode.ChildNodes[2], 0);    
                    }
                }
            }
            catch (GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
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
            var qr = new QueryResult();
            DBTransaction dbTransaction;
            if (CommandType == Transaction.CommandType.Commit)
            {
                dbTransaction = graphDBSession.CommitTransaction();

            }
            else
            {
                dbTransaction = graphDBSession.RollbackTransaction();
            }

            if (dbTransaction.Success())
            {
                var readoutVals = new Dictionary<String, Object>();
                readoutVals.Add("UUID", dbTransaction.UUID.ToHexString());
                readoutVals.Add("Created", dbTransaction.Created);
                readoutVals.Add("Finished", dbTransaction.Finished);
                readoutVals.Add("Distributed", dbTransaction.Distributed);
                readoutVals.Add("IsolationLevel", dbTransaction.IsolationLevel.ToString());
                readoutVals.Add("LongRunning", dbTransaction.LongRunning);
                readoutVals.Add("Name", dbTransaction.Name);
                readoutVals.Add("State", dbTransaction.State.ToString());

                var selResultSet = new SelectionResultSet(new DBObjectReadout(readoutVals));
                qr.AddResult(selResultSet);
            }
            else
            {
                qr.AddErrorsAndWarnings(dbTransaction);
            }
            qr.AddErrorsAndWarnings(ParsingResult);

            return qr;
        }

        #endregion

    }

}
