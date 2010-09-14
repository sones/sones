/* GraphFS CLI - EXIT
 * (c) Henning Rauch, 2009
 * 
 * Exits the CLI
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.Lib.ErrorHandling;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Exits the CLI
    /// </summary>

    public class CLI_EXIT : AAllInBuildCLICommands
    {

        #region Constructor

        public CLI_EXIT()
        {

            // Command name and description
            InitCommand("EXIT",
                        "Exits the CLI",
                        "Exits the CLI");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            WriteLine("Now exiting...");

            return Exceptional.OK;

        }

        #endregion

    }

}
