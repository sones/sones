using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_SettingIsInactive : GraphDBSettingError
    {
        public String Setting { get; private set; }

        public Error_SettingIsInactive(String mySetting)
        {
            Setting = mySetting;
        }

        public override string ToString()
        {
            return String.Format("The setting {0} is inactive", Setting);
        }
    }
}
