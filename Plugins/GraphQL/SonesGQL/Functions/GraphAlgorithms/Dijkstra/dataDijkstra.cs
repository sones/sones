using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions.Dijkstra
{
    #region dataDijkstra
    public class dataDijkstra
    {

        Dictionary<long, Tuple<IVertex, double, ulong, ISingleEdge, IVertex>> list;
        public int Count { get { return Count; } }

        int count;

        public dataDijkstra()
        {
            list = new Dictionary<long, Tuple<IVertex, double, ulong, ISingleEdge, IVertex>>();
            count = 0;

        }
        public void add(IVertex current_node, double current_distance, UInt64 current_depth, ISingleEdge current_edge, IVertex father)
        {
            var id = current_node.VertexID;
            list.Add(id, Tuple.Create(current_node, current_distance, current_depth, current_edge, father));
            count++;
        }


        public void set(IVertex value, double current_distance, ulong current_depth, ISingleEdge current_edge, IVertex father)
        {
            var key = value.VertexID;
            list[key] = Tuple.Create(value, current_distance, current_depth, current_edge, father);

        }

        public ulong getDepth(long current_vertex)
        {

            return list[current_vertex].Item3;
        }

        public ulong getDepth(int current_vertexID)
        {

            return list.ElementAt(current_vertexID).Value.Item3;
        }

        public double getDistance(long current_vertex)
        {

            return list[current_vertex].Item2;
        }

        public double getDistance(int current_vertexID)
        {

            return list.ElementAt(current_vertexID).Value.Item2;
        }

        public Tuple<IVertex, double, ulong, ISingleEdge, IVertex> getElement(long key)
        {
            Tuple<IVertex, double, ulong, ISingleEdge, IVertex> temp;
            list.TryGetValue(key, out temp);
            return temp;
        }

        private Tuple<IVertex, double, ulong, ISingleEdge, IVertex> getTuple(int key)
        {
            return this.list.ElementAt(key).Value;
        }

        private Tuple<IVertex, double, ulong, ISingleEdge, IVertex> getTuple(long key)
        {
            return this.list[key];
        }

        public void Clear()
        {
            list.Clear();
            count = 0;
        }

        private bool ConstainsKey(long key)
        {

            return this.list.ContainsKey(key);
        }


    }
    #endregion
}
