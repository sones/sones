using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using System.Diagnostics;

namespace sones.Lib.Settings.Errors
{
    
    /// <summary>
    /// The generic class for all errors within the GraphAppSettings
    /// </summary>
    public class GraphSettingError : GeneralError
    {

        #region Constructor(s)

        #region GraphFSError()

        public GraphSettingError()
        {
            Message = default(String);
            StackTrace = null;
        }

        #endregion

        #region GraphSettingError(myMessage)

        public GraphSettingError(String myMessage)
        {
            Message = myMessage;
            StackTrace = null;
        }

        #endregion

        #region GraphSettingError(myMessage, myStackTrace)

        public GraphSettingError(String myMessage, StackTrace myStackTrace)
        {
            Message = myMessage;
            StackTrace = myStackTrace;
        }

        #endregion

        #endregion

    }

}
