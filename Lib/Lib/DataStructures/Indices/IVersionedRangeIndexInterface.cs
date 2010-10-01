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
 * IVersionedRangeIndexInterface
 * (c) Martin Junghanns, 2009 - 2010
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.Lib.DataStructures.Indices
{
    interface IVersionedRangeIndexInterface<TKey, TValue>
    {
        IEnumerable<TValue> GreaterThen(TKey myKey, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> GreaterThen(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> LowerThen(TKey myKey, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> LowerThen(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Int64 myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);
    }
}
