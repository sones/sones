// Copyright (C) 2009 Robert Rossney <rrossney@gmail.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;

namespace LRUCache
{

    internal class LRUCacheEnumerator<T> : IEnumerator<T>
    {

        private readonly LRUCache<T> Cache;
        private LinkedListNode<T> CurrentNode;

        internal LRUCacheEnumerator(LRUCache<T> cache)
        {
            Cache = cache;
            CurrentNode = cache._LRUList.First;
        }

        #region IEnumerator<T> Members

        T IEnumerator<T>.Current
        {
            get { return CurrentNode.Value; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return CurrentNode.Value; }
        }

        bool IEnumerator.MoveNext()
        {
            CurrentNode = CurrentNode.Next;
            return (CurrentNode != null);
        }

        void IEnumerator.Reset()
        {
            CurrentNode = Cache._LRUList.First;
        }

        #endregion

    }

}
