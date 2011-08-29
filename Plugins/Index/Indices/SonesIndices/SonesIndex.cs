using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Helper;
using sones.Library.PropertyHyperGraph;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using sones.Plugins.Index.ErrorHandling;
using sones.Library.VersionedPluginManager;
using System.Threading;

namespace sones.Plugins.Index
{
    /// <summary>
    /// This index is the default implementation of
    /// a sones GraphDB index.
    /// 
    /// This in-memory index wraps a .NET Concurrent
    /// Dictionary to fulfill the methods.
    /// It represents a multiple-value index where
    /// 1-n values can be associated with a search key.
    /// </summary>
    public class SonesIndex : ISonesIndex, IPluginable
    {
        #region Private Members

        /// <summary>
        /// internal index structure
        /// </summary>
        private readonly ConcurrentDictionary<IComparable, HashSet<Int64>> _Index;

        #region Counts

        /// <summary>
        /// Number of stored values
        /// </summary>
        private Int64 _ValueCount;

        #endregion

        #region Helper

        /// <summary>
        /// used for some thread safe actions
        /// </summary>
        private object _Lock;

        #endregion

        #region PropertyID

        /// <summary>
        /// The ID of the indexed property
        /// </summary>
        private Int64 _PropertyID;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the SonesIndex
        /// </summary>
        public SonesIndex()
        {
            _Index      = new ConcurrentDictionary<IComparable, HashSet<Int64>>();

            _ValueCount = 0;
            _Lock       = new object();
        }

        public SonesIndex(IList<Int64> myPropertyIDs) : this()
        {
            _PropertyID = myPropertyIDs.FirstOrDefault();
        }

        #endregion

        #region ISonesIndex Members

        /// <summary>
        /// Returns the name of the index
        /// </summary>
        public string IndexName
        {
            get { return "sonesindex"; }
        }

        /// <summary>
        /// Returns the number of keys in the index.
        /// </summary>
        /// <returns>Number of keys</returns>
        public long KeyCount()
        {
            return _Index.Keys.LongCount();
        }

        /// <summary>
        /// Returns the number of values in the index.
        /// </summary>
        /// <returns>Number of values</returns>
        public long ValueCount()
        {
            return _ValueCount;
        }

        /// <summary>
        /// Returns all keys currently stored in the index
        /// </summary>
        /// <returns>All stored keys</returns>
        public IEnumerable<IComparable> Keys()
        {
            return _Index.Keys;
        }

        /// <summary>
        /// Sets the propertyID for internal processing
        /// </summary>
        /// <param name="myPropertyID">ID of the indexed property</param>
        public void Init(long myPropertyID)
        {
            _PropertyID = myPropertyID;
        }

        /// <summary>
        /// Returns the type of the stored keys or <code>IComparable</code>
        /// if the index is empty.
        /// </summary>
        /// <returns>
        /// The type of the stored keys or <code>IComparable</code>
        /// if the index is empty.
        /// </returns>
        public Type GetKeyType()
        {
            if (!_Index.IsEmpty)
            {
                return _Index.First().GetType();
            }
            return typeof(IComparable);
        }

        /// <summary>
        /// Adds the vertexID of the given vertex to the index based
        /// on the indexed property.
        /// 
        /// Throws <code>IndexAddFailedException</code> if the given vertex
        /// doesn't have the indexed property.
        /// </summary>
        /// <param name="myVertex">Vertex to add (vertexID)</param>
        /// <param name="myIndexAddStrategy">Define what happens if the key already exists.</param>
        public void Add(IVertex myVertex, 
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            // check if vertex has property
            if (myVertex.HasProperty(_PropertyID))
            {
                // get property
                var prop = myVertex.GetProperty(_PropertyID);
                // and add it to the index
                Add(prop, myVertex.VertexID, myIndexAddStrategy);
            }
            else
            {
                throw new IndexAddFailedException(String.Format("Vertex {0} has no indexable property with ID {1}",
                    myVertex.VertexID,
                    _PropertyID));
            }
        }

