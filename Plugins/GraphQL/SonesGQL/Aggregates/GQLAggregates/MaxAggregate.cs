using System;
using System.Collections.Generic;
using System.Linq;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Max
    /// </summary>
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

        /// <summary>
        /// Calculates the maximum of index attributes
        /// </summary>
        public FuncParameter Aggregate(IIndex<IComparable, long> myAttributeIndex, IVertexType myGraphDBType, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return new FuncParameter(myAttributeIndex.Keys().Max());
        }

        /// <summary>
        /// Calculates the maximum
        /// <seealso cref="IGQLAggregate"/>
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IVertex> myDBObjects, IAttributeDefinition myTypeAttribute, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, params ParameterValue[] myParameters)
        {
            IComparable aggregateResult = null;

            var foundFirstMax = false;

            #region is IPropertyDefinition
            if (myTypeAttribute is IPropertyDefinition)
            {
                foreach (var dbo in myDBObjects)
                {
                    if (dbo.HasProperty(myTypeAttribute.AttributeID))
                    {
                        var attrResult = dbo.GetProperty(myTypeAttribute.AttributeID);

                        if (foundFirstMax == false)
                        {
                            aggregateResult = attrResult;

                            foundFirstMax = true;
                        }
                        else
                        {
                            #region Compare current with max value

                            if (aggregateResult.CompareTo(attrResult) < 0)
                            {
                                aggregateResult = attrResult;
                            }

                            #endregion
                        }
                    }
                }
            }
            #endregion
            #region else
            else
            {
                return new FuncParameter(new AggregateException("Aggregate not valid on type " + myTypeAttribute.Name));
            }
            #endregion
            
            return new FuncParameter(aggregateResult);
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
