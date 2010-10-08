/*
 * GeneralWarning
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
    /// This class carries information of warning.
    /// </summary>
    public class GeneralWarning : IWarning
    {

        #region Properties

        public String     Message    { get; protected set; }
        public StackTrace StackTrace { get; protected set; }

        #endregion

        #region Constructor(s)

        #region GeneralWarning()

        public GeneralWarning()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region GeneralWarning(myMessage)

        public GeneralWarning(String myMessage)
        {

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = null;

        }

        #endregion

        #region GeneralWarning(myMessage, myStackTrace)

        public GeneralWarning(String myMessage, StackTrace myStackTrace)
        {

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

            _StringBuilder.AppendLine(String.Format("Message: ", Message));

            if (StackTrace != null)
                _StringBuilder.AppendLine(String.Format("Stacktrace: ", StackTrace.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion

    }

}
