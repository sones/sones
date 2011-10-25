using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

namespace sones.Plugins.SonesGQL.Functions.Dijkstra
{
    #region dataDijkstra
    public class dataDijkstra
    {

        Dictionary<long, Tuple<IVertex, double, ulong, Tuple<ISingleEdge,IOutgoingEdgeDefinition>, IVertex>> list;
        public int Count { get { return Count; } }

        int count;

        public dataDijkstra()
        {
            list = new Dictionary<long, Tuple<IVertex, double, ulong, Tuple<ISingleEdge, IOutgoingEdgeDefinition>, IVertex>>();
            count = 0;

        }
        public void add(IVertex current_node, double current_distance, UInt64 current_depth, ISingleEdge current_edge,IOutgoingEdgeDefinition edgeType, IVertex father)
        {
            var id = current_node.VertexID;
            list.Add(id, Tuple.Create(current_node, current_distance, current_depth, Tuple.Create(current_edge,edgeType), father));
            count++;
        }


        public void set(IVertex value, double current_distance, ulong current_depth, ISingleEdge current_edge,IOutgoingEdgeDefinition edgeType, IVertex father)
        {
            var key = value.VertexID;
            list[key] = Tuple.Create(value, current_distance, current_depth, Tuple.Create(current_edge,edgeType), father);

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

        public Tuple<IVertex, double, ulong, Tuple<ISingleEdge,IOutgoingEdgeDefinition>, IVertex> getElement(long key)
        {
            Tuple<IVertex, double, ulong, Tuple<ISingleEdge,IOutgoingEdgeDefinition>, IVertex> temp;
            list.TryGetValue(key, out temp);
            return temp;
        }

        private Tuple<IVertex, double, ulong, Tuple<ISingleEdge, IOutgoingEdgeDefinition>, IVertex> getTuple(int key)
        {
            return this.list.ElementAt(key).Value;
        }

        private Tuple<IVertex, double, ulong, Tuple<ISingleEdge, IOutgoingEdgeDefinition>, IVertex> getTuple(long key)
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
