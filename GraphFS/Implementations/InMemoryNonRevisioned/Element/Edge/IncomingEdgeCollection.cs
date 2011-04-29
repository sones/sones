using System;
using System.Collections.Generic;
using System.Linq;
using sones.Library.PropertyHyperGraph;
using System.Collections;
using sones.GraphFS.Element.Vertex;

namespace sones.GraphFS.Element.Edge
{
    /// <summary>
    /// This class stores a distict collection of single edges and is optimized for insert
    /// </summary>
    public sealed class IncomingEdgeCollection : IEnumerable<IVertex>
    {
        #region data

        /// <summary>
        /// The single edges that are contained in this structure
        /// </summary>
        private InMemoryVertex[] _containedVertices;

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
        public IncomingEdgeCollection(Int32 myStartingSize = 100)
        {
            _containedVertices = new InMemoryVertex[myStartingSize];
            _idx = 0;
            _isDirty = false;
        }

        /// <summary>
        /// Create a new SinglEdgeCollection
        /// </summary>
        /// <param name="myFirstVertex">The first vertex of the collection</param>
        /// <param name="myStartingSize">The starting size of the underlying data structure</param>
        public IncomingEdgeCollection(InMemoryVertex myFirstVertex, Int32 myStartingSize = 100)
        {
            _containedVertices = new InMemoryVertex[myStartingSize];
            _idx = 0;

            _containedVertices[_idx++] = myFirstVertex;
            _isDirty = true;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Removes a vertex from the collection
        /// </summary>
        /// <param name="myVertex">The vertex to remove.</param>
        public void RemoveVertex(InMemoryVertex myVertex)
        {
            var contElements = _containedVertices.Where(item => item == myVertex);
            _containedVertices = _containedVertices.Except(contElements).ToArray();
            _idx = _idx - contElements.Count();

            _isDirty = true;
        }

        /// <summary>
        /// Adds a vertex to the collection
        /// </summary>
        /// <param name="myVertex">The edge that is going to be added</param>
        public void AddVertex(InMemoryVertex myVertex)
        {
            if (_idx >= _containedVertices.Length)
            {
                #region grow

                var newArray = new InMemoryVertex[_containedVertices.Length * 2];

                Array.Copy(_containedVertices, newArray, _containedVertices.Length);

                _containedVertices = newArray;

                #endregion
            }

            _containedVertices[_idx++] = myVertex;

            _isDirty = true;
        }

        /// <summary>
        /// returns all edges
        /// </summary>
        /// <returns>A distinct array of single edges</returns>
        public InMemoryVertex[] GetAllVertices()
        {
            //Todo: do sth faster here
            Reorganize();

            return _containedVertices;
        }

        #endregion

        #region IEnumerable<IVertex> Members

        public IEnumerator<IVertex> GetEnumerator()
        {
            Reorganize();

            return _containedVertices.AsEnumerable<IVertex>().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            Reorganize();

            return _containedVertices.GetEnumerator();
        }

        #endregion

        #region private helper

        /// <summary>
        /// Reorganize this datastructure to contain unique values only
        /// </summary>
        private void Reorganize()
        {
            //This is really slow :(

            if (_isDirty)
            {
                HashSet<InMemoryVertex> helper = new HashSet<InMemoryVertex>(_containedVertices);

                helper.Remove(null);

                _containedVertices = helper.ToArray();

                _isDirty = false;
            }
        }

        #endregion
    }
}