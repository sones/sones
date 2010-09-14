/*
 * GraphDBError
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Diagnostics;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Warnings
{

    /// <summary>
    /// The generic class for all errors within the GraphDB
    /// </summary>

    public class GraphDBWarning : GeneralWarning
    {

        #region Constructor(s)

        #region GraphDBWarning()

        public GraphDBWarning()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region GraphDBWarning(myMessage)

        public GraphDBWarning(String myMessage)
        {

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = null;

        }

        #endregion

        #region GraphDBWarning(myMessage, myStackTrace)

        public GraphDBWarning(String myMessage, StackTrace myStackTrace)
        {

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = myStackTrace;

        }

        #endregion

        #endregion

    }

}
