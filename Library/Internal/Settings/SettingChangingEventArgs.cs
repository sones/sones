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

namespace sones.Library.Settings
{
    public delegate void GraphSettingChangingEvent(SettingChangingEventArgs myGraphDSSetting);

    /// <summary>
    /// The arguments of the event which is fired for a changed setting.
    /// </summary>
    public sealed class SettingChangingEventArgs : EventArgs
    {

        /// <summary>
        /// The Setting information containing the default vaule, type and name
        /// </summary>
        public IGraphSetting Setting { get; private set; }

        /// <summary>
        /// The new setting value
        /// </summary>
        public String SettingValue { get; set; }

        public SettingChangingEventArgs(IGraphSetting mySetting, String mySettingValue)
        {
            Setting = mySetting;
            SettingValue = mySettingValue;
        }

    }
}
