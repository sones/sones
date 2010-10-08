/*
 * GraphFS CLI - DS
 * (c) Achim Friedland, 2009
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDS.Connectors.CLI;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Shows the size of a disc
    /// </summary>

    public class FSCLI_DS : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_DS()
        {

            // Command name and description
            InitCommand("DS",
                        "Shows the size of a disc",
                        "Shows the size of a disc");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal | CLICommandSymbolTerminal + stringLiteralPVFS);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            WriteLine(Environment.NewLine + "Total disc size of the following devices:" + Environment.NewLine);

            var Size = myAGraphDSSharp.GetNumberOfBytes(ObjectLocation.Root);
            WriteLine("{0,-12} {1,15} Bytes", FSPathConstants.PathDelimiter, Size.ToByteFormattedString(2));

            foreach (var _Mountpoint in myAGraphDSSharp.GetChildFileSystemMountpoints(true))
            {
                Size = myAGraphDSSharp.GetNumberOfBytes(_Mountpoint);
                WriteLine("{0,-12} {1,15} ", _Mountpoint, Size.ToByteFormattedString(2));
            }

            WriteLine(Environment.NewLine);

            return Exceptional.OK;

        }

        #endregion

    }

}
