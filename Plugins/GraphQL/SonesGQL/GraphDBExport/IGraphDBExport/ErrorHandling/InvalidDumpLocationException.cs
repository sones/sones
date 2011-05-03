using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.DBExport
{
    public class InvalidDumpLocationException : ASonesQLGraphDBExportException
    {
        private String Location;
        private String Info;
        private String ExpectedFile;
        private String ExpectedHttp;

        public InvalidDumpLocationException(String myLocation, String myExpectedFile, String myExpectedHttp, String myInfo)
        {
            Location = myLocation;
            Info = myInfo;
            ExpectedFile = myExpectedFile;
            ExpectedHttp = myExpectedHttp;

            _msg = String.Format("Invalid dump destination: {0}. Expected {1} or {2}\n\n{3}", Location, myExpectedFile, myExpectedHttp, Info);
        }

    }
}
