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
