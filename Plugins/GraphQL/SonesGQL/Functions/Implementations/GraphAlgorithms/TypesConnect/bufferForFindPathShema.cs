using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace TypesConnect
{
    #region bufferForFindPathShema
    public class bufferForFindPathShema
    {

        SortedDictionary<Tuple<double, long>, Tuple<IVertexType, double, ulong>> buf;


        public int Count { get { return count; } }
        int count;

        public bufferForFindPathShema()
        {
            buf = new SortedDictionary<Tuple<double, long>, Tuple<IVertexType, double, ulong>>();

            count = 0;

        }

        public void add(IVertexType current_node, double current_distance, UInt64 current_depth)
        {
            var id = current_node.ID;
            buf.Add(Tuple.Create(current_distance, id), Tuple.Create(current_node, current_distance, current_depth));

            count++;



        }
        public Tuple<IVertexType, double, ulong> min()
        {
            return buf.ElementAt(0).Value;
        }


        public void remove(double key_primary, long key_secondary)
        {
            buf.Remove(Tuple.Create(key_primary, key_secondary));
            count--;
        }

        public void set(double key_primary, IVertexType value, double current_distance, ulong current_depth)
        {
            var key = value.ID;
            buf.Remove(Tuple.Create(key_primary, key));
            buf.Add(Tuple.Create(current_distance, key), Tuple.Create(value, current_distance, current_depth));
        }

        public ulong getDepth(double key_primary, long current_vertex)
        {
            return buf[Tuple.Create(key_primary, current_vertex)].Item3;
        }

        public ulong getDepth(int current_vertexID)
        {
            return buf.ElementAt(current_vertexID).Value.Item3;
        }

        public double getDistance(double key_primary, long current_vertex)
        {
            return buf[Tuple.Create(key_primary, current_vertex)].Item2;
        }

        public double getDistance(int current_vertexID)
        {
            return buf.ElementAt(current_vertexID).Value.Item2;
        }

        public Tuple<IVertexType, double, ulong> getElement(double key_primary, long index)
        {
            Tuple<IVertexType, double, ulong> output;
            buf.TryGetValue(Tuple.Create(key_primary, index), out output);
            return output;
        }

        public void Clear()
        {
            buf.Clear();
            count = 0;
        }



    }
    #endregion
}
