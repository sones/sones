/*
 * GraphFS CLI - DU
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
    /// Shows used space on disc.
    /// </summary>

    public class FSCLI_DU : AllBasicFSCLICommands
    {
        
        #region Constructor

        public FSCLI_DU()
        {

            // Command name and description
            InitCommand("DU",
                        "Shows used space on disc",
                        "Shows used space on disc");

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

            WriteLine(Environment.NewLine + "Used space on the following devices:" + Environment.NewLine);

            var Size     = myAGraphDSSharp.GetNumberOfBytes(ObjectLocation.ParseString(FSPathConstants.PathDelimiter));
            var UsedSize = myAGraphDSSharp.GetNumberOfBytes(ObjectLocation.ParseString(FSPathConstants.PathDelimiter)) - myAGraphDSSharp.GetNumberOfFreeBytes(ObjectLocation.ParseString(FSPathConstants.PathDelimiter));
            var Procent  = (((Double)UsedSize) / ((Double)Size)) * (Double)100;
            WriteLine("{0,-12} {1,15} ({2:0.#}%)", FSPathConstants.PathDelimiter, UsedSize.ToByteFormattedString(2), Procent);

            foreach (var _Mountpoint in myAGraphDSSharp.GetChildFileSystemMountpoints(true))
            {
                Size     = myAGraphDSSharp.GetNumberOfBytes(_Mountpoint);
                UsedSize = myAGraphDSSharp.GetNumberOfBytes(_Mountpoint) - myAGraphDSSharp.GetNumberOfFreeBytes(_Mountpoint);
                Procent  = (((Double)UsedSize) / ((Double)Size)) * (Double)100;
                WriteLine("{0,-12} {1,15} ({2:0.#}%)", _Mountpoint, UsedSize.ToByteFormattedString(2), Procent);
            }

            WriteLine(Environment.NewLine);

            return Exceptional.OK;

        }

        #endregion

    }

}
