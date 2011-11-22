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
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions.Dijkstra
{
    #region buffer

    public class BufferDijkstra
    {

        private SortedDictionary<Tuple<double, long>, Tuple<IVertex, double, ulong>> _buffer;
        private int _count;

        public int Count { get { return _count; } }
        

        public BufferDijkstra()
        {
            _buffer = new SortedDictionary<Tuple<double, long>, Tuple<IVertex, double, ulong>>();
            _count = 0;
        }

        public void Add(IVertex current_node, double current_distance, UInt64 current_depth)
        {
            var id = current_node.VertexID;
            _buffer.Add(Tuple.Create(current_distance, id), Tuple.Create(current_node, current_distance, current_depth));

            _count++;

        }

        public Tuple<IVertex, double, ulong> Min()
        {
            return _buffer.ElementAt(0).Value;
        }


        public void Remove(double key_primary, long key_secondary)
        {
            _buffer.Remove(Tuple.Create(key_primary, key_secondary));
            _count--;
        }

        public void Set(double key_primary, IVertex value, double current_distance, ulong current_depth)
        {
            var key = value.VertexID;
            _buffer.Remove(Tuple.Create(key_primary, key));
            _buffer.Add(Tuple.Create(current_distance, key), Tuple.Create(value, current_distance, current_depth));
        }

        public ulong GetDepth(double key_primary, long current_vertex)
        {
            return _buffer[Tuple.Create(key_primary, current_vertex)].Item3;
        }

        public ulong GetDepth(int current_vertexID)
        {
            return _buffer.ElementAt(current_vertexID).Value.Item3;
        }

        public double GetDistance(double key_primary, long current_vertex)
        {
            return _buffer[Tuple.Create(key_primary, current_vertex)].Item2;
        }

        public double GetDistance(int current_vertexID)
        {
            return _buffer.ElementAt(current_vertexID).Value.Item2;
        }

        public Tuple<IVertex, double, ulong> GetElement(double key_primary, long index)
        {
            Tuple<IVertex, double, ulong> output;
            _buffer.TryGetValue(Tuple.Create(key_primary, index), out output);
            return output;
        }

        public void Clear()
        {
            _buffer.Clear();
            _count = 0;
        }

    }
    #endregion
}
