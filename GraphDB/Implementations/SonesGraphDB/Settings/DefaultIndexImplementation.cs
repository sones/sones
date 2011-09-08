using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Settings;

namespace sones.GraphDB.Settings
{
    public sealed class DefaultIndexImplementation : IGraphSetting
    {
        #region IGraphSetting Members

        public string SettingName
        {
            get { return "DefaultIndexImplementation"; }
        }

        public string DefaultSettingValue
        {
            get { return "sonesindex"; }
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
