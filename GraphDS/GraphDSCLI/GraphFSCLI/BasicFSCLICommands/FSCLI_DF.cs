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
