/*
 * UnspecifiedWarning
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Diagnostics;
using System.Text;

#endregion

namespace sones.Lib.ErrorHandling
{

    /// <summary>
    /// This class carries information of unspecified warnings.
    /// </summary>
    public class UnspecifiedWarning : IWarning
    {

        #region Properties

        public String     ID         { get; protected set; }
        public String     Message    { get; protected set; }
        public StackTrace StackTrace { get; protected set; }

        #endregion

        #region Constructor(s)

        #region UnspecifiedWarning()

        public UnspecifiedWarning()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region UnspecifiedWarning(myID)

        public UnspecifiedWarning(String myID)
        {

            if (myID == null)
                throw new ArgumentNullException("myID");

            Message     = default(String);
            StackTrace  = null;

        }

        #endregion

        #region UnspecifiedWarning(myID, myMessage)

        public UnspecifiedWarning(String myID, String myMessage)
        {

            if (myID == null)
                throw new ArgumentNullException("myID");

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = null;

        }

        #endregion

        #region UnspecifiedWarning(myID, myMessage, myStackTrace)

        public UnspecifiedWarning(String myID, String myMessage, StackTrace myStackTrace)
        {

            if (myID == null)
                throw new ArgumentNullException("myID");

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = myStackTrace;

        }

        #endregion

        #endregion


        #region ToString()

        public override String ToString()
        {

            var _StringBuilder = new StringBuilder();

            _StringBuilder.AppendLine(String.Format("ID: ",      ID));
            _StringBuilder.AppendLine(String.Format("Message: ", Message));

            if (StackTrace != null)
                _StringBuilder.AppendLine(String.Format("Stacktrace: ", StackTrace.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion

    }

}
