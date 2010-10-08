/*
 * DSCLI_DSINFO
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.Connectors.GraphDSCLI;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Information on this GraphDS instance
    /// </summary>
    public class DSCLI_DSINFO : AllGraphDSCLICommands
    {

        #region Constructor

        public DSCLI_DSINFO()
        {

            // Command name and description
            InitCommand("DSINFO",
                        "Information on this GraphDS instance.",
                        "Information on this GraphDS instance.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            //var _IndividualCDCommands = myOptions.ElementAt(1).Value[0].Option.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.None);

            //if (_IndividualCDCommands[0].Equals(""))
            //    _IndividualCDCommands[0] = FSPathConstants.PathDelimiter;

            WriteLine("Hello world!");

            return Exceptional.OK;

        }

        #endregion

    }

}
