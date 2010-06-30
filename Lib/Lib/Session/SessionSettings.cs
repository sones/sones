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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Settings;

namespace sones.Lib.Session
{
    public class SessionSettings
    {

        #region Data

        protected Dictionary<String, ISettings> _SessionSettings = new Dictionary<string, ISettings>();

        #endregion
        
        #region Constructor

        public SessionSettings()
        {
        }

        public SessionSettings(Dictionary<String, ISettings> settings)
        {
            _SessionSettings = settings;
        }

        #endregion

        #region Settings

        /// <summary>
        /// Add a copy of the setting with the name <paramref name="myName"/>
        /// </summary>
        /// <param name="myName"></param>
        /// <param name="mySetting"></param>
        public void AddSessionSetting(string myName, ISettings mySetting)
        {
            if (!_SessionSettings.ContainsKey(myName))
                _SessionSettings.Add(myName, mySetting.Clone());
            else
                ChangeSessionSetting(myName, mySetting);
        }

        /// <summary>
        /// Removes the setting
        /// </summary>
        /// <param name="myName"></param>
        public void RemoveSessionSetting(string myName)
        {
            _SessionSettings.Remove(myName);
        }

        public void ChangeSessionSetting(string myName, ISettings mySetting)
        {
            if (_SessionSettings.ContainsKey(myName))
                _SessionSettings[myName] = (ISettings)mySetting.Clone();
            else
                AddSessionSetting(myName, mySetting);
        }

        /// <summary>
        /// Create a copy of all settings
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ISettings> GetAllSettings()
        {
            Dictionary<string, ISettings> RetVal = new Dictionary<string, ISettings>();
            foreach (var Setting in _SessionSettings)
                RetVal.Add(Setting.Key, Setting.Value.Clone());

            return RetVal;
        }

        /// <summary>
        /// Returns a copy of the setting value
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        public ISettings GetSettingValue(string myName)
        {
            Dictionary<string, ISettings> RetVal = GetAllSettings();

            if (RetVal.ContainsKey(myName))
                return RetVal[myName].Clone();

            return null;
        }

        #endregion 
    }
}
