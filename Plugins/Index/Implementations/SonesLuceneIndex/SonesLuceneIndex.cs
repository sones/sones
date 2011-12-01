using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Compound;
using sones.Plugins.Index.Abstract;
using sones.Plugins.Index.Persistent;
using sones.Plugins.Index.Fulltext;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.LuceneIdx;
using sones.Plugins.Index.ErrorHandling;
using sones.Library.CollectionWrapper;

namespace sones.Plugins.Index.LuceneIdx
{
    public class LuceneCompoundKey : ICompoundIndexKey
    {
        private long _PropertyID;
        private String _Key;

        public LuceneCompoundKey(long myPropertyID, String myKey)
        {
            _PropertyID = myPropertyID;
            _Key = myKey;
        }

        public long PropertyID
        {
            get { return _PropertyID; }
        }

        public IComparable Key
        {
            get { return _Key; }
        }
    }

    public class SonesLuceneIndex : ASonesIndex, ISonesPersistentIndex, ISonesFulltextIndex, IPluginable
    {
        #region Data

        /// <summary>
        /// The lucene index connector.
        /// </summary>
        private LuceneIndex _LuceneIndex;

        #endregion

        #region Settings

        private const int _MaxResultsFirst = 100;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the id of this index within the Solr instance.
        /// </summary>
        public String IndexId
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        public SonesLuceneIndex()
        {

        }
        
        public SonesLuceneIndex(String myIndexId, String myPersistencePath = null, IList<Int64> myPropertyIDs = null)
        {
            if (myIndexId == null)
                throw new ArgumentNullException("myIndexId");

            if (myPersistencePath == null)
            {
                _LuceneIndex = new LuceneIndex(myIndexId);
            }
            else
            {
                _LuceneIndex = new LuceneIndex(myIndexId, myPersistencePath);
            }

            IndexId = myIndexId;
        }

        #endregion

        #region ISonesPersistentIndex

        public void Dispose()
        {
            
        }

        public void Shutdown()
        {
            
        }

        #endregion

        #region ISonesFulltextIndex
        
        public ISonesFulltextResult Query(string myQuery)
        {
            var result = _LuceneIndex.GetEntries(_MaxResultsFirst, myQuery);

            if (result.TotalHits > _MaxResultsFirst)
            {
                result = _LuceneIndex.GetEntries(result.TotalHits, myQuery);
            }

            var lucene_result =  new LuceneResult(result);
            result.Close();

            return lucene_result;
        }

        public long KeyCount(long myPropertyID)
        {
            var keys = _LuceneIndex.GetKeys(entry => (entry.IndexId == this.IndexId) && (entry.PropertyId == myPropertyID));
            var groupedkeys = keys.GroupBy(s => s);
            var count = groupedkeys.Count();
            keys.Close();

            return count;
        }

        public IEnumerable<IComparable> Keys(long myPropertyID)
        {
            var result = _LuceneIndex.GetEntriesInnerByField(_MaxResultsFirst, "*:*", myPropertyID.ToString(), LuceneIndex.Fields.PROPERTY_ID);
            if (result.TotalHits > _MaxResultsFirst)
            {
                result.Close();
                result = _LuceneIndex.GetEntriesInnerByField(result.TotalHits, "*:*", myPropertyID.ToString(), LuceneIndex.Fields.PROPERTY_ID);
            }

            // Unfortunately we have to breakup Lazy here as index interface doesn't support close (GRAPHDB-544)
            List<IComparable> ret = new List<IComparable>();

            foreach (var entry in result.Select<LuceneEntry, IComparable>((e) => (e.Text)))
            {
                ret.Add(entry);
            }

            result.Close();

            return ret;
        }

        public IDictionary<long, Type> GetKeyTypes()
        {
            var retdict = new Dictionary<Int64, Type>();
            foreach (var propId in _PropertyIDs)
            {
                retdict.Add(propId, typeof(string));
            }
            return retdict;
        }

