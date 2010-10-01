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
 * CacheTimer<T>
 * (c) Achim Friedland, 2010
 */

#region Using

using System;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.Caches
{

    public struct CacheTimer<T>
    {

        #region Data

        private readonly T _Object;
        private UInt64 _Timestamp;

        #endregion

        #region Constructor(s)

        public CacheTimer(T myObject)
        {
            _Object = myObject;
            _Timestamp = TimestampNonce.Ticks;
        }

        #endregion

        #region Object

        public T Object
        {
            get
            {
                _Timestamp = TimestampNonce.Ticks;
                return _Object;
            }
        }

        #endregion

        #region Timestamp

        public UInt64 Timestamp
        {
            get
            {
                return _Timestamp;
            }
        }

        #endregion

    }

}
