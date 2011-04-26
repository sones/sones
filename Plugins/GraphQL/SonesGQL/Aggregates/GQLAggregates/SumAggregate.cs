using System;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;
using sones.Library.Arithmetics;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Sum
    /// </summary>
    public sealed class SumAggregate : IGQLAggregate
    {
        #region constructor

        /// <summary>
        /// Creates a new sum aggregate
        /// </summary>
        public SumAggregate()
        {
 
        }

        #endregion

        #region IGQLAggregate Members

        /// <summary>
        /// Calculates the sum
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            var sumType = myPropertyDefinition.BaseType;
            IComparable sum = null;
                        
            foreach (var value in myValues)
            {
                if (sum == null)
                {
                    sum = ArithmeticOperations.Add(sumType, 0, value);
                }
                else
                {
                    sum = ArithmeticOperations.Add(sumType, sum, value);
                }
            }

            return new FuncParameter(sum, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "SUM"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            return new SumAggregate();
        }

        #endregion
    }
}
