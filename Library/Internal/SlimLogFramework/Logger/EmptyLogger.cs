using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.SlimLogFramework.Logger
{
    public class EmptyLogger: ILogger
    {
        public EmptyLogger() { }

        void ILogger.Log(string myMessage)
        {
        }

        void ILogger.Log(string myMessage, params object[] myParams)
        {
        }

        void ILogger.Log(Level myLevel, string myMessage)
        {
        }

        void ILogger.Log(Level myLevel, string myMessage, params object[] myParams)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
