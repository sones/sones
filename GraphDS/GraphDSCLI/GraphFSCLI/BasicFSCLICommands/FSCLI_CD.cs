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
 * GraphFS CLI - CD
 * (c) Henning Rauch, 2009
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            var _IndividualCDCommands = myOptions.ElementAt(1).Value[0].Option.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.None);

            if (_IndividualCDCommands[0].Equals(""))
                _IndividualCDCommands[0] = FSPathConstants.PathDelimiter;


            foreach (var _ActualCDCommand in _IndividualCDCommands)
            {
                if (!_ActualCDCommand.Equals("."))
                    if (!(myCurrentPath.Equals(FSPathConstants.PathDelimiter) && _ActualCDCommand.Equals("..")))
                        CD_private(myAGraphDSSharp, ref myCurrentPath, _ActualCDCommand);
            }

            return Exceptional.OK;

        }

        #endregion

        #region (private) CDCommand_private (myIGraphFSSession, myCurrentPath, myParameter)

        private void CD_private(IGraphFSSession myIGraphFSSession, ref String myCurrentPath, String myParameter)
        {

            var _DirectoryObjectLocation = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myParameter);

            try
            {

                if ((myIGraphFSSession).ObjectStreamExists(_DirectoryObjectLocation, FSConstants.DIRECTORYSTREAM).Value ||
                    (myIGraphFSSession).ObjectStreamExists(_DirectoryObjectLocation, FSConstants.VIRTUALDIRECTORY).Value)
                {

                    if (myCurrentPath.Equals(FSPathConstants.PathDelimiter) && _DirectoryObjectLocation.Equals("/.."))
                        myCurrentPath = FSPathConstants.PathDelimiter;

                    else
                        myCurrentPath = SimplifyObjectLocation(_DirectoryObjectLocation.ToString());

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
