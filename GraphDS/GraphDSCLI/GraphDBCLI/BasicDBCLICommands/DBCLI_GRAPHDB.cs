/*
 * DBCLI_GraphDB
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.DataStructures.UUID;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Starts and quits a given GraphDB
    /// </summary>

    public class DBCLI_GraphDB : AllBasicDBCLICommands
    {

        #region Constructor

        public DBCLI_GraphDB()
        {

            // Command name and description
            InitCommand("GraphDB",
                        "Starts and quits a given GraphDB",
                        "Starts and quits a given GraphDB");

            NonTerminal GraphDB_Action = new NonTerminal("GraphDB_Action");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + GraphDB_Action);

            GraphDB_Action.Rule = StartSymbol + stringLiteralPVFS
                                    | StopSymbol;
            //GraphDB_Action.GraphOptions.Add(GraphOption.IsOption);
            
            _CommandNonTerminals.Add(GraphDB_Action);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            if (myOptions.ElementAt(1).Value[0].Option.Equals(StartSymbol.DisplayName))
            {
                //we have a start
                if (myAGraphDSSharp != null)
                {
                    if (myAGraphDSSharp.IGraphDBSession == null)
                    {
                        var internalGraphDB = new GraphDB2(new UUID(), new ObjectLocation(myOptions.ElementAt(2).Value[0].Option), myAGraphDSSharp, true);
                        var DB = new GraphDBSession(internalGraphDB, myAGraphDSSharp.SessionToken.SessionInfo.Username);
                    }
                    else
                    {
                        Console.WriteLine("GraphDB instance could not be started, because you already started a GraphDB.");
                    }
                }
                else
                {
                    Console.WriteLine("GraphDB instance could not be started, because you do not have a GraphVFS mounted.");
                }
            }

            else
            {

                if (myAGraphDSSharp.IGraphDBSession != null)
                {
                    myAGraphDSSharp.IGraphDBSession.Shutdown();
                }
                else
                {
                    Console.WriteLine("No GraphDB instance started...");
                }
            }

            return Exceptional.OK;

        }

        #endregion

    }

}
