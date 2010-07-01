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


/*
 * GraphDSCLI - DSINFO
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.Connectors.GraphDSCLI;

using sones.Lib.CLI;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Information on this GraphDS instance
    /// </summary>
    public class DSCLI_DSINFO : AllGraphDSCLICommands
    {

        #region Constructor

        public DSCLI_DSINFO()
        {

            // Command name and description
            InitCommand("DSINFO",
                        "Information on this GraphDS instance.",
                        "Information on this GraphDS instance.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIGraphDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No valid IGraphFSSession instance found!");
                return;
            }

            var _IGraphDBSession = myIGraphDBSession as IGraphFSSession;

            if (_IGraphDBSession == null)
            {
                WriteLine("No valid IGraphDBSession instance found!");
                return;
            }

            //var _IndividualCDCommands = myOptions.ElementAt(1).Value[0].Option.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.None);

            //if (_IndividualCDCommands[0].Equals(""))
            //    _IndividualCDCommands[0] = FSPathConstants.PathDelimiter;

            WriteLine("Hello world!");

        }

        #endregion


    }

}
