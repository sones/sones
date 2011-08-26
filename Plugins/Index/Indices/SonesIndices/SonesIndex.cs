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

namespace sones.Plugins.Index
{
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

        public SonesIndex()
        {
            _Index      = new ConcurrentDictionary<IComparable, HashSet<Int64>>();

            _ValueCount = 0;
            _Lock       = new object();
        }

        #endregion

        #region ISonesIndex Members

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
        /// <returns></returns>
        public IEnumerable<IComparable> Keys()
        {
            return _Index.Keys;
        }

        /// <summary>
        /// Sets the propertyID for internal processing
        /// </summary>
        /// <param name="myPropertyID"></param>
        public void Init(long myPropertyID)
        {
            _PropertyID = myPropertyID;
        }

        /// <summary>
        /// Returns the type of the stored keys or <code>IComparable</code>
        /// if the index is empty.
        /// </summary>
        /// <returns></returns>
        public Type GetKeyType()
        {
            if (!_Index.IsEmpty)
            {
                return _Index.First().GetType();
            }
            return typeof(IComparable);
        }

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
        /// <param name="myKeyValuePairs"></param>
        /// <param name="myIndexAddStrategy"></param>
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
        /// Removes all given key from the index.
        /// </summary>
        /// <param name="myKeys"></param>
        public void RemoveRange(IEnumerable<IComparable> myKeys)
        {
            Parallel.ForEach(myKeys, k => Remove(k));
        }

        /// <summary>
        /// Internal index structure doesn't support optimizations.
        /// </summary>
        public void Optimize()
        {
            // silence is golden
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.sonesindex"; }
        }

        public string PluginShortName
        {
            get { return "sonesidx"; }
        }

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
        /// <returns></returns>
        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            return new SonesIndex();
        }

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
            _ValueCount = 0;
        }

        #endregion
    }
}
