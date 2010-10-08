using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphFS.Session
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
