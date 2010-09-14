/* GraphFS CLI - SAVEHISTORY
 * (c) Henning Rauch, 2009
 * 
 * Saves a specified history
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
    /// Saves a specified history
    /// </summary>

    public class CLI_SAVEHISTORY : AAllInBuildCLICommands
    {

        #region Constructor

        public CLI_SAVEHISTORY()
        {

            // Command name and description
            InitCommand("SAVEHISTORY",
                        "Saves the actual history to a specified file",
                        "Saves the actual history to a specified file");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralExternalEntry);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            //lulu

            return Exceptional.OK;

        }

        #endregion

    }

}
