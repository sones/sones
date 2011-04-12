using System;
using sones.Library.Settings;

namespace sones.GraphDB.Settings
{
    /// <summary>
    /// setting that defines the default request scheduler implementation
    /// </summary>
    public sealed class DefaultRequestSchedulerImplementation : IGraphSetting
    {
        #region IGraphSetting Members

        public string SettingName
        {
            get { return "DefaultRequestSchedulerImplementation"; }
        }

        public string DefaultSettingValue
        {
            get { return "BasicRequestScheduler"; }
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
