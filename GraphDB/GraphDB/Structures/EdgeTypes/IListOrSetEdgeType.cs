
using System;

namespace sones.GraphDB.Structures.EdgeTypes
{

    public interface IListOrSetEdgeType
    {
        /// <summary>
        /// Count all values
        /// </summary>
        /// <returns>The number of values</returns>
        UInt64 Count();
        
        /// <summary>
        /// Returns the top <paramref name="myNumOfEntries"/> values
        /// </summary>
        /// <param name="myNumOfEntries"></param>
        /// <returns></returns>
        IListOrSetEdgeType GetTopAsEdge(UInt64 myNumOfEntries);

        /// <summary>
        /// Union with a AEdgeType of the same type.
        /// </summary>
        /// <param name="myAListEdgeType"></param>
        void UnionWith(IListOrSetEdgeType myAListEdgeType);

        /// <summary>
        /// Removes all elements which exist more than once
        /// </summary>
        void Distinction();

        /// <summary>
        /// Clear all elements
        /// </summary>
        void Clear();

    }

}
