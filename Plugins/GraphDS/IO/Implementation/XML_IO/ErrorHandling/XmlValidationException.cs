using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling;

namespace sones.Plugins.GraphDS.IO.XML_IO.ErrorHandling
{
    public class XmlValidationException : AGraphDSException
    {
        public XmlValidationException(String myMessage)
        {
            _msg = myMessage;
        }
    }
}
