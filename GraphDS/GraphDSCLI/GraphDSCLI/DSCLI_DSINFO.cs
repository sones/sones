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
 * DSCLI_DSINFO
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.Connectors.GraphDSCLI;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;
using sones.Lib.ErrorHandling;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            //var _IndividualCDCommands = myOptions.ElementAt(1).Value[0].Option.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.None);

            //if (_IndividualCDCommands[0].Equals(""))
            //    _IndividualCDCommands[0] = FSPathConstants.PathDelimiter;

            WriteLine("Hello world!");

            return Exceptional.OK;

        }

        #endregion

    }

}
