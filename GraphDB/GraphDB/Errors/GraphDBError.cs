/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/*
 * GraphDBError
 * Achim Friedland, 2010
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
