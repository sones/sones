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
