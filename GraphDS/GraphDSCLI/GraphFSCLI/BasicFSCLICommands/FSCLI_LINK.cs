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
