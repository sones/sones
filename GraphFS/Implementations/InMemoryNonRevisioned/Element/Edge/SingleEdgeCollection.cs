using System;
using System.Collections.Generic;
using System.Linq;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// This class stores a distict collection of single edges and is optimized for insert
    /// </summary>
    public sealed class SingleEdgeCollection
    {
        #region data

        /// <summary>
        /// The single edges that are contained in this structure
        /// </summary>
        private SingleEdge[] _containedSingleEdges;

        /// <summary>
        /// The current idx
        /// </summary>
        private int _idx;

        /// <summary>
        /// A flag if the structure is marked as dirty, which means that sth has been added
        /// </summary>
        private bool _isDirty;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new SingleEdgeCollection
        /// </summary>
        /// <param name="myStartingSize">The starting size of the underlying data structure</param>
        public SingleEdgeCollection(Int32 myStartingSize = 100)
        {
            _containedSingleEdges = new SingleEdge[myStartingSize];
            _idx = 0;
            _isDirty = false;
        }

        /// <summary>
        /// Create a new SinglEdgeCollection
        /// </summary>
        /// <param name="myFirstEdge">The first edge of the collection</param>
        /// <param name="myStartingSize">The starting size of the underlying data structure</param>
        public SingleEdgeCollection(SingleEdge myFirstEdge, Int32 myStartingSize = 100)
        {
            _containedSingleEdges = new SingleEdge[myStartingSize];
            _idx = 0;

            _containedSingleEdges[_idx++] = myFirstEdge;
            _isDirty = true;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds an edge to the collection
        /// </summary>
        /// <param name="mySingleEdge">The edge that is going to be added</param>
        public void AddEdge(SingleEdge mySingleEdge)
        {
            if (_idx >= _containedSingleEdges.Length)
            {
                #region grow

                var newArray = new SingleEdge[_containedSingleEdges.Length*2];

                Array.Copy(_containedSingleEdges, newArray, _containedSingleEdges.Length);

                _containedSingleEdges = newArray;

                #endregion
            }

            _containedSingleEdges[_idx++] = mySingleEdge;

            _isDirty = true;
        }

        /// <summary>
        /// returns all edges
        /// </summary>
        /// <returns>A distinct array of single edges</returns>
        public SingleEdge[] GetAllEdges()
        {
            if (_isDirty)
            {
                //Todo: do sth faster here
                HashSet<SingleEdge> helper = new HashSet<SingleEdge>(_containedSingleEdges);

                _containedSingleEdges = helper.ToArray();

                _isDirty = false;
            }

            return _containedSingleEdges;
        }

        #endregion
    }
}