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
 * GraphFS CLI - ECHO
 * (c) Achim Friedland, 2009
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.DataStructures;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            var _Content        = myOptions.ElementAt(1).Value[0].Option;
            var _FileLocation   = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myOptions.ElementAt(2).Value[0].Option);

            if (myAGraphDSSharp.ObjectExists(_FileLocation).Value != Trinary.TRUE)
            {

                try
                {
                    AFSObject _FileObject = new FileObject() { ObjectLocation = _FileLocation, ObjectData = Encoding.UTF8.GetBytes(_Content)};
                    myAGraphDSSharp.StoreFSObject(_FileObject, true);
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }

            }

            else
            {

                if (myAGraphDSSharp.ObjectStreamExists(_FileLocation, FSConstants.INLINEDATA).Value)
                {

                }

            }

            return Exceptional.OK;

        }

        #endregion

    }

}
