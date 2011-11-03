using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace TypesConnect
{
    #region dataDijkstraForFindPathShema
    public class dataForFindPathShema
    {

        Dictionary<long, Tuple<IVertexType, double, ulong, long, IVertexType>> list;
        public int Count { get { return Count; } }

        int count;

        public dataForFindPathShema()
        {
            list = new Dictionary<long, Tuple<IVertexType, double, ulong, long, IVertexType>>();
            count = 0;

        }
        public void add(IVertexType current_node, double current_distance, UInt64 current_depth, long current_edge, IVertexType father)
        {
            var id = current_node.ID;
            list.Add(id, Tuple.Create(current_node, current_distance, current_depth, current_edge, father));
            count++;
        }


        public void set(IVertexType value, double current_distance, ulong current_depth, long current_edge, IVertexType father)
        {
            var key = value.ID;
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

        public Tuple<IVertexType, double, ulong, long, IVertexType> getElement(long key)
        {
            Tuple<IVertexType, double, ulong, long, IVertexType> temp;
            list.TryGetValue(key, out temp);
            return temp;
        }

        private Tuple<IVertexType, double, ulong, long, IVertexType> getTuple(int key)
        {
            return this.list.ElementAt(key).Value;
        }

        private Tuple<IVertexType, double, ulong, long, IVertexType> getTuple(long key)
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
