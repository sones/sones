using System;
using sones.Library.ErrorHandling;
using System.Diagnostics;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The desire functionality is not implemented
    /// </summary>
    public class NotImplementedQLException : AGraphQLException
    {
        public String Message { get; protected set; }
                                
        /// <summary>
        /// Creates a new NotImplementedException exception
        /// </summary>
        /// <param name="myMessage">The given message</param>
        public NotImplementedQLException(String myMessage)
        {            
            Message = myMessage;
            _msg = String.Format("Stacktrace" + Environment.NewLine + "{0}" + Environment.NewLine + Environment.NewLine + Message, StackTrace);
        }

    }

}
