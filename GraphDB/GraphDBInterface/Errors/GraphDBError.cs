/*
 * GraphDBError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Diagnostics;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Errors
{

    /// <summary>
    /// The generic class for all errors within the GraphDB
    /// </summary>

    public class GraphDBError : IError
    {

        #region Properties

        public String       Message         { get; protected set; }
        public StackTrace   StackTrace      { get; protected set; }

        #endregion

        #region Constructors

        #region GraphDBError()

        public GraphDBError()
        {
            Message = default(String);
#if DEBUG
            StackTrace = new StackTrace(true);
#endif
        }

        #endregion

        #region GraphDBError(myMessage)

        public GraphDBError(String myMessage)
        {
            Message = myMessage;
#if DEBUG
            StackTrace = new StackTrace(true);
#endif
        }

        #endregion

        #region GraphDBError(myMessage, myStackTrace)

        public GraphDBError(String myMessage, StackTrace myStackTrace)
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