        /// <summary>
        /// Adds given vertices to the index.
        /// </summary>
        /// <param name="myVertices">A collection of indexes</param>
        /// <param name="myIndexAddStrategy">Define what happens when a key already exists.</param>
        public void AddRange(IEnumerable<IVertex> myVertices, 
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            Parallel.ForEach(myVertices, v => Add(v, myIndexAddStrategy));
        }

        /// <summary>
        /// Adds a given key and a vertexID to the index.
        /// </summary>
        /// <param name="myKey">Search key</param>
        /// <param name="myVertexID">Associated vertexID</param>
        /// <param name="myIndexAddStrategy">Define what happens if key exists</param>
        public void Add(IComparable myKey, long myVertexID, 
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            HashSet<Int64> values;
            HashSet<Int64> newValues = new HashSet<long>() { myVertexID };

            lock (_Lock)
            {                
                int valueCountDiff = 0;

                if (_Index.TryGetValue(myKey, out values))
                {
                    // subtract number of old values
                    valueCountDiff -= values.Count;

                    switch (myIndexAddStrategy)
                    {
                        case IndexAddStrategy.MERGE:
                            values.UnionWith(newValues);
                            valueCountDiff += values.Count;
                            break;
                        case IndexAddStrategy.REPLACE:
                            values = newValues;
                            valueCountDiff++;
                            break;
                        case IndexAddStrategy.UNIQUE:
                            throw new IndexKeyExistsException(String.Format("Index key {0} already exist.", myKey.ToString()));
                    }
                }
                else
                {
                    values = newValues;
                    valueCountDiff += newValues.Count;
                }
                
                // add it to the index
                _Index.AddOrUpdate(myKey, values, (k, v) => values);
                
                // update the value count
                _ValueCount += valueCountDiff;
            }
        }

        /// <summary>
        /// Adds a given collection of key-value-pairs to the index.
        /// </summary>
        /// <param name="myKeyValuePairs">Key-Value-Pairs</param>
        /// <param name="myIndexAddStrategy">Define what happes if a key already exists.</param>
        public void AddRange(IEnumerable<KeyValuePair<IComparable, long>> myKeyValuePairs, 
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            Parallel.ForEach(myKeyValuePairs, kvp => Add(kvp.Key, kvp.Value, myIndexAddStrategy));
        }

        /// <summary>
        /// Returns all values associated with given key, if it exists.
        /// </summary>
        /// <param name="myKey">Search key</param>
        /// <param name="myVertexIDs">Associated values</param>
        /// <returns>True, if the key exists</returns>
        public bool TryGetValues(IComparable myKey, out IEnumerable<long> myVertexIDs)
        {
            var values = new HashSet<long>();
            var done = _Index.TryGetValue(myKey, out values);
            myVertexIDs = values;
            return done;
        }

        /// <summary>
        /// Returns all values for the given key or throws a
        /// <code>IndexKeyNotFoundException</code> if the key
        /// is not stored in the index.
        /// </summary>
        /// <param name="myKey">Search key</param>
        /// <returns>All values associated to the key.</returns>
        public IEnumerable<long> this[IComparable myKey]
        {
            get 
            {
                HashSet<Int64> values;
                if (_Index.TryGetValue(myKey, out values))
                {
                    return values;
                }
                else
                {
                    throw new IndexKeyNotFoundException(String.Format("Index key {0} not found.", myKey.ToString()));
                }
            }
        }

        /// <summary>
        /// Checks, if a key exists in the index.
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        public bool ContainsKey(IComparable myKey)
        {
            return _Index.ContainsKey(myKey);
        }

        #region Remove / RemoveRange / TryRemoveValue

