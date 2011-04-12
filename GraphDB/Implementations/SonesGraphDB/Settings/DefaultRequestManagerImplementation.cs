using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Settings;

namespace sones.GraphDB.Settings
{
    /// <summary>
    /// setting that defines the default request manager implementation
    /// </summary>
    public sealed class DefaultRequestManagerImplementation : IGraphSetting
    {
        #region IGraphSetting Members

        public string SettingName
        {
            get { return "DefaultRequestManagerImplementation"; }
        }

        public string DefaultSettingValue
        {
            get { return "RequestManager"; }
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
