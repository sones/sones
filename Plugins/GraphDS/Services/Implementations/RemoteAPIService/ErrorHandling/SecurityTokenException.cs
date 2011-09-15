using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDS.Services.RemoteAPIService.ErrorHandling
{
    public class SecurityTokenException : AGraphDSException
    {
         /// <summary>
        /// The constructor for the exception.
        /// </summary>
        /// <param name="myMessage">The exception message.</param>
        public SecurityTokenException(String myMessage, Exception myInnerException = null)
        {
            _msg = myMessage;
        }
    }
}
