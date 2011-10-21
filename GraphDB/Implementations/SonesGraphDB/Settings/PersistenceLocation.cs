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
using sones.Library.Settings;

// HACK: this is to be replaced by dedicated settings !!! (btk,23.09.2011)
namespace sones.GraphDB.Settings
{
    /// <summary>
    /// setting that defines the persistence location of GraphDB
    /// </summary>
    public sealed class PersistenceLocation : IGraphSetting
    {
        #region IGraphSetting Members

        public string SettingName
        {
            get { return "PersistenceLocation"; }
        }

        public string DefaultSettingValue
        {
            get { return ""; }
        }

        public Type SettingType
        {
            get { return typeof(String); }
        }

        public bool IsValidValue(string myValue)
        {
            return true;
        }
        #endregion
    }
}
