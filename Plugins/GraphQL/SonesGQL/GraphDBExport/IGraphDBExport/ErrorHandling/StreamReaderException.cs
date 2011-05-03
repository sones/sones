using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.DBExport
{
    public class StreamReaderException : ASonesQLGraphDBExportException
    {
        private String Stream;
        private String Info;
        private Exception InnerException;

        public StreamReaderException(String myStream, String myInfo, Exception myInnerException)
        {
            Stream = myStream;
            Info = myInfo;
            InnerException = myInnerException;

            if(InnerException != null)
                _msg = String.Format("Error during initializing new {0} StreamReader.\n\n{1}\n\n{2}", Stream, InnerException.Message, Info);
            else
                _msg = String.Format("Error during initializing new {0} StreamReader.\n\n{1}", Stream, Info);
        }
    }
}
