/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* PandoraFS CLI - DF
 * Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;

using sones.Lib;
using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;

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

        public override void Execute(ref object myIGraphFSSession, ref object myIGraphDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No file system mounted...");
                return;
            }

            WriteLine(Environment.NewLine + "Free space on the following devices:" + Environment.NewLine);

            var Size     = _IGraphFSSession.GetNumberOfBytes(new ObjectLocation(FSPathConstants.PathDelimiter));
            var FreeSize = _IGraphFSSession.GetNumberOfFreeBytes(new ObjectLocation(FSPathConstants.PathDelimiter));
            var Procent  = (((Double)FreeSize) / ((Double)Size)) * (Double)100;
            WriteLine("{0,-12} {1,15} ({2:0.#}%)", FSPathConstants.PathDelimiter, FreeSize.ToByteFormattedString(2), Procent);

            foreach (var _Mountpoint in _IGraphFSSession.GetChildFileSystemMountpoints(true))
            {
                Size = _IGraphFSSession.GetNumberOfBytes(new ObjectLocation(_Mountpoint));
                FreeSize = _IGraphFSSession.GetNumberOfFreeBytes(new ObjectLocation(_Mountpoint));
                Procent = (((Double)FreeSize) / ((Double)Size)) * (Double)100;
                WriteLine("{0,-12} {1,15} ({2:0.#}%)", _Mountpoint, FreeSize.ToByteFormattedString(2), Procent);
            }

            WriteLine(Environment.NewLine);

        }

        #endregion


    }

}
