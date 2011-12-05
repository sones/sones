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
using sones.GraphDB.TypeSystem;

namespace sones.Plugins.SonesGQL.Functions.Dijkstra
{
    #region DataDijkstra

    public class DataDijkstra
    {

        private Dictionary<long, Tuple<IVertex, double, ulong, Tuple<ISingleEdge,IOutgoingEdgeDefinition>, IVertex>> _list;
        private int _count;

        public int Count { get { return Count; } }


        public DataDijkstra()
        {
            _list = new Dictionary<long, Tuple<IVertex, double, ulong, Tuple<ISingleEdge, IOutgoingEdgeDefinition>, IVertex>>();
            _count = 0;

        }

        public void Add(IVertex current_node, double current_distance, UInt64 current_depth, ISingleEdge current_edge,IOutgoingEdgeDefinition edgeType, IVertex father)
        {
            var id = current_node.VertexID;
            _list.Add(id, Tuple.Create(current_node, current_distance, current_depth, Tuple.Create(current_edge,edgeType), father));
            _count++;
        }


        public void Set(IVertex value, double current_distance, ulong current_depth, ISingleEdge current_edge,IOutgoingEdgeDefinition edgeType, IVertex father)
        {
            var key = value.VertexID;
            _list[key] = Tuple.Create(value, current_distance, current_depth, Tuple.Create(current_edge,edgeType), father);

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

        public Tuple<IVertex, double, ulong, Tuple<ISingleEdge,IOutgoingEdgeDefinition>, IVertex> GetElement(long key)
        {
            Tuple<IVertex, double, ulong, Tuple<ISingleEdge,IOutgoingEdgeDefinition>, IVertex> temp;
            _list.TryGetValue(key, out temp);
            return temp;
        }

        private Tuple<IVertex, double, ulong, Tuple<ISingleEdge, IOutgoingEdgeDefinition>, IVertex> GetTuple(int key)
        {
            return this._list.ElementAt(key).Value;
        }

        private Tuple<IVertex, double, ulong, Tuple<ISingleEdge, IOutgoingEdgeDefinition>, IVertex> GetTuple(long key)
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
