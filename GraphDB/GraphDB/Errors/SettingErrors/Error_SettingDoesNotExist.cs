using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_SettingDoesNotExist : GraphDBSettingError
    {
        public String SettingName { get; private set; }

        public Error_SettingDoesNotExist(String mySettingName)
        {
            SettingName = mySettingName;
        }

        public override string ToString()
        {
            return String.Format("The setting {0} does not exist.", SettingName);
        }
    }
}
