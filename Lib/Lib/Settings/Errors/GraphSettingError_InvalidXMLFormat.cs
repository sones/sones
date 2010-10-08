using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Settings.Errors
{

    /// <summary>
    /// The XML of the setting is not well formed.
    /// </summary>
    public class GraphSettingError_InvalidXMLFormat : GraphSettingError
    {
        private   string _Info;

        public GraphSettingError_InvalidXMLFormat()
        {
        }

        public GraphSettingError_InvalidXMLFormat(string myInfo)
        {
            _Info = myInfo;
        }
    }
}
