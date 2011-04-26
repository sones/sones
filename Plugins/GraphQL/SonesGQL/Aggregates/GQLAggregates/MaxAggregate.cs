using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISonesGQLFunction.Structure;
using sones.Plugins.Index.Interfaces;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Aggregates
{
    public sealed class MaxAggregate : IGQLAggregate
    {
        #region constructor

        /// <summary>
        /// Creates a new max aggregate
        /// </summary>
        public MaxAggregate()
        {

        }

        #endregion

        #region IGQLAggregate Members

        public FuncParameter Aggregate(IIndex<IComparable, long> myAttributeIndex, IVertexType myGraphDBType, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public FuncParameter Aggregate(IEnumerable<IVertex> myDBObjects, IAttributeDefinition myTypeAttribute, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params ParameterValue[] myParameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "MAX"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new MaxAggregate();
        }

        #endregion
    }
}
