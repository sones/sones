/*
 * AutoDiscoveryError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Diagnostics;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.Lib.Reflection
{

    /// <summary>
    /// The generic class for all errors within the AutoDiscovery&lt;T&gt; class
    /// </summary>

    public class AutoDiscoveryError : GeneralError
    {
        #region Constructors

        #region GraphFSError()

        public AutoDiscoveryError()
        {
            Message = default(String);
        }

        #endregion

        #region AutoDiscoveryError(myMessage)

        public AutoDiscoveryError(String myMessage)
        {
            Message = myMessage;
        }

        #endregion

        #region AutoDiscoveryError(myMessage, myStackTrace)

        public AutoDiscoveryError(String myMessage, StackTrace myStackTrace)
        {
            Message     = myMessage;
            StackTrace  = myStackTrace;
        }

        #endregion

        #endregion

        #region ToString()

        public override String ToString()
        {
            return Message;
        }

        #endregion

    }

}
