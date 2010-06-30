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
 * IVersionedDictionaryObject
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Text;
using sones.Lib.DataStructures;
using System.Collections.Generic;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.Dictionaries;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface of a VersionedDictionaryObject to store a mapping TKey => DictionaryValueHistory&lt;TValue&gt;.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>
    public interface IVersionedDictionaryObject<TKey, TValue> : IDictionaryObject<TKey, TValue>, IVersionedDictionaryInterface<TKey, TValue>
        where TKey : IComparable
    {
    }

}