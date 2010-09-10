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

/* PandoraFS CLI - ECHO
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

using sones.Lib;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;


#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Writes a string into a file.
    /// </summary>

    public class FSCLI_ECHO : AllAdvancedFSCLICommands
    {


        #region Constructor

        public FSCLI_ECHO()
        {

            // Command name and description
            InitCommand("ECHO",
                        "Writes a string into a file.",
                        "Writes a string into a file.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteral + stringLiteralPVFS);

        }

        #endregion


        #region Execute Command

        public override void Execute(ref object myPFSObject, ref object myPDBObject, ref String myCurrentPath, Dictionary<string, List<AbstractCLIOption>> myOptions, string myInputString)
        {

            var _Content        = myOptions.ElementAt(1).Value[0].Option;
            var _FileLocation   = new ObjectLocation(GetObjectLocation(myCurrentPath, myOptions.ElementAt(2).Value[0].Option));

            if (((IGraphFSSession)myPFSObject).ObjectExists(_FileLocation).Value != Trinary.TRUE)
            {

                try
                {
                    AFSObject _FileObject = new FileObject() { ObjectLocation = _FileLocation, ObjectData = Encoding.UTF8.GetBytes(_Content)};
                    ((IGraphFSSession)myPFSObject).StoreFSObject(_FileObject, true);

                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }

            }

            else
            {

                if (((IGraphFSSession)myPFSObject).ObjectStreamExists(_FileLocation, FSConstants.INLINEDATA).Value)
                {

                }


            }





        }

        #endregion

    }

}
