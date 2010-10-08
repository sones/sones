using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Settings.Errors
{

    /// <summary>
    /// Could not load the setting file due to a FileNotFound or mailformed XML
    /// </summary>
    public class GraphSettingError_CouldNotLoadSettingsFile : GraphSettingError
    {
        private   string _Info;

        public GraphSettingError_CouldNotLoadSettingsFile()
        {
        }

        public GraphSettingError_CouldNotLoadSettingsFile(string myInfo)
        {
            _Info = myInfo;
        }
    }
}
