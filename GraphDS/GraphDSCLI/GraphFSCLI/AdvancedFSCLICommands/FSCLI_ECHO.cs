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
