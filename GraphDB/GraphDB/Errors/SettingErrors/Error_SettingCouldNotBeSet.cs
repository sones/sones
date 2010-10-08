using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_SettingCouldNotBeSet : GraphDBSettingError
    {
        public String Setting { get; private set; }

        public Error_SettingCouldNotBeSet(String mySetting)
        {
            Setting = mySetting;
        }

        public override string ToString()
        {
            return String.Format("The setting {0} could not be set!", Setting);
        }
    }
}
