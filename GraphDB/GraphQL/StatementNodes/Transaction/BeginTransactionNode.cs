/* <id name="GraphDB – BeginTransactionNode" />
 * <copyright file="BeginTransactionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Transactions;

using sones.GraphDB.Errors.Transactions;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.Structures.Result;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Transaction
{

    public class BeginTransactionNode : AStatement
    {

        #region Properties

        public Boolean IsDistributed { get; private set; }
        public Boolean IsLongRunning { get; private set; }
        public IsolationLevel Isolation { get; private set; }
        public String Name { get; private set; }
        public DateTime? TimeStamp { get; private set; }

        #endregion

        #region AStatement

        #region AStatement properties

        public override string StatementName
        {
            get { return "BeginTransaction"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        #endregion

        #region constructors

        public BeginTransactionNode()
        {
            TimeStamp = null;
        }

        #endregion

        #region private helper methods

        /// <summary>
        /// get transaction options like distributed or long-running
        /// </summary>
        /// <param name="parseNode">the node that contain the values</param>
        private void GetTransactOptions(ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes[1] != null)
            {
                if (parseNode.ChildNodes[1].HasChildNodes())
                {
                    if (parseNode.ChildNodes[1].ChildNodes[0] != null)
                    {
                        switch (parseNode.ChildNodes[1].ChildNodes[0].Token.Text.ToUpper())
                        {
                            case DBConstants.TRANSACTION_DISTRIBUTED:
                                IsDistributed = true;
                                break;

                            case DBConstants.TRANSACTION_LONGRUNNING:
                                IsLongRunning = true;
                                break;
                        }
                    }

                    if (parseNode.ChildNodes[1].ChildNodes.Count > 1)
                    {
                        if (parseNode.ChildNodes[1].ChildNodes[1].Token.Text.ToUpper() == DBConstants.TRANSACTION_LONGRUNNING)
                            IsLongRunning = true;
                    }
                }
            }
        }
                
        /// <summary>
        /// get the values for isolation, name or timestamp
        /// </summary>
        /// <param name="parseNode">the child node that contain the values</param>
        /// <param name="myCurrentChildNode">current child node</param>
        private void GetTransactAttributes(ParseTreeNode parseNode, Int32 myCurrentChildNode)
        {
            if (myCurrentChildNode < parseNode.ChildNodes.Count)
            {
                if (parseNode.ChildNodes[myCurrentChildNode].HasChildNodes())
                {
                    switch (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[0].Token.Text.ToUpper())
                    {
                        case DBConstants.TRANSACTION_ISOLATION :
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                IsolationLevel isolation;
                                if (!Enum.TryParse<IsolationLevel>(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, true, out isolation))
                                {
                                    throw new GraphDBException(new Error_InvalidTransactionIsolationLevel(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString));
                                }
                                Isolation = isolation;
                                //_Isolation = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, true);
                            }
                            break;
                        
                        case DBConstants.TRANSACTION_NAME :
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                Name = parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString;
                            }
                            break;
                        
                        case DBConstants.TRANSACTION_TIMESTAMP :
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                TimeStamp = DateTime.ParseExact(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, "yyyyddMM.HHmmss.fffffff", null);
                            }
                            break;
                    }

                }

                GetTransactAttributes(parseNode, myCurrentChildNode + 1);
            }            
        }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            try
            {                
                if (parseNode.HasChildNodes())
                {
                    GetTransactOptions(parseNode);

                    //in the case we have some optional parameters
                    if (parseNode.ChildNodes.Count > 3)
                    {
                        GetTransactAttributes(parseNode.ChildNodes[3], 0);
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
            var dbTransaction = graphDBSession.BeginTransaction(IsDistributed, IsLongRunning, Isolation, Name, TimeStamp);

            if (dbTransaction.Success())
            {
                var readoutVals = new Dictionary<String, Object>();
                readoutVals.Add("UUID", dbTransaction.UUID.ToHexString());
                readoutVals.Add("Created", dbTransaction.Created);
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
