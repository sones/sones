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

namespace sones.Plugins.Index.Lucene
{
    public class SonesLuceneIndex : ASonesIndex, ISonesPersistentIndex, ISonesFulltextIndex, IPluginable
    {
        #region ISonesPersistentIndex
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISonesFulltextIndex
        
        public ISonesFulltextResult Query(string myQuery)
        {
            throw new NotImplementedException();
        }

        public long KeyCount(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComparable> Keys(long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public IDictionary<long, Type> GetKeyTypes()
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<ICompoundIndexKey> myKeys, long myVertexID, Helper.IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<KeyValuePair<IEnumerable<ICompoundIndexKey>, long>> myKeysValuePairs, Helper.IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValues(IEnumerable<ICompoundIndexKey> myKeys, out IEnumerable<long> myVertexIDs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValuesPartial(IEnumerable<ICompoundIndexKey> myKeys, out IEnumerable<long> myVertexIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> this[IEnumerable<ICompoundIndexKey> myKeys]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IPluginable
        
        public string PluginName
        {
            get { throw new NotImplementedException(); }
        }

        public string PluginShortName
        {
            get { throw new NotImplementedException(); }
        }

        public string PluginDescription
        {
            get { throw new NotImplementedException(); }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { throw new NotImplementedException(); }
        }

        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ASonesIndex
        
        public override string IndexName
        {
            get { throw new NotImplementedException(); }
        }

        public override long KeyCount()
        {
            throw new NotImplementedException();
        }

        public override long ValueCount()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IComparable> Keys()
        {
            throw new NotImplementedException();
        }

        public override Type GetKeyType()
        {
            throw new NotImplementedException();
        }

        public override void Add(IComparable myKey, long? myVertexID, Helper.IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValues(IComparable myKey, out IEnumerable<long> myVertexIDs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<long> this[IComparable myKey]
        {
            get { throw new NotImplementedException(); }
        }

        public override bool ContainsKey(IComparable myKey)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(IComparable myKey)
        {
            throw new NotImplementedException();
        }

        public override void RemoveRange(IEnumerable<IComparable> myKeys)
        {
            throw new NotImplementedException();
        }

        public override bool TryRemoveValue(IComparable myKey, long myValue)
        {
            throw new NotImplementedException();
        }

        public override void Optimize()
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool SupportsNullableKeys
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
