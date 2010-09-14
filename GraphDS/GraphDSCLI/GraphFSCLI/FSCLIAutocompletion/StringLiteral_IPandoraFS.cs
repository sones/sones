/* <id Name=”GraphLib – Autocompletion” />
 * <copyright file=”PVFSAC.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements the autocompletion
 * for PVFS files or directories.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Lib.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;
using sones.GraphDS.API.CSharp;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// This class implements the autocompletion
    /// for PVFS files or directories.
    /// </summary>

    public class StringLiteral_IGraphFS : AGraphFSCLIAutocompletions
    {

        #region properties

        public override String Name { get { return "stringLiteral_IGraphFS"; } }

        #endregion

        #region completion method

        public override List<String> Complete(AGraphDSSharp myGraphDSSharp, ref String CurrentPath, string CurrentStringLiteral)
        {

            #region Data

            List<String>        _PossibleFSEntries  = new List<String>();
            IEnumerable<String> _DirectoryElements  = null;

            #endregion

            if (CurrentStringLiteral.Length.Equals(0))
            {
                try
                {
                    _DirectoryElements = myGraphDSSharp.GetDirectoryListing(ObjectLocation.ParseString(CurrentPath)).Value;
                }
                catch
                {
                    //do nothing
                }

                if (_DirectoryElements != null)
                {
                    _PossibleFSEntries.AddRange(_DirectoryElements);
                }
            }
            else
            {
                #region extract dir prefix from CurrentStringLiteral

                int indexOfLastPathDelimitter = CurrentStringLiteral.LastIndexOf(FSPathConstants.PathDelimiter);
                String PrefixDir = CurrentStringLiteral.Substring(0, indexOfLastPathDelimitter + 1);

                if (CurrentStringLiteral.StartsWith(FSPathConstants.PathDelimiter))
                {
                    Boolean directoryExists = false;
                    try
                    {
                        directoryExists = myGraphDSSharp.isIDirectoryObject(ObjectLocation.ParseString(SimplifyObjectLocation(PrefixDir))).Value == Trinary.TRUE;
                    }
                    catch
                    {
                        //do nothing
                    }
                    if (directoryExists)
                    {

                        try
                        {
                            _DirectoryElements = myGraphDSSharp.GetDirectoryListing(ObjectLocation.ParseString(SimplifyObjectLocation(PrefixDir))).Value;
                        }
                        catch
                        {
                            //do nothing
                        }

                        if (_DirectoryElements != null)
                        {

                            foreach (String aDirElement in _DirectoryElements)
                            {
                                _PossibleFSEntries.Add(PrefixDir + aDirElement);
                            }

                        }
                    }
                }
                else
                {
                    if (PrefixDir.Length.Equals(0))
                    {
                        try
                        {
                            _DirectoryElements = myGraphDSSharp.GetDirectoryListing(ObjectLocation.ParseString(CurrentPath)).Value;
                        }
                        catch
                        {
                            //do nothing
                        }

                        if (_DirectoryElements != null)
                        {
                            _PossibleFSEntries.AddRange(_DirectoryElements);
                        }
                    }
                    else
                    {
                        try
                        {
                            _DirectoryElements = myGraphDSSharp.GetDirectoryListing(ObjectLocation.ParseString(SimplifyObjectLocation(CurrentPath + FSPathConstants.PathDelimiter + PrefixDir))).Value;
                        }
                        catch
                        {
                            //do nothing
                        }

                        if (_DirectoryElements != null)
                        {
                            foreach (String aDirElement in _DirectoryElements)
                            {
                                _PossibleFSEntries.Add(PrefixDir + aDirElement);
                            }
                        }
                    }
                }



                #endregion
            }

            return _PossibleFSEntries;
        }

        #endregion



        #region SimplifyObjectLocation(myCurrentPath)

        /// <summary>
        /// Simplifies an object location by removing relative path fragments
        /// like /directoryname1/../directoryname2 -> /directoryname2
        /// </summary>
        /// <param name="myCurrentPath">the current path</param>
        /// <returns>a simplified current path</returns>
        public String SimplifyObjectLocation(String myCurrentPath)
        {

            String _newPath = "";
            String[] _PathSeperator = { FSPathConstants.PathDelimiter, "" };
            String[] _SplittedCurrentPath = myCurrentPath.Split(_PathSeperator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < _SplittedCurrentPath.Length; i++)
            {

                if (_SplittedCurrentPath[i].Equals(FSConstants.DotDotLink))
                {
                    _SplittedCurrentPath[i] = "";
                    _SplittedCurrentPath[i - 1] = "";
                }

                else if (_SplittedCurrentPath[i].Equals(FSPathConstants.PathDelimiter))
                {
                    _SplittedCurrentPath[i] = "";
                }

            }

            foreach (var _Path in _SplittedCurrentPath)
                if (!_Path.Equals("")) _newPath = String.Concat(_newPath, FSPathConstants.PathDelimiter, _Path);

            if (_newPath.Equals(""))
                _newPath = FSPathConstants.PathDelimiter;

            return _newPath;

        }

        #endregion


    }

}
