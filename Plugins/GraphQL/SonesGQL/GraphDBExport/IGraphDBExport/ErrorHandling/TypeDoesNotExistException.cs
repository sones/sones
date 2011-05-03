using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.DBExport
{
    public class TypeDoesNotExistException : ASonesQLGraphDBExportException
    {
        private String Type;
        private String Info;

        public TypeDoesNotExistException(String myType, String myInfo)
        {
            Type = myType;
            Info = myInfo;

            _msg = String.Format("Error type {0} does not exist.\n\n{1}", Type, Info);
        }
    }
}
