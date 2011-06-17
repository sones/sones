using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Library.SlimLogFramework.Logger;

namespace sones.Library.SlimLogFramework
{
    public sealed class LogManager
    {
        public static LogManager Instance = new LogManager();

        private static ILogger _EmptyLogger = new EmptyLogger();
        private static ILogger _DefaultLogger = _EmptyLogger;

        private LogManager()
        {

        }

        ~LogManager()
        {
            foreach (var logger in _Loggers)
                logger.Value.Dispose();
        }


        private SortedDictionary<String, ILogger> _Loggers = new SortedDictionary<String, ILogger>();

        public ILogger GetLogger()
        {
            return _EmptyLogger;
        }

        public ILogger GetLogger(String myCategory)
        {
            ILogger result;
            if (_Loggers.TryGetValue(myCategory, out result))
                return result;

            return _DefaultLogger;
        }

        public void ConfigureDefaultLogger(ILogger myDefaultLogger)
        {
            _DefaultLogger = myDefaultLogger;
        }

    
        public void ConfigureLogger(String myCategory, ILogger myLogger)
        {
            _Loggers.Remove(myCategory);
            _Loggers.Add(myCategory, myLogger);
        }
}
}
