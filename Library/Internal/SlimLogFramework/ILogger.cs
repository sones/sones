using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.SlimLogFramework
{
    public interface ILogger: IDisposable
    {
        void Log(String myMessage);
        void Log(String myMessage, params Object[] myParams);
        void Log(Level myLevel, String myMessage);
        void Log(Level myLevel, String myMessage, params Object[] myParams);

    }
}
