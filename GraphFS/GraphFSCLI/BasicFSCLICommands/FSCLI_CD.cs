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


/* PandoraFS CLI - CD
 * (c) Henning Rauch, 2009
 * 
 * Changes the current directory
 * 
 * Lead programmer:
 *      Henning Rauch
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
    /// Changes the current directory
    /// </summary>

    public class FSCLI_CD : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_CD()
        {

            // Command name and description
            InitCommand("CD",
                        "Changes the current directory.",
                        "Changes the current directory.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralPVFS);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIGraphDBSession, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
        {

            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No valid IGraphFSSession instance found!");
                return;
            }

            var _IndividualCDCommands = myOptions.ElementAt(1).Value[0].Option.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.None);

            if (_IndividualCDCommands[0].Equals(""))
                _IndividualCDCommands[0] = FSPathConstants.PathDelimiter;


            foreach (var _ActualCDCommand in _IndividualCDCommands)
            {
                if (!_ActualCDCommand.Equals("."))
                    if (!(myCurrentPath.Equals(FSPathConstants.PathDelimiter) && _ActualCDCommand.Equals("..")))
                        CD_private(ref myIGraphFSSession, ref myCurrentPath, _ActualCDCommand);
            }

        }

        #endregion

        #region (private) CDCommand_private (myIGraphFSSession, myCurrentPath, myParameter)

        private void CD_private(ref Object myIGraphFSSession, ref String myCurrentPath, String myParameter)
        {

            var _DirectoryObjectLocation = GetObjectLocation(myCurrentPath, myParameter);

            try
            {

                if (((IGraphFSSession)myIGraphFSSession).ObjectStreamExists(new ObjectLocation(_DirectoryObjectLocation), FSConstants.DIRECTORYSTREAM).Value ||
                    ((IGraphFSSession)myIGraphFSSession).ObjectStreamExists(new ObjectLocation(_DirectoryObjectLocation), FSConstants.VIRTUALDIRECTORY).Value)
                {

                    if (myCurrentPath.Equals(FSPathConstants.PathDelimiter) && _DirectoryObjectLocation.Equals("/.."))
                        myCurrentPath = FSPathConstants.PathDelimiter;

                    else
                        myCurrentPath = SimplifyObjectLocation(_DirectoryObjectLocation);

                }
                else
                    WriteLine("Sorry, this directory does not exist!");

            }

            catch (Exception e)
            {
                WriteLine(e.Message);
                WriteLine(e.StackTrace);
            }

        }

        #endregion

    }

}
