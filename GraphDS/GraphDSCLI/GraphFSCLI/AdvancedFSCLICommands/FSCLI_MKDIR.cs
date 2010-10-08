/*
 * GraphFS CLI - MKDIR
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDS.Connectors.CLI;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Creates a new directory (command category: AdvancedFSCLICommand)
    /// </summary>
    public class FSCLI_MKDIR : AllAdvancedFSCLICommands
    {

        #region Constructor

        public FSCLI_MKDIR()
        {

            // Command name and description
            InitCommand("MKDIR",
                        "Creates a new directory.",
                        "Creates a new directory.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralPVFS);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            ObjectLocation _DirectoryObjectLocation = null;

            try
            {
                _DirectoryObjectLocation = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myOptions.ElementAt(1).Value[0].Option);
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                return Exceptional.OK;
            }

            try
            {
                myAGraphDSSharp.CreateDirectoryObject(_DirectoryObjectLocation);
            }

            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            return Exceptional.OK;

        }

        #endregion

    }

}
