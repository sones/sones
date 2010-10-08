/* GraphFS CLI - USEHISTORY
 * (c) Henning Rauch, 2009
 * 
 * Loads a specified history
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
    /// Loads a specified history
    /// </summary>

    public class CLI_USEHISTORY : AAllInBuildCLICommands
    {


        #region Constructor

        public CLI_USEHISTORY()
        {

            // Command name and description
            InitCommand("USEHISTORY",
                        "Saves the actual history to a specified file",
                        "Saves the actual history to a specified file");


            NonTerminal USEHISTORY_Options = new NonTerminal("USEHISTORY_Options");
            SymbolTerminal USEHISTORY_Default_Option = Symbol("default");

            USEHISTORY_Options.Rule = stringLiteralExternalEntry
                                        | USEHISTORY_Default_Option;
            USEHISTORY_Options.GraphOptions.Add(GraphOption.IsOption);

            _CommandNonTerminals.Add(USEHISTORY_Options);

            _CommandSymbolTerminal.Add(USEHISTORY_Default_Option);

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + USEHISTORY_Options);


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
