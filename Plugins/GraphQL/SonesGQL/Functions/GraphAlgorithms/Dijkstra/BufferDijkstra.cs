using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.SonesGQL.Functions.Dijkstra
{
    #region buffer
    public class bufferDijkstra
    {

        SortedDictionary<Tuple<double, long>, Tuple<IVertex, double, ulong>> buffer;


        public int Count { get { return count; } }
        int count;

        public bufferDijkstra()
        {
            buffer = new SortedDictionary<Tuple<double, long>, Tuple<IVertex, double, ulong>>();

            count = 0;

        }

        public void add(IVertex current_node, double current_distance, UInt64 current_depth)
        {
            var id = current_node.VertexID;
            buffer.Add(Tuple.Create(current_distance, id), Tuple.Create(current_node, current_distance, current_depth));

            count++;



        }
        public Tuple<IVertex, double, ulong> min()
        {
            return buffer.ElementAt(0).Value;
        }


        public void remove(double key_primary, long key_secondary)
        {
            buffer.Remove(Tuple.Create(key_primary, key_secondary));
            count--;
        }

        public void set(double key_primary, IVertex value, double current_distance, ulong current_depth)
        {
            var key = value.VertexID;
            buffer.Remove(Tuple.Create(key_primary, key));
            buffer.Add(Tuple.Create(current_distance, key), Tuple.Create(value, current_distance, current_depth));
        }

        public ulong getDepth(double key_primary, long current_vertex)
        {
            return buffer[Tuple.Create(key_primary, current_vertex)].Item3;
        }

        public ulong getDepth(int current_vertexID)
        {
            return buffer.ElementAt(current_vertexID).Value.Item3;
        }

        public double getDistance(double key_primary, long current_vertex)
        {
            return buffer[Tuple.Create(key_primary, current_vertex)].Item2;
        }

        public double getDistance(int current_vertexID)
        {
            return buffer.ElementAt(current_vertexID).Value.Item2;
        }

        public Tuple<IVertex, double, ulong> getElement(double key_primary, long index)
        {
            Tuple<IVertex, double, ulong> output;
            buffer.TryGetValue(Tuple.Create(key_primary, index), out output);
            return output;
        }

        public void Clear()
        {
            buffer.Clear();
            count = 0;
        }



    }
    #endregion
}
