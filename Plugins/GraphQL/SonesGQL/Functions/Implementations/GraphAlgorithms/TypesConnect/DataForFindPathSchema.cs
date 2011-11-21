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
using sones.GraphDB.TypeSystem;

namespace sones.Plugins.SonesGQL.Functions.TypesConnect
{
    #region DataDijkstraForFindPathSchema

    public class DataForFindPathSchema
    {

        private Dictionary<long, Tuple<IVertexType, double, ulong, long, IVertexType>> _list;
        private int _count;

        public int Count { get { return Count; } }


        public DataForFindPathSchema()
        {
            _list = new Dictionary<long, Tuple<IVertexType, double, ulong, long, IVertexType>>();
            _count = 0;

        }
        public void Add(IVertexType current_node, double current_distance, UInt64 current_depth, long current_edge, IVertexType father)
        {
            var id = current_node.ID;
            _list.Add(id, Tuple.Create(current_node, current_distance, current_depth, current_edge, father));
            _count++;
        }


        public void Set(IVertexType value, double current_distance, ulong current_depth, long current_edge, IVertexType father)
        {
            var key = value.ID;
            _list[key] = Tuple.Create(value, current_distance, current_depth, current_edge, father);

        }

        public ulong GetDepth(long current_vertex)
        {

            return _list[current_vertex].Item3;
        }

        public ulong GetDepth(int current_vertexID)
        {
            return _list.ElementAt(current_vertexID).Value.Item3;
        }

        public double GetDistance(long current_vertex)
        {
            return _list[current_vertex].Item2;
        }

        public double GetDistance(int current_vertexID)
        {

            return _list.ElementAt(current_vertexID).Value.Item2;
        }

        public Tuple<IVertexType, double, ulong, long, IVertexType> GetElement(long key)
        {
            Tuple<IVertexType, double, ulong, long, IVertexType> temp;
            _list.TryGetValue(key, out temp);
            return temp;
        }

        private Tuple<IVertexType, double, ulong, long, IVertexType> GetTuple(int key)
        {
            return this._list.ElementAt(key).Value;
        }

        private Tuple<IVertexType, double, ulong, long, IVertexType> GetTuple(long key)
        {
            return this._list[key];
        }

        public void Clear()
        {
            _list.Clear();
            _count = 0;
        }

        private bool ConstainsKey(long key)
        {
            return this._list.ContainsKey(key);
        }
    }
    #endregion
}
