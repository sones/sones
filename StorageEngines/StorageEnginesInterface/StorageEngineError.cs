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
    public class StorageEngineError : IError
    {

        #region Properties

        public String       Message     { get; protected set; }
        public StackTrace   StackTrace  { get; protected set; }

        #endregion

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
