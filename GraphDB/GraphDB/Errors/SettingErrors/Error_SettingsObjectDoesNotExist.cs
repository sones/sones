using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_SettingsObjectDoesNotExist : GraphDBSettingError
    {

        public Error_SettingsObjectDoesNotExist()
        {
        }

        public override string ToString()
        {
            return String.Format("The settings object does not exist.");
        }

    }
}
