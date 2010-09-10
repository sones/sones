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
 * GraphDSExtensionsCLI
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

using sones.GraphDS.API.CSharp;
using sones.GraphDB.Connectors.GraphDBCLI;
using sones.GraphFS.Connectors.GraphFSCLI;
using sones.GraphFS.Connectors.GraphDSCLI;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// Extension methods to start the GraphDS CLI (command line interface).
    /// </summary>
    public static class GraphDSExtensionsCLI
    {

        #region OpenCLI()

        /// <summary>
        /// Starts the GraphDS command line interface using the default commands.
        /// </summary>
        public static void OpenCLI(this AGraphDSSharp myAGraphDSSharp)
        {

            var _GraphDBCLI = new GraphCLI(
                                myAGraphDSSharp,
                                myAGraphDSSharp.DatabaseName,
                                typeof(AllBasicFSCLICommands),
                                typeof(AllAdvancedFSCLICommands),
                                typeof(AllBasicDBCLICommands),
                                typeof(AllAdvancedDBCLICommands),
                                typeof(AllGraphDSCLICommands)
                              );

            _GraphDBCLI.Run();

        }

        #endregion

        #region OpenCLI(params myCommandTypes)

        /// <summary>
        /// Starts the GraphDS command line interface using the given array of command types.
        /// </summary>
        /// <param name="myCommandTypes">The array of command types to load at CLI start-up.</param>
        public static void OpenCLI(this AGraphDSSharp myAGraphDSSharp, params Type[] myCommandTypes)
        {

            var _GraphDBCLI = new GraphCLI(
                                myAGraphDSSharp,
                                myAGraphDSSharp.DatabaseName,
                                myCommandTypes
                              );

            _GraphDBCLI.Run();

        }

        #endregion

    }

}
