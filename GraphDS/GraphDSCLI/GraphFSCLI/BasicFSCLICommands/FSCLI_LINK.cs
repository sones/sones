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
 * GraphFS CLI - LINK
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDS.Connectors.CLI;
using sones.Lib.DataStructures;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;
using sones.Lib.ErrorHandling;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            // 1 parameter -> Print the target of the symlink
            if (myOptions.Count == 2)
            {

                var SymlinkLocation = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myOptions.ElementAt(1).Value[0].Option);

                if (myAGraphDSSharp.isSymlink(SymlinkLocation).Value == Trinary.TRUE)
                    WriteLine(" -> " + myAGraphDSSharp.GetSymlink(SymlinkLocation));

                else
                    WriteLine("Symlink does not exist!");

            }

            // 2 parameters -> Create new symlink
            else
            {

                var SymlinkLocation = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myOptions.ElementAt(1).Value[0].Option);
                var SymlinkTarget   = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myOptions.ElementAt(2).Value[0].Option);

                myAGraphDSSharp.AddSymlink(SymlinkLocation, SymlinkTarget);

                WriteLine(SymlinkLocation.ToString() + " -> " + SymlinkTarget.ToString());

            }

            return Exceptional.OK;

        }

        #endregion

    }

}
