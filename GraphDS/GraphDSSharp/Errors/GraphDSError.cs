/*
 * GraphDSError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Diagnostics;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS.Errors
{

    /// <summary>
    /// The generic class for all errors within the GraphDS
    /// </summary>
    public class GraphDSError : GeneralError
    {

        #region Constructor(s)

        #region GraphDSError()

        public GraphDSError()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region GraphDSError(myMessage)

        public GraphDSError(String myMessage)
        {
            Message     = myMessage;
            StackTrace  = null;
        }

        #endregion

        #region GraphDSError(myMessage, myStackTrace)

        public GraphDSError(String myMessage, StackTrace myStackTrace)
        {
            Message     = myMessage;
            StackTrace  = myStackTrace;
        }

        #endregion

        #endregion

    }

}
