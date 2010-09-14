/*
 * GraphFS CLI - LL
 * (c) Henning Rauch, 2009
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDS.Connectors.CLI;

using sones.Lib.ErrorHandling;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Shows all objects and their object streams within
    /// the current directory
    /// </summary>

    public class FSCLI_LL : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_LL()
        {

            // Command name and description
            InitCommand("LL",
                        "Lists the extended content of a directory.",
                        "Lists the extended content of a directory.");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            var _GetExtendedDirectoryListingExceptional = myAGraphDSSharp.GetExtendedDirectoryListing(ObjectLocation.ParseString(myCurrentPath));

            if (_GetExtendedDirectoryListingExceptional.Success() && _GetExtendedDirectoryListingExceptional.Value != null)
            {
                foreach (var _DirectoryEntry in _GetExtendedDirectoryListingExceptional.Value)
                {

                    String _ObjectStreamInfo = null;

                    foreach (var _ObjectStream in _DirectoryEntry.Streams)
                        if (_ObjectStreamInfo != null)
                            _ObjectStreamInfo = _ObjectStreamInfo + ", " + _ObjectStream;
                        else
                            _ObjectStreamInfo = _ObjectStream;

                    WriteLine("{0,-30} {1}", _DirectoryEntry.Name, _ObjectStreamInfo);

                }
            }

            else
            {
                foreach (var _IError in _GetExtendedDirectoryListingExceptional.IErrors)
                    WriteLine(_IError.Message);
            }


            WriteLine();

            return Exceptional.OK;

        }

        #endregion

    }    

}
