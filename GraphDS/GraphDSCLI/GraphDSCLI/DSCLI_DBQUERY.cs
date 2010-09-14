/*
 * DSCLI_DBQUERY
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;


using sones.GraphFS.Connectors.GraphDSCLI;
using sones.GraphIO.TEXT;
using sones.GraphDS.Connectors.CLI;
using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            QueryResult _QueryResult;

            if (CLI_Output == CLI_Output.Standard)
                _QueryResult = QueryDB(myAGraphDSSharp, myOptions.ElementAt(1).Value[0].Option);
            else
                _QueryResult = QueryDB(myAGraphDSSharp, myOptions.ElementAt(1).Value[0].Option, false);

            Write(_QueryResult.ToTEXT());

            if (CLI_Output == CLI_Output.Standard)
                WriteLine();

            return Exceptional.OK;

        }

        #endregion

    }

}

