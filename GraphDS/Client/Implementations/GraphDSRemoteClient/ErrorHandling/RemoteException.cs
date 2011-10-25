using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.ErrorHandling;

namespace sones.GraphDS.GraphDSRemoteClient.ErrorHandling
{
    public class RemoteException : ASonesException
    {
        private String _msg;

        public RemoteException(String myMessage)
        {
            _msg = myMessage;
        }

        public override string Message
        {
            get { return _msg; }
        }
    }
}
