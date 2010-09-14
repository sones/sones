/* <id name=”GraphLib – Autocompletion” />
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
using sones.GraphDS.API.CSharp;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    /// <summary>
    /// This class implements the autocompletion
    /// for PVFS-external files or directories.
    /// </summary>

    public class StringLiteral_ExternalEntry : AInBuildAutocompletions
    {

        #region Properties

        public override String Name { get { return "stringLiteral_ExternalEntry"; } }

        #endregion

        #region completion method

        public override List<String> Complete(AGraphDSSharp myGraphDSSharp, ref String CurrentPath, string CurrentStringLiteral)
        {

            #region Data

            List<String> possibleExternalEntries = new List<String>();
            String[]     DirsInDirectory         = null;
            String[]     FilesInDirectory        = null;

            #endregion

            if (CurrentStringLiteral.Length.Equals(0))
            {
                DirsInDirectory  = Directory.GetDirectories(".");
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
                        DirsInDirectory  = Directory.GetDirectories(".");
                        FilesInDirectory = Directory.GetFiles(".");
                    }
                    else
                    {
                        DirsInDirectory  = Directory.GetDirectories(PrefixDir);
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
                        DirsInDirectory  = Directory.GetDirectories(PrefixDir);
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
