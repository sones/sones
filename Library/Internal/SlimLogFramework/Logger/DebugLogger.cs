using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sones.Library.SlimLogFramework.Logger
{
    public class DebugLogger: ILogger
    {
        private Level _Level;

        public DebugLogger() : this(Level.SEVERE) { }

        public DebugLogger(Level myLevel)
        {
            this._Level = myLevel;
        }

        void ILogger.Log(string myMessage)
        {
            DoLog(Level.SEVERE, myMessage, null);
        }

        void ILogger.Log(string myMessage, params object[] myParams)
        {
            DoLog(Level.SEVERE, myMessage, myParams);
        }


        void ILogger.Log(Level myLevel, string myMessage)
        {
            DoLog(myLevel, myMessage, null);
        }

        void ILogger.Log(Level myLevel, string myMessage, params object[] myParams)
        {
            DoLog(myLevel, myMessage, myParams);
        }

        private void DoLog(Level myLevel, string myMessage, object[] myParams)
        {
            if (myLevel > _Level)
                return;

            var message = (myParams == null)
                ? myMessage
                : String.Format(myMessage, myParams);

            Debug.WriteLine(myMessage);
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
