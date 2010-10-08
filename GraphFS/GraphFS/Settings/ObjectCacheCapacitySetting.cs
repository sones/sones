using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Settings;

namespace sones.GraphFS.Settings
{

    public class ObjectCacheCapacitySetting : IGraphSetting
    {

        #region IGraphDSSetting Members

        public String SettingName
        {
            get
            {
                return "ObjectCacheCapacitySetting";
            }
        }

        public Type SettingType
        {
            get
            {
                return typeof(UInt64);
            }
        }

        public String DefaultSettingValue
        {
            get { return "500000"; }
        }

        public bool IsValidValue(string myValue)
        {
            UInt64 temp;
            return UInt64.TryParse(myValue, out temp);
        }

        #endregion
    }

}
