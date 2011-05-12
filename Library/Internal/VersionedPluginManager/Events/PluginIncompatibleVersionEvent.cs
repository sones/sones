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
using System.Reflection;

namespace sones.Library.VersionedPluginManager
{

    #region PluginIncompatibleVersionEvent

    public delegate void PluginIncompatibleVersionEvent(
        PluginManager myPluginManager, PluginIncompatibleVersionEventArgs myPluginIncompatibleVersionEventArgs);

    public class PluginIncompatibleVersionEventArgs : EventArgs
    {
        #region Properties

        public Version PluginVersion { get; private set; }
        public Version MinVersion { get; private set; }
        public Version MaxVersion { get; private set; }
        public Type PluginType { get; private set; }
        public Assembly PluginAssembly { get; private set; }

        #endregion

        public PluginIncompatibleVersionEventArgs(Assembly myPluginAssembly, Version myPluginVersion,
                                                  Version myMinVersion, Version myMaxVersion, Type myPluginType)
        {
            PluginVersion = myPluginVersion;
            MinVersion = myMinVersion;
            MaxVersion = myMaxVersion;
            PluginType = myPluginType;
            PluginAssembly = myPluginAssembly;
        }
    }

    #endregion
}