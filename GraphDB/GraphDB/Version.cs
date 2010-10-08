/* <id name="GraphDB – Version Information" />
 * <copyright file="Version.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace sones.GraphDB
{
    /// <summary>
    /// The version information of this Graph Database build
    /// </summary>
    public static class Version
    {
        public static UInt16 VersionMajor = 4;
        public static UInt16 VersionMinor = 1;
        public static UInt16 BuildNumber = 1;

        public static String VersionString
        { 
            get
            {
                return "GraphDatabase Revision " + VersionMajor + "." + VersionMinor + " (Build " + BuildNumber + ")";
            }
        }
    }

}