        public void Add(IEnumerable<ICompoundIndexKey> myKeys, long myVertexID, Helper.IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            foreach (var key in myKeys)
            {
                AddEntry(key.Key, new HashSet<long>() { myVertexID }, myIndexAddStrategy, key.PropertyID);
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<IEnumerable<ICompoundIndexKey>, long>> myKeysValuePairs, Helper.IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            foreach (var kvp in myKeysValuePairs)
            {
                foreach (var key in kvp.Key)
                {
                    AddEntry(key.Key, new HashSet<long>() { kvp.Value }, myIndexAddStrategy, key.PropertyID);
                }
            }
        }

        public bool TryGetValues(IEnumerable<ICompoundIndexKey> myKeys, out IEnumerable<long> myVertexIDs)
        {
            LuceneReturn results = null;
            var results_compound = new List<Tuple<long, IComparable, long>>();
            foreach (var key in myKeys)
            {
                results = _LuceneIndex.GetEntriesInnerByField(_MaxResultsFirst, key.Key as String, key.PropertyID.ToString(), LuceneIndex.Fields.PROPERTY_ID);
                if (results.TotalHits > _MaxResultsFirst)
                {
                    results.Close();
                    results = _LuceneIndex.GetEntriesInnerByField(results.TotalHits, key.Key as String, key.PropertyID.ToString(), LuceneIndex.Fields.PROPERTY_ID);
                }
                
                // Unfortunately we have to breakup Lazy here as index interface doesn't support close (GRAPHDB-544)
                foreach (var entry in results
                             .Where((e) => e.PropertyId != null)
                             .Select((e) => new Tuple<long, IComparable, long>((long)e.PropertyId, e.Text, e.VertexId)))
                {
                    results_compound.Add(entry);
                }
                results.Close();
            }

            var grouped = from myresults in results_compound group myresults by myresults.Item3;

            if (grouped.Count() > 0)
            {
                var _myVertexIDs = grouped
                    .Where((myGroup) =>
                    {
                        var join =
                            from entry in myGroup
                            join key in myKeys
                            on new
                            {
                                JoinField1 = entry.Item2,
                                JoinField2 = entry.Item1
                            }
                            equals new
                            {
                                JoinField1 = key.Key,
                                JoinField2 = key.PropertyID
                            }
                            select entry;

                        if (join.Count() == myKeys.Count())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    })
                    .Select<IGrouping<long, Tuple<long, IComparable, long>>, long>((g) => g.Key);

                if (_myVertexIDs.Count() > 0)
                {
                    myVertexIDs = _myVertexIDs;
                    return true;
                }
                else
                {
                    myVertexIDs = null;
                    return false;
                }
            }
            else
            {
                myVertexIDs = null;
                return false;
            }
        }

        public bool TryGetValuesPartial(IEnumerable<ICompoundIndexKey> myKeys, out IEnumerable<long> myVertexIDs)
        {
            LuceneReturn results = null;
            var results_compound = new List<Tuple<long, IComparable, long>>();
            foreach (var key in myKeys)
            {
                results = _LuceneIndex.GetEntriesInnerByField(_MaxResultsFirst, key.Key as String, key.PropertyID.ToString(), LuceneIndex.Fields.PROPERTY_ID);
                if (results.TotalHits > _MaxResultsFirst)
                {
                    results.Close();
                    results = _LuceneIndex.GetEntriesInnerByField(_MaxResultsFirst, key.Key as String, key.PropertyID.ToString(), LuceneIndex.Fields.PROPERTY_ID);
                }
                
                // Unfortunately we have to breakup Lazy here as index interface doesn't support close (GRAPHDB-544)
                foreach (var entry in results.Where((e) => e.PropertyId != null).Select((e) => new Tuple<long, IComparable, long>((long)e.PropertyId, e.Text, e.VertexId)))
                {
                    results_compound.Add(entry);
                }
                results.Close();
            }

            var grouped = from myresults in results_compound group myresults by myresults.Item3;

            if (grouped.Count() > 0)
            {
                myVertexIDs = grouped.Select<IGrouping<long, Tuple<long, IComparable, long>>, long>((g) => g.Key);
                return true;
            }
            else
            {
                myVertexIDs = null;
                return false;
            }
        }

        public IEnumerable<long> this[IEnumerable<ICompoundIndexKey> myKeys]
        {
            get
            {
                IEnumerable<long> myVertexIDs = null;
                if (TryGetValues(myKeys, out myVertexIDs))
                {
                    return myVertexIDs;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region IPluginable
        
        public string PluginName
        {
            get { return "sones.sonesindexlucene"; }
        }

        public string PluginShortName
        {
            get { return "sones.sonesidxluc"; }
        }

        public string PluginDescription
        {
            get { return "An index which represents a 1:n key:value relationship using SOLR."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get
            {
                return new PluginParameters<Type>
				{ 
                    { "LucenePersistent", typeof(bool) },
					{ "LuceneDirectory", typeof(String) }
				};
            }
        }

        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            String directory = null;
            bool bPersistent = false;

            if (myParameters != null && myParameters.ContainsKey("LucenePersistent"))
            {
                bPersistent = (bool)myParameters["LucenePersistent"];

                if (myParameters != null && myParameters.ContainsKey("LuceneDirectory"))
                {
                    directory = (String)myParameters["LuceneDirectory"];
                }
                else
                {
                    bPersistent = false;
                }
            }

            if (bPersistent)
            {
                return new SonesLuceneIndex(UniqueString, directory);
            }
            else
            {
                return new SonesLuceneIndex(UniqueString);
            }
        }

        #endregion

        #region ASonesIndex
        
        public override string IndexName
        {
            get { return "sonesindexpersistentsolr"; }
        }

        public override long KeyCount()
        {
            var keys = _LuceneIndex.GetKeys(entry => entry.IndexId == this.IndexId);
            var groupedkeys = keys.GroupBy(s => s);
            var count = groupedkeys.Count();
            keys.Close();

            return count;
        }

        public override long ValueCount()
        {
            var ret = _LuceneIndex.GetValues();
            var retgrouped = ret.GroupBy(l => l);
            var count = retgrouped.Count();
            ret.Close();
            return count;
        }

        public override IEnumerable<IComparable> Keys()
        {
            var keys_withdoubles = _LuceneIndex.GetKeys(entry => entry.IndexId == this.IndexId);
            var keys_grouped = keys_withdoubles.GroupBy(s => s);

            // Unfortunately we have to breakup Lazy here as index interface doesn't support close (GRAPHDB-544)
            List<String> keys = new List<String>();
            foreach (var group in keys_grouped)
            {
                keys.Add(group.ElementAt(0));
            }

            keys_withdoubles.Close();

            return keys;
        }

        public override Type GetKeyType()
        {
            return typeof(IComparable);
        }

        public override void Add(IComparable myKey, long? myVertexID, Helper.IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            if (myKey != null)
            {
                if (myVertexID == null)
                {
                    AddEntry(myKey, new HashSet<Int64>(), myIndexAddStrategy);
                }
                else
                {
                    AddEntry(myKey, new HashSet<Int64>() { (Int64)myVertexID }, myIndexAddStrategy);
                }
            }
        }

        public override bool TryGetValues(IComparable myKey, out IEnumerable<long> myVertexIDs)
        {
            if (myKey == null)
            {
                myVertexIDs = null;
                return false;
            }


            myVertexIDs = this[myKey];
            return myVertexIDs.Count() > 0;
        }

        public override IEnumerable<long> this[IComparable myKey]
        {
            get
            {
                if (myKey == null)
                {
                    return null;
                }

                string query = System.Convert.ToString(myKey);

                if (string.IsNullOrWhiteSpace(query))
                {
                    throw new ArgumentException(String.Format("Key {0} was not valid ", myKey));
                }

                var values_withdoubles = _LuceneIndex.GetEntriesInnerByField(_MaxResultsFirst, query, IndexId, LuceneIndex.Fields.INDEX_ID);
                if (values_withdoubles.TotalHits > _MaxResultsFirst)
                {
                    values_withdoubles = _LuceneIndex.GetEntriesInnerByField(values_withdoubles.TotalHits, query, IndexId, LuceneIndex.Fields.INDEX_ID);
                }

                var values_grouped = values_withdoubles.GroupBy(e => e.VertexId);
                
                // Unfortunately we have to breakup Lazy here as index interface doesn't support close (GRAPHDB-544)
                List<LuceneEntry> values = new List<LuceneEntry>();
                foreach (var group in values_grouped)
                {
                    values.Add(group.ElementAt(0));
                }
                values_withdoubles.Close();

                return values.Select(entry => entry.VertexId);
            }
        }

        public override bool ContainsKey(IComparable myKey)
        {
            if (myKey == null)
            {
                return false;
            }

            string query = System.Convert.ToString(myKey);

            if (string.IsNullOrWhiteSpace(query))
                return false;

            return _LuceneIndex.HasEntry(query, entry => entry.IndexId == this.IndexId);
        }

        public override bool Remove(IComparable myKey)
        {
            if (myKey == null)
            {
                return false;
            }

            var deleted = -1;
            if (ContainsKey(myKey))
            {
                deleted = _LuceneIndex.DeleteEntry(System.Convert.ToString(myKey), entry => entry.IndexId == this.IndexId);
            }
            return (deleted > 0);
        }

        public override void RemoveRange(IEnumerable<IComparable> myKeys)
        {
            foreach (var key in myKeys)
            {
                Remove(key);
            }
        }

        public override bool TryRemoveValue(IComparable myKey, long myValue)
        {
            if (myKey == null)
            {
                return false;
            }

            if (_LuceneIndex.DeleteEntry(myKey as String, (e) => (e.VertexId == myValue)) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Optimize()
        {
            _LuceneIndex.Optimize();
        }

        public override void Clear()
        {
            _LuceneIndex.Empty();
        }

        public override bool SupportsNullableKeys
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Private Helper

        /// <summary>
        /// Adds an entry to the index.
        /// </summary>
        /// <param name="myKey">The key.</param>
        /// <param name="myValues">The value.</param>
        /// <param name="myIndexAddStrategy">The index add strategy.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///		myKey is NULL.
        /// </exception>
        private void AddEntry(IComparable myKey, ISet<Int64> myValues, IndexAddStrategy myIndexAddStrategy, long? myPropertyID = null)
        {
            if (myKey == null)
                throw new ArgumentNullException("myKey");

            string key = myKey.ToString();
            
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            switch (myIndexAddStrategy)
            {
                case IndexAddStrategy.MERGE:
                    {
                        foreach (var item in myValues)
                        {
                            _LuceneIndex.AddEntry(new LuceneEntry(IndexId, System.Convert.ToInt64(item), key, myPropertyID));
                        }

                        break;
                    }
                case IndexAddStrategy.REPLACE:
                    {
                        string luceneQuery = key;

                        if (string.IsNullOrWhiteSpace(luceneQuery))
                        {
                            luceneQuery = "*:*";
                        }

                        var result = _LuceneIndex.GetEntries(_MaxResultsFirst, luceneQuery);
                        if (result.TotalHits > _MaxResultsFirst)
                        {
                            result = _LuceneIndex.GetEntries(result.TotalHits, luceneQuery);
                        }

                        var entries = result.Where(entry => entry.Text == key).ToList();
                        foreach (var entry in entries)
                        {
                            _LuceneIndex.DeleteEntry(entry);
                        }

                        foreach (var value in myValues)
                        {
                            _LuceneIndex.AddEntry(new LuceneEntry(IndexId, System.Convert.ToInt64(value), key, myPropertyID));
                        }

                        break;
                    }
                case IndexAddStrategy.UNIQUE:
                    {
                        bool hasKey = false;

                        if (string.IsNullOrWhiteSpace(myKey.ToString()))
                            hasKey = _LuceneIndex.GetKeys(entry => entry.IndexId == this.IndexId).Any(k => k == key);
                        else
                            hasKey = _LuceneIndex.HasEntry(key, entry => entry.IndexId == this.IndexId);

                        if (hasKey)
                        {
                            throw new IndexKeyExistsException(String.Format("Index key {0} already exist.", key));
                        }
                        else
                        {
                            foreach (var value in myValues)
                            {
                                _LuceneIndex.AddEntry(new LuceneEntry(IndexId, System.Convert.ToInt64(value), key, myPropertyID));
                            }
                        }

                        break;
                    }
            }
        }

        #endregion

    }
}
