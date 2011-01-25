using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Settings;

namespace sones.Lib.Settings
{
    public class AttributeIndexTypeSetting : IGraphSetting
    {

        #region IGraphDSSetting Members

        public String SettingName
        {
            get
            {
                return "AttributeIndexTypeSetting";
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
            get { return "HashTable"; }
        }

        public bool IsValidValue(string myValue)
        {
            return true;
        }

        #endregion
    }
}
