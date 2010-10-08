/*
 * GraphFS CLI - CAT
 * (c) Achim Friedland, 2009
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphFS.InternalObjects;
using sones.GraphDS.Connectors.CLI;
using sones.GraphFS.Errors;
using sones.GraphDS.API.CSharp;
using sones.Lib.ErrorHandling;

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

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            var            _tmpObjectLocation              = new ObjectLocation(ObjectLocation.ParseString(myCurrentPath), myOptions.ElementAt(1).Value[0].Option);
            ObjectLocation _ObjectLocation;
            var            _ObjectStreamDelimiterPosition  = _tmpObjectLocation.IndexOf(FSConstants.ObjectStreamDelimiter);
            var            _ObjectStreamTypes              = new List<String>();

            if (_ObjectStreamDelimiterPosition > 0)
            {
                _ObjectLocation     = ObjectLocation.ParseString(_tmpObjectLocation.Substring(0, _ObjectStreamDelimiterPosition));
                _ObjectStreamTypes.Add(_tmpObjectLocation.Substring(_ObjectStreamDelimiterPosition + FSConstants.ObjectStreamDelimiter.Length));
            }

            else
            {
                _ObjectLocation     = _tmpObjectLocation;
                _ObjectStreamTypes  = new List<String>(myAGraphDSSharp.GetObjectStreams(_ObjectLocation).Value);
            }

            foreach (var _ObjectStream in _ObjectStreamTypes)
            {

                WriteLine(_ObjectStream + ":");
                WriteLine("");

                switch (_ObjectStream)
                {

                    #region SYSTEMMETADATASTREAM

                    //case FSConstants.SYSTEMMETADATASTREAM:
                    //    foreach (var _KeyValuePair in myAGraphDSSharp.GetSystemMetadata(new ObjectLocation(_ObjectLocation)))
                    //        foreach (var val in _KeyValuePair.Value)
                    //            WriteLine("{0,-25} = {1}", _KeyValuePair.Key, val);
                    //    break;

                    #endregion

                    #region USERMETADATASTREAM

                    //case FSConstants.USERMETADATASTREAM:
                    //    foreach (var _KeyValuePair in myAGraphDSSharp.GetUserMetadata(new ObjectLocation(_ObjectLocation)).Value)
                    //        foreach (var val in _KeyValuePair.Value)
                    //            WriteLine("{0,-25} = {1}", _KeyValuePair.Key, val);
                    //    break;

                    #endregion

                    #region UNDEFINEDATTRIBUTESSTREAM

                    case "UNDEFINEDATTRIBUTESSTREAM":
                        foreach (var _KeyValuePair in myAGraphDSSharp.GetMetadata<Object>(_ObjectLocation, "UNDEFINEDATTRIBUTESSTREAM", FSConstants.DefaultEdition).Value)
                       //    foreach (var val in _KeyValuePair.Value)
                                WriteLine("{0,-25} = {1}", _KeyValuePair.Key, _KeyValuePair.Value);
                        break;

                    #endregion

                    #region LISTOF_STRINGS

                    case FSConstants.LISTOF_STRINGS:
                        foreach (var _String in myAGraphDSSharp.GetFSObject<ListOfStringsObject>(_ObjectLocation).Value)
                            WriteLine(_String);
                        break;

                    #endregion

                    #region FILESTREAM

                    case FSConstants.FILESTREAM:
                        var FileContent = Encoding.UTF8.GetString(myAGraphDSSharp.GetFSObject<FileObject>(_ObjectLocation, FSConstants.FILESTREAM, null, null, 0, false).Value.ObjectData);
                        WriteLine(FileContent);
                        break;

                    #endregion

                    #region INLINEDATA

                    case FSConstants.INLINEDATA:
                        var _Inlinedata = myAGraphDSSharp.GetFSObject<DirectoryObject>(_ObjectLocation.Path).Value.GetInlineData(_ObjectLocation.Name);

                        if (_ObjectLocation.Name.Equals(FSConstants.DotUUID))
                            WriteLine((new ObjectUUID(_Inlinedata)).ToString());

                        else
                            WriteLine(_Inlinedata.ToString());

                        break;

                    #endregion

                }

                WriteLine();

            }

            return Exceptional.OK;

        }

        #endregion

    }

}
