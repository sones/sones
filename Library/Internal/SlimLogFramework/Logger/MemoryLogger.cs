using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.SlimLogFramework.Logger
{
    public class MemoryLogger: ILogger
    {
        #region Data

        private int _MaxSize;
        private int _Size;
        private Level _Level;
        private LinkedList<String> _Entries = new LinkedList<String>();

        #endregion

        #region Consts

        private const int _DefaultSize = 4 * 1024 * 1024;
        
        #endregion

        #region c'tors

        public MemoryLogger() : this(_DefaultSize, Level.SEVERE) { }

        public MemoryLogger(int myMaxSize) : this(myMaxSize, Level.SEVERE) { }

        public MemoryLogger(Level level) : this(_DefaultSize, level) { }

        public MemoryLogger(int myMaxSize, Level level)
        {
            // TODO: Complete member initialization
            this._MaxSize = myMaxSize;
            this._Level = level;
        }

        #endregion

        #region Public methods

        public IEnumerable<String> GetLogEntries()
        {
            lock (_Entries)
            {
                //return a copy
                return _Entries.ToArray();
            }
        }

        #endregion

        #region ILogger 

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

        #endregion

        #region private methods

        private void DoLog(Level myLevel, string myMessage, params object[] myParams)
        {
            if (myLevel > _Level)
                return;

            var log = (myParams == null)
                ? String.Format(myMessage, myParams)
                : myMessage;

            var logSize = myMessage.Length;

            lock (_Entries)
            {
                while (_Size + logSize > _MaxSize)
                {
                    _Size -= _Entries.First.Value.Length;
                    _Entries.RemoveFirst();
                }

                _Size += logSize;
                _Entries.AddLast(log);
            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            _Entries.Clear();
        }

        #endregion
    }
}
