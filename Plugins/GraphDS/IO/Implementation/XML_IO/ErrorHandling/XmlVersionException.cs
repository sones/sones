using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling;

namespace sones.Plugins.GraphDS.IOInterface.XML_IO.ErrorHandling
{
    public class XmlVersionException : AGraphDSException
    {
        public XmlVersionException(String myMessage)
        {
            _msg = myMessage;
        }
    }
}
