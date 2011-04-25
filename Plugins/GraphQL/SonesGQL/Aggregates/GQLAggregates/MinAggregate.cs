using sones.Plugins.Index.Interfaces;
using System;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Aggregates
{
    public sealed class MinAggregate : IGQLAggregate
    {
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
            get { throw new NotImplementedException(); }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { throw new NotImplementedException(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}