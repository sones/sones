/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – Settings" />
 * <copyright file="InstanceSettings.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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
