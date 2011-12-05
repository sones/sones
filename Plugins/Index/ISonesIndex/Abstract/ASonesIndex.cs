using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Helper;
using sones.Library.PropertyHyperGraph;
using sones.Library.CollectionWrapper;
using sones.Plugins.Index.ErrorHandling;

namespace sones.Plugins.Index.Abstract
{
    public abstract class ASonesIndex : ISonesIndex
    {
        #region Protected Members

        /// <summary>
        /// Indexed propertyIDs
        /// </summary>
        protected IList<Int64> _PropertyIDs;

        #endregion

        #region Abstract Members

        public abstract string IndexName { get; }

        public abstract long KeyCount();

        public abstract long ValueCount();

        public abstract IEnumerable<IComparable> Keys();

        public abstract Type GetKeyType();

        public abstract void Add(IComparable myKey, long? myVertexID, 
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        public abstract bool TryGetValues(IComparable myKey, out IEnumerable<long> myVertexIDs);

        public abstract IEnumerable<long> this[IComparable myKey] { get; }

        public abstract bool ContainsKey(IComparable myKey);

        public abstract bool Remove(IComparable myKey);

        public abstract void RemoveRange(IEnumerable<IComparable> myKeys);

        public abstract bool TryRemoveValue(IComparable myKey, long myValue);

        public abstract void Optimize();

        public abstract void Clear();

        public abstract bool SupportsNullableKeys { get; }

        #endregion

        #region Implemented Members

        /// <summary>
        /// Sets the propertyID for internal processing
        /// </summary>
        /// 
        /// <param name="myPropertyID">ID of the indexed property</param>
        /// <param name="bReinit">forces reinitialization (index should be cleared first or otherwise could become inconsistent)</param>
        public void Init(IList<Int64> myPropertyIDs, bool bReinit = false)
        {
            if ((!bReinit) && (_PropertyIDs != null))
            {
                throw new InvalidOperationException("Init Method has already been called for this index or propertyIDs have been set in constructor!");
            }

            _PropertyIDs = myPropertyIDs;
        }

        /// <summary>
        /// Adds the vertexID of the given vertex to the index based
        /// on the indexed property.
        /// 
        /// Throws <code>IndexAddFailedException</code> if the given vertex
        /// doesn't have the indexed property and the index does not support
        /// null-value keys.
        /// </summary>
        /// <param name="myVertex">Vertex to add (vertexID)</param>
        /// <param name="myIndexAddStrategy">Define what happens if the key already exists.</param>
        public virtual void Add(IVertex myVertex,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            if (_PropertyIDs.Count() > 1)
            {
                var keys = CreateIndexEntries(_PropertyIDs, myVertex);

                AddRange(keys, myIndexAddStrategy);
            }
            else if (_PropertyIDs.Count() == 1)
            {
                var key = CreateIndexEntry(_PropertyIDs[0], myVertex);

                if (KeyNullSupportCheck(key))
                {
                    Add(key, myVertex.VertexID, myIndexAddStrategy);
                }
            } else
            {
                throw new ArgumentException("No property IDs have been registered for indexing using Init.");
            }
        }

        /// <summary>
        /// Adds given vertices to the index.
        /// </summary>
        /// <param name="myVertices">A collection of indexes.</param>
        /// <param name="myIndexAddStrategy">Define what happens when a key already exists.</param>
        public virtual void AddRange(IEnumerable<IVertex> myVertices, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            foreach (var vertex in myVertices)
            {
                Add(vertex, myIndexAddStrategy);
            }
        }

        /// <summary>
        /// Adds key-value-pairs to the index.
        /// </summary>
        /// <param name="myKeyValuePairs">Collection of key-value-pairs</param>
        /// <param name="myIndexAddStrategy">Define what happens, if a key already exists.</param>
        public virtual void AddRange(IEnumerable<KeyValuePair<IComparable, long?>> myKeyValuePairs, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            foreach (var kvPair in myKeyValuePairs)
            {
                Add(kvPair.Key, kvPair.Value, myIndexAddStrategy);
            }
        }

        /// <summary>
        /// Checks if the given vertex is indexed and if yes, removes
        /// the vertex id from the index.
        /// </summary>
        /// <param name="myVertex">Vertex which ID shall be removed</param>
        /// <returns>True, if the vertexID has been removed from the index.</returns>
        public virtual bool Remove(IVertex myVertex)
        {
            if (_PropertyIDs.Count() > 1)
            {
                bool bFail = false;
                var keys = CreateIndexEntries(_PropertyIDs, myVertex);

                foreach (var key in keys)
                {
                    if (!KeyNullSupportCheck(key.Key))
                    {
                        bFail = true;
                        continue;
                    }
                    if (!TryRemoveValue(key.Key, myVertex.VertexID))
                    {
                        bFail = true;
                    }
                }

                return !bFail;
            }
            else if (_PropertyIDs.Count() == 1)
            {
                var key = CreateIndexEntry(_PropertyIDs[0], myVertex);

                if (!KeyNullSupportCheck(key))
                {
                    return false;
                }

                return TryRemoveValue(key, myVertex.VertexID);
            }
            else
            {
                throw new ArgumentException("No property IDs have been registered for indexing using Init.");
            }
        }

        /// <summary>
        /// Removes a collection of vertices from the index.
        /// </summary>
        /// <param name="myVertices">The vertices to be removed</param>
        public virtual void RemoveRange(IEnumerable<IVertex> myVertices)
        {
            foreach (var vertex in myVertices)
            {
                Remove(vertex);
            }
        }

        #endregion

        #region Private Static Members

        /// <summary>
        /// Creates a search key by checking which properties are indexed.
        /// </summary>
        /// <param name="myIndexedProperties">Indexed properties</param>
        /// <param name="myVertex">Vertex which shall be stored in the index.</param>
        /// <returns>A search key for that index</returns>
        private static IComparable CreateIndexEntry(Int64 myIndexedProperty, IVertex myVertex)
        {
            if (myVertex.HasProperty(myIndexedProperty))
            {
                return myVertex.GetProperty(myIndexedProperty);
            }
            return null;
        }

        /// <summary>
        /// Creates a search key by checking which properties are indexed.
        /// </summary>
        /// <param name="myIndexedProperties">Indexed properties</param>
        /// <param name="myVertex">Vertex which shall be stored in the index.</param>
        /// <returns>A search key for that index</returns>
        private static IEnumerable<KeyValuePair<IComparable,long?>> CreateIndexEntries(IList<Int64> myIndexedProperties, IVertex myVertex)
        {
            if (myIndexedProperties.Count > 0)
            {
                var values = new List<KeyValuePair<IComparable,long?>>();
                for (int i = 0; i < myIndexedProperties.Count; i++)
                {
                    if (myVertex.HasProperty(myIndexedProperties[i]))
                    {
                        values.Add(new KeyValuePair<IComparable,long?>(myVertex.GetProperty(myIndexedProperties[i]), myVertex.VertexID));
                    }
                }
                return values;
            }
            throw new ArgumentException("A unique definition must contain at least one element.");
        }
        

        #endregion

        #region Protected Members

        /// <summary>
        /// Method checks if the given key is null and if the index supports null as key.
        /// </summary>
        /// <param name="myKey">The key to be checked</param>
        protected bool KeyNullSupportCheck(IComparable myKey)
        {
            if (myKey == null)
            {
                return SupportsNullableKeys;
            }
            else
            {
                return true;
            }
        }

        #endregion

    }
}
