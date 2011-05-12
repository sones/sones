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

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
  internal class ThreadSafeStore<TKey, TValue>
  {
    private readonly object _lock = new object();
    private Dictionary<TKey, TValue> _store;
    private readonly Func<TKey, TValue> _creator;

    public ThreadSafeStore(Func<TKey, TValue> creator)
    {
      if (creator == null)
        throw new ArgumentNullException("creator");

      _creator = creator;
    }

    public TValue Get(TKey key)
    {
      if (_store == null)
        return AddValue(key);

      TValue value;
      if (!_store.TryGetValue(key, out value))
        return AddValue(key);

      return value;
    }

    private TValue AddValue(TKey key)
    {
      TValue value = _creator(key);

      lock (_lock)
      {
        if (_store == null)
        {
          _store = new Dictionary<TKey, TValue>();
          _store[key] = value;
        }
        else
        {
          // double check locking
          TValue checkValue;
          if (_store.TryGetValue(key, out checkValue))
            return checkValue;

          Dictionary<TKey, TValue> newStore = new Dictionary<TKey, TValue>(_store);
          newStore[key] = value;

          _store = newStore;
        }

        return value;
      }
    }
  }
}