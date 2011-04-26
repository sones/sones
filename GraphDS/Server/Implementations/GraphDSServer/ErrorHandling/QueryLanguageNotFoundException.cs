using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDSServer.ErrorHandling
{
    /// <summary>
    /// This exception will be thrown if the GraphDS server does not support a query language.
    /// </summary>
    public class QueryLanguageNotFoundException : AGraphDSException
    {
        /// <summary>
        /// The constructor for the exception.
        /// </summary>
        /// <param name="myMessage">The exception message.</param>
        public QueryLanguageNotFoundException(String myMessage)
        {
            _msg = myMessage;
        }
    }
}
