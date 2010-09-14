
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Win32;

namespace sones.Libraries.MIMEType
{

    public static class MIMEType
    {

        public static String GetMimeType(String myFileName)
        {

            var _MIMEType       = "application/unknown";
            var _FileExtension  = Path.GetExtension(myFileName).ToLower();
            var _RegKey         = Registry.ClassesRoot.OpenSubKey(_FileExtension);

            if (_RegKey != null && _RegKey.GetValue("Content Type") != null)
                _MIMEType = _RegKey.GetValue("Content Type").ToString();

            return _MIMEType;

        }

    }

}
