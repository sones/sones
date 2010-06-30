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


/* PandoraFS CLI - LL
 * (c) Henning Rauch, 2009
 * 
 * Shows all objects and their object streams within
 * the current directory
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
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Shows all objects and their object streams within
    /// the current directory
    /// </summary>

    public class FSCLI_LL : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_LL()
        {

            // Command name and description
            InitCommand("LL",
                        "Lists the extended content of a directory.",
                        "Lists the extended content of a directory.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
        {

            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No valid IGraphFSSession instance found!");
                return;
            }

            var _GetExtendedDirectoryListingExceptional = ((IGraphFSSession)_IGraphFSSession).GetExtendedDirectoryListing(new ObjectLocation(myCurrentPath));

            if (_GetExtendedDirectoryListingExceptional != null && _GetExtendedDirectoryListingExceptional.Success && _GetExtendedDirectoryListingExceptional.Value != null)
            {
                foreach (var _DirectoryEntry in _GetExtendedDirectoryListingExceptional.Value)
                {

                    String _ObjectStreamInfo = null;

                    foreach (var _ObjectStream in _DirectoryEntry.Streams)
                        if (_ObjectStreamInfo != null)
                            _ObjectStreamInfo = _ObjectStreamInfo + ", " + _ObjectStream;
                        else
                            _ObjectStreamInfo = _ObjectStream;

                    WriteLine("{0,-30} {1}", _DirectoryEntry.Name, _ObjectStreamInfo);

                }
            }

            else
            {
                foreach (var _IError in _GetExtendedDirectoryListingExceptional.Errors)
                    WriteLine(_IError.Message);
            }


            WriteLine();

        }

        #endregion

    }    

}
