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
    public sealed class IncomingEdgeCollection : ISet<InMemoryVertex>, ISet<IVertex>
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
            if (_isDirty)
            {
                //Todo: do sth faster here
                HashSet<InMemoryVertex> helper = new HashSet<InMemoryVertex>(_containedVertices);

                _containedVertices = helper.ToArray();

                _isDirty = false;
            }

            return _containedVertices;
        }

        #endregion

        #region ISet<InMemoryVertex> Members

        public bool Add(InMemoryVertex item)
        {
            AddVertex(item);
            return true;
        }

        public void ExceptWith(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<InMemoryVertex> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<InMemoryVertex> Members

        void ICollection<InMemoryVertex>.Add(InMemoryVertex item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(InMemoryVertex item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(InMemoryVertex[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(InMemoryVertex item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<InMemoryVertex> Members

        public IEnumerator<InMemoryVertex> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISet<IVertex> Members

        public bool Add(IVertex item)
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<IVertex> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<IVertex> Members

        void ICollection<IVertex>.Add(IVertex item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IVertex item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IVertex[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IVertex item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<IVertex> Members

        IEnumerator<IVertex> IEnumerable<IVertex>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}