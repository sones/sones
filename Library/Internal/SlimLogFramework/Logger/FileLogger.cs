using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sones.Library.SlimLogFramework.Logger
{
    public class FileLogger: ILogger
    {
        private Level _Level;
        private StreamWriter _stream;
        private bool _disposed = false;


        public FileLogger(String myPath) : this(myPath, Level.SEVERE) { }

        public FileLogger(String myPath, Level myLevel)
        {
            this._Level = myLevel;
            _stream = File.CreateText(myPath);
            _stream.AutoFlush = true;
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

            myMessage = (myParams == null)
                ? myMessage
                : String.Format(myMessage, myParams);

            
            _stream.WriteLine(DateTime.Now + " " + myLevel.ToString() + ": " + myMessage);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_stream != null && ! _disposed)
            {
                _disposed = true;
                try
                {
                    _stream.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    //nothing to do
                }
            }
        }

        #endregion
    }
}
