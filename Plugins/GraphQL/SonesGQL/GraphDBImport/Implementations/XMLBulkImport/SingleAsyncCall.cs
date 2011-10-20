/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace sones.Plugins.SonesGQL.XMLBulkImport
{
    /// <summary>
    /// An asyncronous process.
    /// </summary>
    internal class SingleAsyncCall: IDisposable
    {
        #region Data

        /// <summary>
        /// Stores the current task.
        /// </summary>
        private Task _currentTask;

        private CancellationTokenSource _cancel = new CancellationTokenSource();

        #endregion
    
        #region IDisposable Members

        public void Dispose()
        {
            _cancel.Cancel();

            if (_currentTask != null)
                Task.WaitAll(_currentTask);

            _cancel.Dispose();
        }

        #endregion
    }
}

