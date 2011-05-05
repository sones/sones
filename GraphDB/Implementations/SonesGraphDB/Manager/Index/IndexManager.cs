using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Plugin;
using System.Collections.Generic;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;
using System.Linq;
using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore;
using sones.Plugins.Index.Interfaces;
using sones.GraphDB.Request.CreateVertexTypes;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.LanguageExtensions;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeManagement.Base;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphDB.Manager.Index
{
    /// <summary>
    /// This class represents an index manager.
    /// </summary>
    /// The responsibilities of the index manager are creating, removing und retrieving of indices.
    /// Each database has one index manager.
    public sealed class IndexManager : IIndexManager, IManager
    {
        #region data

        /// <summary>
        /// The plugin manager that is used to generate new instances of indices
        /// </summary>
        private readonly GraphDBPluginManager _pluginManager;

        /// <summary>
        /// The potential parameters for plugin-indices
        /// </summary>
        private readonly Dictionary<string, PluginDefinition> _indexPluginParameter;

        private IVertexStore _vertexStore;

        private IManagerOf<IVertexTypeHandler> _vertexTypeManager;

        private Dictionary<long, IIndex<IComparable, Int64>> _indices;
        private IDManager _idManager;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new index manager
        /// </summary>
        /// <param name="myVertexStore">The vertex store of the graphDB</param>
        /// <param name="myPluginManager">The sones graphDB plugin manager</param>
        /// <param name="myPluginDefinitions">The parameters for plugin-indices</param>
        public IndexManager(IDManager myIDManager, GraphDBPluginManager myPluginManager, List<PluginDefinition> myPluginDefinitions = null)
        {
            _idManager = myIDManager;
            _pluginManager = myPluginManager;

            _indexPluginParameter = myPluginDefinitions != null 
                ? myPluginDefinitions.ToDictionary(key => key.NameOfPlugin, value => value) 
                : new Dictionary<string, PluginDefinition>();
        }

        #endregion

        #region IIndexManager Members

        public IIndexDefinition CreateIndex(IndexPredefinition myIndexDefinition, SecurityToken mySecurity, TransactionToken myTransaction, bool myIsUserDefined = true)
        {
            var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(myIndexDefinition.VertexTypeName, myTransaction, mySecurity);
            var indexID = _idManager[(long)BaseTypes.Index].GetNextID();
            var info = new VertexInformation((long)BaseTypes.Index, indexID);

            var index = _pluginManager.GetAndInitializePlugin<IIndex<IComparable, Int64>>(myIndexDefinition.TypeName, _indexPluginParameter[myIndexDefinition.TypeName].PluginParameter, indexID);

            var indexVertex = BaseGraphStorageManager.StoreIndex(
                _vertexStore,
                info,
                myIndexDefinition.Name,
                myIndexDefinition.Comment,
                DateTime.UtcNow.ToBinary(),
                myIndexDefinition.TypeName,
                GetIsSingleValue(index),
                GetIsRangeValue(index),
                GetIsVersionedValue(index),
                true,
                new VertexInformation((long)BaseTypes.VertexType, vertexType.ID),
                null,
                mySecurity,
                myTransaction);

            _indices.Add(indexID, index);

            return BaseGraphStorageManager.CreateIndexDefinition(indexVertex, vertexType);
        }

        public bool HasIndex(IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return myPropertyDefinition.InIndices.CountIsGreater(0);
        }

        public IEnumerable<IIndex<IComparable, long>> GetIndices(IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return myPropertyDefinition.InIndices.Select(_ => _indices[_.ID]);
        }

        public IIndex<IComparable, long> GetIndex(IVertexType myVertexType, IList<IPropertyDefinition> myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            myVertexType.CheckNull("myVertexType");
            myPropertyDefinition.CheckNull("myPropertyDefinition");
            
            if (myPropertyDefinition.Count == 0)
                throw new ArgumentOutOfRangeException("myPropertyDefinition", "At least one property must be given.");

            var propertyTypes = myPropertyDefinition.GroupBy(_ => _.RelatedType);
            foreach (var group in propertyTypes)
            {
                if (!myVertexType.IsAncestorOrSelf(group.Key))
                {
                    throw new ArgumentException(string.Format("The properties ({0}) defined on type {1} is not part of inheritance hierarchy of {2}.", 
                        string.Join(",", group.Select(_ => _.Name)), 
                        group.Key.Name, 
                        myVertexType.Name));
                }
            }

            var result = myVertexType.GetIndexDefinitions(true).Where(_ => myPropertyDefinition.SequenceEqual(_.IndexedProperties));

            if (!result.CountIsGreater(0))
                return null;

            if (result.CountIsGreater(1))
                //TODO better exception here.
                throw new UnknownDBException("There are more than one indices on the same sequence of properties.");

            return _indices[result.First().ID];

        }

        public string GetBestMatchingIndexName(bool myIsSingleValue, bool myIsRange, bool myIsVersioned)
        {
            IEnumerable<String> result;
            if (myIsSingleValue)
            {
                result = _pluginManager.GetPluginsForType<ISingleValueIndex<IComparable, Int64>>();
            }
            else 
            {
                result = _pluginManager.GetPluginsForType<IMultipleValueIndex<IComparable, Int64>>();
            }

            if (myIsRange)
            {
                result = result.Where(_ => _ is IRangeIndex<IComparable, Int64>);
            }
            else
            {
                result = result.Where(_ => !(_ is IRangeIndex<IComparable, Int64>));
            }

            if (myIsVersioned)
            {
                result = result.Where(_ => _ is IVersionedIndex<IComparable, Int64, Int64>);
            }
            else
            {
                result = result.Where(_ => !(_ is IVersionedIndex<IComparable, Int64, Int64>));
            }

            //ASK: Should this throw an exception, if no index is available?
            return result.First();
        }


        public IEnumerable<IIndexDefinition> DescribeIndex(String myTypeName, String myIndexName, String myEdition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertextype = _vertexTypeManager.ExecuteManager.GetVertexType(myTypeName, myTransaction, mySecurity);

            var indices = vertextype.GetIndexDefinitions(true);

            if (!string.IsNullOrWhiteSpace(myIndexName))
            {
                indices = indices.Where(x => myIndexName.Equals(x.Name));
            }
            
            return indices;
        }

        public void RemoveIndexInstance(long myIndexID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _indices.Remove(myIndexID);
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _vertexTypeManager = myMetaManager.VertexTypeManager;
            _vertexStore = myMetaManager.VertexStore;
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {

        }

        #endregion

        private static bool GetIsVersionedValue(IIndex<IComparable, Int64> index)
        {
            return index is IVersionedIndex<IComparable, Int64, Int64>;
        }

        private bool GetIsRangeValue(IIndex<IComparable, Int64> index)
        {
            return index is IRangeIndex<IComparable, Int64>;
        }

        private bool GetIsSingleValue(IIndex<IComparable, long> index)
        {
            return index is ISingleValueIndex<IComparable, Int64>;
        }


    }
}
