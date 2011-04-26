using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDSServer.ErrorHandling
{
    /// <summary>
    /// This exception will be thrown if the REST service could not be startet.
    /// </summary>
    public class RESTServiceCouldNotStartetException : AGraphDSException
    {
        /// <summary>
        /// The constructor for the exception.
        /// </summary>
        /// <param name="myMessage">The exception message.</param>
        public RESTServiceCouldNotStartetException(String myMessage)
        {
            _msg = myMessage;
        }
    }
}
