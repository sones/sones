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

/* <id name=”PandoraLib – Autocompletion” />
 * <copyright file=”ExternalEntryAC.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements the autocompletion
 * for PVFS-external files or directories.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace sones.Lib.CLI
{

    /// <summary>
    /// This class implements the autocompletion
    /// for PVFS-external files or directories.
    /// </summary>

    public class StringLiteral_ExternalEntry : AInBuildAutocompletions
    {

        #region properties

        public override String Name { get { return "stringLiteral_ExternalEntry"; } }

        #endregion

        #region completion method

        public override List<String> Complete(ref object PVFSObject, ref object PDBObject, ref String CurrentPath, string CurrentStringLiteral)
        {
            #region Data
            List<String> possibleExternalEntries = new List<string>();
            String[] DirsInDirectory = null;
            String[] FilesInDirectory = null;
            #endregion

            if (CurrentStringLiteral.Length.Equals(0))
            {
                DirsInDirectory = Directory.GetDirectories(".");
                FilesInDirectory = Directory.GetFiles(".");
            }
            else
            {
                int indexOfLastPathDelimitter = CurrentStringLiteral.LastIndexOf("/");
                String PrefixDir = CurrentStringLiteral.Substring(0, indexOfLastPathDelimitter + 1);

                #region Handle Directories

                if (CurrentStringLiteral.StartsWith("."))
                {
                    #region relative
                    if (PrefixDir.Length.Equals(0))
                    {
                        DirsInDirectory = Directory.GetDirectories(".");
                        FilesInDirectory = Directory.GetFiles(".");
                    }
                    else
                    {
                        DirsInDirectory = Directory.GetDirectories(PrefixDir);
                        FilesInDirectory = Directory.GetFiles(PrefixDir);
                    }

                    #endregion
                }
                else
                {
                    #region absolut
                    String drivePattern = "^[a-z,A-Z]:/";
                    Regex regex = new Regex(drivePattern);

                    if (regex.IsMatch(CurrentStringLiteral))
                    {
                        DirsInDirectory = Directory.GetDirectories(PrefixDir);
                        FilesInDirectory = Directory.GetFiles(PrefixDir);
                    }

                    #endregion
                }
                #endregion
            }



            if (DirsInDirectory != null)
            {
                foreach (String aDirElement in DirsInDirectory)
                {
                    possibleExternalEntries.Add(aDirElement.Replace("\\", "/") + "/");
                }
            }

            if (FilesInDirectory != null)
            {
                foreach (String aFileElement in FilesInDirectory)
                {
                    possibleExternalEntries.Add(aFileElement.Replace("\\", "/"));
                }
            }

            return possibleExternalEntries;
        }

        #endregion


    }

}
