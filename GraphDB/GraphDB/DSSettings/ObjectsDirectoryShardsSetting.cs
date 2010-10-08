using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB;
using sones.Lib.Settings;

namespace sones.GraphFS.Settings
{

    public class ObjectsDirectoryShardsSetting : IGraphSetting
    {

        #region IGraphDSSetting Members

        public String SettingName
        {
            get
            {
                return "ObjectsDirectoryShardsSetting";
            }
        }

        public Type SettingType
        {
            get
            {
                return typeof(UInt16);
            }
        }

        public String DefaultSettingValue
        {
            get { return DBConstants.ObjectDirectoryShards.ToString(); }
        }

        public bool IsValidValue(string myValue)
        {
            UInt16 temp;
            return UInt16.TryParse(myValue, out temp);
        }

        #endregion
    }

}
