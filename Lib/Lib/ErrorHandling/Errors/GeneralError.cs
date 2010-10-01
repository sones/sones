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
 * GeneralError
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
    /// This class carries information of errors.
    /// </summary>
    public class GeneralError : IError
    {

        #region Properties

        public String     Message    { get; protected set; }
        public StackTrace StackTrace { get; protected set; }

        #endregion

        #region Constructor(s)

        #region GeneralError()

        public GeneralError()
        {
            Message     = default(String);
            StackTrace  = null;
        }

        #endregion

        #region GeneralError(myMessage)

        public GeneralError(String myMessage)
        {

            if (myMessage == null)
                throw new ArgumentNullException("myMessage");

            Message     = myMessage;
            StackTrace  = null;

        }

        #endregion

        #region GeneralError(myMessage, myStackTrace)

        public GeneralError(String myMessage, StackTrace myStackTrace)
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

            _StringBuilder.AppendLine(String.Format("Message: {0}", Message));

            if (StackTrace != null)
                _StringBuilder.AppendLine(String.Format("Stacktrace: {0}", StackTrace.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion

    }

}
