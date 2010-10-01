/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
