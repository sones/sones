/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * Warning_Unspecified
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
    public class Warning_Unspecified : IWarning
    {

        #region Properties

        public String     ID         { get; protected set; }
        public String     Message    { get; protected set; }
        public StackTrace StackTrace { get; protected set; }

        #endregion

        #region Constructor(s)

        #region Warning_Unspecified()

        public Warning_Unspecified()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region Warning_Unspecified(myID)

        public Warning_Unspecified(String myID)
        {

            if (myID == null)
                throw new ArgumentNullException("myID");

            Message     = default(String);
            StackTrace  = null;

        }

        #endregion

        #region Warning_Unspecified(myID, myMessage)

        public Warning_Unspecified(String myID, String myMessage)
        {

            if (myID == null)
                throw new ArgumentNullException("myID");

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = null;

        }

        #endregion

        #region Warning_Unspecified(myID, myMessage, myStackTrace)

        public Warning_Unspecified(String myID, String myMessage, StackTrace myStackTrace)
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
