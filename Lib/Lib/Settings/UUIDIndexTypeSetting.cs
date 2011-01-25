using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Settings;

namespace sones.Lib.Settings
{
    public class UUIDIndexTypeSetting : IGraphSetting
    {

        #region IGraphDSSetting Members

        public String SettingName
        {
            get
            {
                return "UUIDIndexTypeSetting";
            }
        }

        public Type SettingType
        {
            get
            {
                return typeof(String);
            }
        }

        public String DefaultSettingValue
        {
            get { return "UUID"; }
        }

        public bool IsValidValue(string myValue)
        {
            return true;
        }

        #endregion
    }
}
