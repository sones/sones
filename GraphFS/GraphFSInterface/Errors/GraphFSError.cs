/*
 * GraphFSError
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
    /// The generic class for all errors within the GraphFS
    /// </summary>
    public class GraphFSError : GeneralError
    {

        #region Constructor(s)

        #region GraphFSError()

        public GraphFSError()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region GraphFSError(myMessage)

        public GraphFSError(String myMessage)
        {
            Message     = myMessage;
            StackTrace  = null;
        }

        #endregion

        #region GraphFSError(myMessage, myStackTrace)

        public GraphFSError(String myMessage, StackTrace myStackTrace)
        {
            Message     = myMessage;
            StackTrace  = myStackTrace;
        }

        #endregion

        #endregion

    }

}
