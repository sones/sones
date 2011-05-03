using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.DBExport
{
    public sealed class ExportFailedException : ASonesQLGraphDBExportException
    {
        private String DumpType;
        private String Info;

        public ExportFailedException(String myDumpType, String myInfo)
        {
            DumpType = myDumpType;
            Info = myInfo;

            _msg = String.Format("Export failed, the Export{0} of GQLGrammar returned null.\n\n{1}", DumpType, Info);
        }
    }
}
