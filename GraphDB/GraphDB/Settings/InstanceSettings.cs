/* <id name="GraphDB – Settings" />
 * <copyright file="InstanceSettings.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Exceptions;
using sones.Lib.Serializer;
using sones.GraphDB.Licensing;

namespace sones.GraphDB.DataStructures.Settings
{
    public class InstanceSettings
    {

        //[NonSerialized]
        //public static String SettingsObjectName = ".database.instancesettings";

        public String Identifier;   // the Identifier of this Instance aka InstanceName

        public UInt16 VersionMajor;
        public UInt16 VersionMinor;
        public UInt16 BuildNumber;

        public LicensedFeatures Features; // which limits (used Cores, CPU %, RAM, Requests/s, ...) are there for this instance

        #region Defaults

        public InstanceSettings()
        {
            Identifier = Guid.NewGuid().ToString();
            VersionMajor = Version.VersionMajor;
            VersionMinor = Version.VersionMinor;
            BuildNumber = Version.BuildNumber;
            
            Features = new LicensedFeatures();
        }

        #endregion

    }

}
