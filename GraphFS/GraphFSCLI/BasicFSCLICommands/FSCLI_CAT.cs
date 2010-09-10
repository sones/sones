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

/* PandoraFS CLI - CAT
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
using System.IO;

using sones.Lib;
using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.GraphFS.Session;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.InternalObjects;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Shows the content of an object
    /// </summary>

    public class FSCLI_CAT : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_CAT()
        {

            // Command name and description
            InitCommand("CAT",
                        "Shows the content of an object",
                        "Shows the content of an object");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + stringLiteralPVFS);

        }

        #endregion


        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No file system mounted...");
                return;
            }

            String _tmpObjectLocation              = GetObjectLocation(myCurrentPath, myOptions.ElementAt(1).Value[0].Option);
            String _ObjectLocation                 = String.Empty;
            var    _ObjectStreamDelimiterPosition  = _tmpObjectLocation.IndexOf(FSConstants.ObjectStreamDelimiter);
            var    _ObjectStreamTypes              = new List<String>();

            if (_ObjectStreamDelimiterPosition > 0)
            {
                _ObjectLocation     = _tmpObjectLocation.Substring(0, _ObjectStreamDelimiterPosition);
                _ObjectStreamTypes.Add(_tmpObjectLocation.Substring(_ObjectStreamDelimiterPosition + FSConstants.ObjectStreamDelimiter.Length));
            }

            else
            {
                _ObjectLocation     = _tmpObjectLocation;
                _ObjectStreamTypes  = new List<String>(_IGraphFSSession.GetObjectStreams(new ObjectLocation(_ObjectLocation)).Value);
            }

            var _ObjectPath = DirectoryHelper.GetObjectPath(_ObjectLocation);
            var _ObjectName = DirectoryHelper.GetObjectName(_ObjectLocation);

            foreach (var _ObjectStream in _ObjectStreamTypes)
            {

                WriteLine(_ObjectStream + ":");
                WriteLine("");

                switch (_ObjectStream)
                {

                    #region SYSTEMMETADATASTREAM

                    //case FSConstants.SYSTEMMETADATASTREAM:
                    //    foreach (var _KeyValuePair in _IGraphFSSession.GetSystemMetadata(new ObjectLocation(_ObjectLocation)))
                    //        foreach (var val in _KeyValuePair.Value)
                    //            WriteLine("{0,-25} = {1}", _KeyValuePair.Key, val);
                    //    break;

                    #endregion

                    #region USERMETADATASTREAM

                    //case FSConstants.USERMETADATASTREAM:
                    //    foreach (var _KeyValuePair in _IGraphFSSession.GetUserMetadata(new ObjectLocation(_ObjectLocation)))
                    //        foreach (var val in _KeyValuePair.Value)
                    //            WriteLine("{0,-25} = {1}", _KeyValuePair.Key, val);
                    //    break;

                    #endregion

                    #region LISTOF_STRINGS

                    case FSConstants.LISTOF_STRINGS:
                        foreach (var _String in _IGraphFSSession.GetObject<ListOfStringsObject>(new ObjectLocation(_ObjectLocation)).Value)
                            WriteLine(_String);
                        break;

                    #endregion

                    #region FILESTREAM

                    case FSConstants.FILESTREAM:
                        var FileContent = Encoding.UTF8.GetString(_IGraphFSSession.GetObject<FileObject>(new ObjectLocation(_ObjectLocation), FSConstants.FILESTREAM, null, null, 0, false).Value.ObjectData);
                        WriteLine(FileContent);
                        break;

                    #endregion

                    #region INLINEDATA

                    case FSConstants.INLINEDATA:
                        var _Inlinedata = _IGraphFSSession.GetObject<DirectoryObject>(new ObjectLocation(_ObjectPath)).Value.GetInlineData(_ObjectName);

                        if (_ObjectName.Equals(FSConstants.DotUUID))
                            WriteLine((new ObjectUUID(_Inlinedata)).ToString());

                        else
                            WriteLine(_Inlinedata.ToString());

                        break;

                    #endregion

                }

                WriteLine();

            }

        }

        #endregion

    }

}
