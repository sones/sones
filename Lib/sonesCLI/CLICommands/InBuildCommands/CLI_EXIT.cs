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


/* PandoraFS CLI - EXIT
 * (c) Henning Rauch, 2009
 * 
 * Exits the CLI
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.Lib.CLI
{

    /// <summary>
    /// Exits the CLI
    /// </summary>

    public class CLI_EXIT : AAllInBuildCLICommands
    {

        #region Constructor

        public CLI_EXIT()
        {

            // Command name and description
            InitCommand("EXIT",
                        "Exits the CLI",
                        "Exits the CLI");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
        {
            WriteLine("Now exiting...");
        }

        #endregion

    }

}
