using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Pandora.Lib.Settings;

namespace sones.GraphFS.Session
{
    public class SessionInfos
    {

        private Dictionary<String, ISettings> _SessionSettings;

        public Dictionary<String, ISettings> SessionSettings
        {
            get
            {
                return _SessionSettings;
            }
            set
            {
                _SessionSettings = value;
            }
        }



        #region Constructor

        #region IronySessionToken()

        public SessionInfos()
        {
            _SessionSettings = new Dictionary<String, ISettings>();
        }

        #endregion

        #endregion


        #region Settings

        public void AddSessionSetting(string myName, ISettings mySetting)
        {
            if (!_SessionSettings.ContainsKey(myName))
                _SessionSettings.Add(myName, mySetting.Clone());
            else
                ChangeSessionSetting(myName, mySetting);
        }

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

        public Dictionary<string, ISettings> GetAllSettings()
        {
            Dictionary<string, ISettings> RetVal = new Dictionary<string, ISettings>();
            foreach (var Setting in _SessionSettings)
                RetVal.Add(Setting.Key, Setting.Value.Clone());

            return RetVal;
        }

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
