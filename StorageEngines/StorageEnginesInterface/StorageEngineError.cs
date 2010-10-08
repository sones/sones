/*
 * StorageEngineError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

using sones.Lib.ErrorHandling;
using System.Diagnostics;

#endregion

namespace sones.StorageEngines
{

    /// <summary>
    /// The generic class for all errors within the StorageEngines
    /// </summary>
    public class StorageEngineError : GeneralError
    {

        #region Constructors

        #region StorageEngineError()

        public StorageEngineError()
        {
            Message = default(String);
        }

        #endregion

        #region StorageEngineError(myMessage)

        public StorageEngineError(String myMessage)
        {
            Message = myMessage;
        }

        #endregion

        #region StorageEngineError(myMessage, myStackTrace)

        public StorageEngineError(String myMessage, StackTrace myStackTrace)
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
