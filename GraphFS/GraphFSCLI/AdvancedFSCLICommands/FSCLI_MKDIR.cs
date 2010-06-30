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


/* PandoraFS CLI - MKDIR
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

using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.Lib.CLI;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Creates a new directory (command category: AdvancedFSCLICommand)
    /// </summary>
    public class FSCLI_MKDIR : AllAdvancedFSCLICommands
    {

        #region Constructor

        public FSCLI_MKDIR()
        {

            // Command name and description
            InitCommand("MKDIR",
                        "Creates a new directory.",
                        "Creates a new directory.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralPVFS);

        }

        #endregion


        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
        {

            String _DirectoryObjectLocation = "";

            try
            {
                _DirectoryObjectLocation = GetObjectLocation(myCurrentPath, myOptions.ElementAt(1).Value[0].Option);
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                //do nothing
            }

            if (_DirectoryObjectLocation.Length.Equals(0))
            {
                //Todo: add some errorhandling
            }
            else
            {

                try
                {
                    ((IGraphFSSession)myIGraphFSSession).CreateDirectoryObject(new ObjectLocation(_DirectoryObjectLocation));
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }
            }

        }

        #endregion

    }

}
