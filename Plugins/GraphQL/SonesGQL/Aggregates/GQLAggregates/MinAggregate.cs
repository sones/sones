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
using sones.GraphDB.Request;
using System.Linq;

namespace sones.Plugins.SonesGQL.Aggregates
{
    public sealed class MinAggregate : IGQLAggregate
    {
        #region constructor

        /// <summary>
        /// Creates a new min aggregate
        /// </summary>
        public MinAggregate()
        {

        }

        #endregion

        #region IGQLAggregate Members

        public FuncParameter Aggregate(IIndex<IComparable, long> myAttributeIndex, IVertexType myGraphDBType, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return new FuncParameter(myAttributeIndex.Keys().Min());
        }

        public FuncParameter Aggregate(IEnumerable<IVertex> myDBObjects, IAttributeDefinition myTypeAttribute, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params ParameterValue[] myParameters)
        {
            IComparable aggregateResult = null;

            var foundFirstMin = false;

            if (myTypeAttribute is IPropertyDefinition)
            {
                foreach (var dbo in myDBObjects)
                {
                    if (dbo.HasProperty(myTypeAttribute.AttributeID))
                    {
                        var attrResult = dbo.GetProperty(myTypeAttribute.AttributeID);

                        if (foundFirstMin == false)
                        {
                            aggregateResult = attrResult;

                            foundFirstMin = true;
                        }
                        else
                        {
                            #region Compare current with min value

                            if (aggregateResult.CompareTo(attrResult) > 0)
                            {
                                aggregateResult = attrResult;
                            }

                            #endregion
                        }
                    }
                }
            }
            else
            {
                return new FuncParameter(new AggregateException("Aggregate not valid on type " + myTypeAttribute.Name));
            }

            return new FuncParameter(aggregateResult);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "MIN"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new MinAggregate();
        }

        #endregion
    }
}