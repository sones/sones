using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.Library.ErrorHandling;
using sones.GraphQL.GQL.ErrorHandling;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace sones.GraphQL.StatementNodes.Transactions
{
    public sealed class BeginTransactionNode : AStatement, IAstNodeInit
    {
        #region Properties

        public Boolean IsDistributed { get; private set; }
        public Boolean IsLongRunning { get; private set; }
        public IsolationLevel Isolation { get; private set; }
        public String Name { get; private set; }
        public DateTime? TimeStamp { get; private set; }

        #endregion

        #region constructors

        public BeginTransactionNode()
        {
            TimeStamp = null;
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            try
            {
                if (parseNode.ChildNodes != null && parseNode.ChildNodes.Count != 0)
                {
                    GetTransactOptions(parseNode);

                    //in the case we have some optional parameters
                    if (parseNode.ChildNodes.Count > 3)
                    {
                        GetTransactAttributes(parseNode.ChildNodes[3], 0);
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
            get { return "BeginTransaction"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            var TransactionToken = myGraphDB.BeginTransaction(mySecurityToken, IsLongRunning, Isolation);

            VertexView view = null;

            if (TransactionToken != null)
            {

                var readoutVals = new Dictionary<String, Object>();

                readoutVals.Add("TransactionToken", TransactionToken);
                readoutVals.Add("UUID", TransactionToken.ID);
                readoutVals.Add("Created", TimeStamp);
                readoutVals.Add("Distributed", IsDistributed);
                readoutVals.Add("IsolationLevel", Isolation);
                readoutVals.Add("LongRunning", IsLongRunning);
                readoutVals.Add("Name", Name);
                
                view = new VertexView(readoutVals, null);

                sw.Stop();

                return new QueryResult(myQuery, "GQL", Convert.ToUInt64(sw.ElapsedMilliseconds), ResultType.Successful, new List<IVertexView> { view });
            }

            sw.Stop();

            // DBTransaction is Exceptional
            return new QueryResult(myQuery, "GQL", Convert.ToUInt64(sw.ElapsedMilliseconds), ResultType.Successful, new List<IVertexView> { view });

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
                if (parseNode.ChildNodes[1].ChildNodes != null && parseNode.ChildNodes[1].ChildNodes.Count != 0)
                {
                    if (parseNode.ChildNodes[1].ChildNodes[0] != null)
                    {
                        switch (parseNode.ChildNodes[1].ChildNodes[0].Token.Text.ToUpper())
                        {
                            case SonesGQLConstants.TRANSACTION_DISTRIBUTED:
                                IsDistributed = true;
                                break;

                            case SonesGQLConstants.TRANSACTION_LONGRUNNING:
                                IsLongRunning = true;
                                break;
                        }
                    }

                    if (parseNode.ChildNodes[1].ChildNodes.Count > 1)
                    {
                        if (parseNode.ChildNodes[1].ChildNodes[1].Token.Text.ToUpper() == SonesGQLConstants.TRANSACTION_LONGRUNNING)
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
                if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes != null && parseNode.ChildNodes[myCurrentChildNode].ChildNodes.Count != 0)
                {
                    switch (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[0].Token.Text.ToUpper())
                    {
                        case SonesGQLConstants.TRANSACTION_ISOLATION:
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                IsolationLevel isolation;
                                if (!Enum.TryParse<IsolationLevel>(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, true, out isolation))
                                {
                                    throw new InvalidTransactionIsolationLevelException(parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, "");
                                }
                                Isolation = isolation;
                                //_Isolation = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString, true);
                            }
                            break;

                        case SonesGQLConstants.TRANSACTION_NAME:
                            if (parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2] != null)
                            {
                                Name = parseNode.ChildNodes[myCurrentChildNode].ChildNodes[2].Token.ValueString;
                            }
                            break;

                        case SonesGQLConstants.TRANSACTION_TIMESTAMP:
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

    }
}
