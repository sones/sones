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

/* PandoraFS CLI - LINK
 * (c) Achim Friedland, 2009
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
    /// Create a new symlink or prints the target of a symlink
    /// </summary>

    public class FSCLI_LINK : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_LINK()
        {

            // Command name and description
            InitCommand("LINK",
                        "Create a new symlink or prints the target of a symlink",
                        "Create a new symlink or prints the target of a symlink");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralPVFS | CLICommandSymbolTerminal + stringLiteralPVFS + stringLiteralPVFS);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No file system mounted...");
                return;
            }

            // 1 parameter -> Print the target of the symlink
            if (myOptions.Count == 2)
            {

                var SymlinkLocation = GetObjectLocation(myCurrentPath, myOptions.ElementAt(1).Value[0].Option);

                if (_IGraphFSSession.isSymlink(new ObjectLocation(SymlinkLocation)).Value == Trinary.TRUE)
                    WriteLine(" -> " + _IGraphFSSession.GetSymlink(new ObjectLocation(SymlinkLocation)));

                else
                    WriteLine("Symlink does not exist!");

            }

            // 2 parameters -> Create new symlink
            else
            {

                var SymlinkLocation = GetObjectLocation(myCurrentPath, myOptions.ElementAt(1).Value[0].Option);
                var SymlinkTarget = GetObjectLocation(myCurrentPath, myOptions.ElementAt(2).Value[0].Option);

                _IGraphFSSession.AddSymlink(new ObjectLocation(SymlinkLocation), new ObjectLocation(SymlinkTarget));

                WriteLine(SymlinkLocation + " -> " + SymlinkTarget);

            }

        }

        #endregion

    }

}
