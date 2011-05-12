/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Runtime.InteropServices;

namespace sones.Library.VersionedPluginManager
{
    /// <summary>
    /// Defines the min and max version of the plugin.
    /// This must be added to the assembly which activates the plugins.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    [ComVisible(true)]
    public sealed class AssemblyVersionCompatibilityAttribute : Attribute
    {
        public AssemblyVersionCompatibilityAttribute(String myPluginName, String myMinVersion, String myMaxVersion)
        {
            PluginName = myPluginName;
            MinVersion = new Version(myMinVersion);
            if (!String.IsNullOrEmpty(myMaxVersion))
            {
                MaxVersion = new Version(myMaxVersion);
            }
        }


        public Version MinVersion { get; private set; }
        public Version MaxVersion { get; private set; }
        public String PluginName { get; private set; }
    }
}