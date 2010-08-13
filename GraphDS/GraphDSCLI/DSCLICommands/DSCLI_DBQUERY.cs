/*
 * DSCLI_DBQUERY
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.CLI;

using sones.GraphDB.Structures.Result;
using sones.GraphFS.Connectors.GraphDSCLI;
using sones.GraphIO;
using sones.GraphIO.TEXT;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// This command enables the user to get informations concerning 
    /// the myAttributes of a given type.
    /// </summary>

    public class DSCLI_DBQUERY : AllGraphDSCLICommands
    {

        #region Constructor

        public DSCLI_DBQUERY()
        {

            // Command name and description
            InitCommand("DBQUERY",
                        "Executes a query",
                        "Executes a query");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteral);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFS2Session, ref object myIGraphDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphDBSession = myIGraphDBSession as IGraphDBSession;
            QueryResult _QueryResult;

            if (_IGraphDBSession == null)
            {
                WriteLine("No database instance started...");
                return;
            }

            if (CLI_Output == CLI_Output.Standard)
                _QueryResult = QueryDB(myOptions.ElementAt(1).Value[0].Option, _IGraphDBSession);
            else
                _QueryResult = QueryDB(myOptions.ElementAt(1).Value[0].Option, _IGraphDBSession, false);

            Write(_QueryResult.ToTEXT());

            if (CLI_Output == CLI_Output.Standard)
                WriteLine();

        }

        #endregion

    }

}

