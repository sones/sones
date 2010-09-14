/*
 * GraphFS CLI - DF
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
    /// Shows free space on disc
    /// </summary>

    public class FSCLI_DF : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_DF()
        {

            // Command name and description
            InitCommand("DF",
                        "Shows free space on disc",
                        "Shows free space on disc");

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

            WriteLine(Environment.NewLine + "Free space on the following devices:" + Environment.NewLine);

            var Size     = myAGraphDSSharp.GetNumberOfBytes(ObjectLocation.Root);
            var FreeSize = myAGraphDSSharp.GetNumberOfFreeBytes(ObjectLocation.Root);
            var Procent  = (((Double)FreeSize) / ((Double)Size)) * (Double)100;
            WriteLine("{0,-12} {1,15} ({2:0.#}%)", FSPathConstants.PathDelimiter, FreeSize.ToByteFormattedString(2), Procent);

            foreach (var _Mountpoint in myAGraphDSSharp.GetChildFileSystemMountpoints(true))
            {
                Size     = myAGraphDSSharp.GetNumberOfBytes(_Mountpoint);
                FreeSize = myAGraphDSSharp.GetNumberOfFreeBytes(_Mountpoint);
                Procent  = (((Double)FreeSize) / ((Double)Size)) * (Double)100;
                WriteLine("{0,-12} {1,15} ({2:0.#}%)", _Mountpoint, FreeSize.ToByteFormattedString(2), Procent);
            }

            WriteLine(Environment.NewLine);

            return Exceptional.OK;

        }

        #endregion

    }

}
