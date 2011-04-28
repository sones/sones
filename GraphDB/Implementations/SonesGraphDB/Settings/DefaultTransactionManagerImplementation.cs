using System;
using sones.Library.Settings;

namespace sones.GraphDB.Settings
{
    /// <summary>
    /// setting that defines the default transaction manager implementation
    /// </summary>
    public sealed class DefaultTransactionManagerImplementation : IGraphSetting
    {
        #region IGraphSetting Members

        public string SettingName
        {
            get { return "DefaultTransactionManagerImplementation"; }
        }

        public string DefaultSettingValue
        {
            get { return "sones.basictransactionmanager"; }
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
