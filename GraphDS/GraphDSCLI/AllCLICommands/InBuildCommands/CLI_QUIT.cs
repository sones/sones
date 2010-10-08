/* GraphFS CLI - QUIT
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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Exits the CLI
    /// </summary>

    public class CLI_QUIT : AAllInBuildCLICommands
    {

        #region Constructor

        public CLI_QUIT()
        {

            // Command name and description
            InitCommand("QUIT",
                        "Quits the CLI",
                        "Quits the CLI");

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
