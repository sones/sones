using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphFS.Events
{

    public delegate void ExceptionOccuredEvent(Object mySender, ExceptionOccuredEventArgs myEventArgs);

    public class ExceptionOccuredEventArgs
    {

        private Exception _Exception;

        public Exception Exception
        {
            get
            {
                return _Exception;
            }
        }

        public ExceptionOccuredEventArgs(Exception myException)
        {
            _Exception = myException;
        }

    }

}