        /// <summary>
        /// Removes a key from the index.
        /// </summary>
        /// <param name="myKey"></param>
        public bool Remove(IComparable myKey)
        {
            HashSet<Int64> values;
            if (!_Index.TryRemove(myKey, out values))
            {
                return false;
            }
            else
            {
                lock (_Lock)
                {
                    // update value count
                    _ValueCount -= values.Count;
                }
                return true;
            }
        }

        /// <summary>
        /// Removes a value associated with the given key 
        /// from the index.
        /// </summary>
        /// <param name="myKey">Search key</param>
        /// <param name="myValue">Value to be removed.</param>
        /// <returns>True, if the value has been removed.</returns>
        public bool TryRemoveValue(IComparable myKey, Int64 myValue)
        {
            HashSet<Int64> values;

            lock (_Lock)
            {
                // get values from internal index
                if (_Index.TryGetValue(myKey, out values))
                {
                    // if the defined value is stored
                    if (values.Contains(myValue))
                    {
                        // then remove it
                        values.Remove(myValue);
                        // decrement the value counter
                        _ValueCount--;
                        // and if there are still values associated with the key
                        if (values.Count > 0)
                        {
                            // update the value set
                            _Index.AddOrUpdate(myKey, values, (key, vals) => vals = values);
                        }
                        else
                        {
                            // or remove the key
                            return Remove(myKey);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all given key from the index.
        /// </summary>
        /// <param name="myKeys">Search keys to be removed</param>
        public void RemoveRange(IEnumerable<IComparable> myKeys)
        {
            Parallel.ForEach(myKeys, k => Remove(k));
        }

        /// <summary>
        /// Checks if the given vertex is indexed and if yes, removes
        /// the vertex id from the index.
        /// </summary>
        /// <param name="myVertex">Vertex which ID shall be removed</param>
        /// <returns>True, if the vertexID has been removed from the index.</returns>
        public bool Remove(IVertex myVertex)
        {
            // check if vertex has property
            if (myVertex.HasProperty(_PropertyID))
            {
                // get property
                var prop = myVertex.GetProperty(_PropertyID);
                // and try to remove if from the index
                return TryRemoveValue(prop, myVertex.VertexID);
            }
            else
            {
                throw new IndexRemoveFailedException(String.Format("Vertex {0} has no indexable property with ID {1}",
                    myVertex.VertexID,
                    _PropertyID));
            }
        }

        #endregion

        /// <summary>
        /// Internal index structure doesn't support optimizations.
        /// </summary>
        public void Optimize()
        {
            // silence is golden
        }

        #endregion

        #region IPluginable Members

        /// <summary>
        /// Plugin name of this index.
        /// </summary>
        public string PluginName
        {
            get { return "sones.sonesindex"; }
        }

        /// <summary>
        /// Plugin short name of this index.
        /// </summary>
        public string PluginShortName
        {
            get { return "sonesidx"; }
        }

        /// <summary>
        /// Paramters which can be set for that index.
        /// </summary>
        public PluginParameters<Type> SetableParameters
        {
            get
            {
                return new PluginParameters<Type>
                {
                };
            }
        }

        /// <summary>
        /// Returns a new instance of that class.
        /// </summary>
        /// <param name="UniqueString"></param>
        /// <param name="myParameters"></param>
        /// <returns>A new instance of SonesIndex</returns>
        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            object propertyIDs;

            if (myParameters.TryGetValue(IndexConstants.PROPERTY_IDS_OPTIONS_KEY, out propertyIDs))
            {
                return new SonesIndex((IList<Int64>) propertyIDs);
            }
            else
            {
                return new SonesIndex();
            }
        }

        /// <summary>
        /// Frees all ressources used by the index
        /// </summary>
        public void Dispose()
        {}

        #endregion

        #region Clear

        /// <summary>
        /// Clears internal index
        /// </summary>
        public void Clear()
        {
            _Index.Clear();

            Interlocked.Exchange(ref _ValueCount, 0L);            
        }

        #endregion
    }
}
