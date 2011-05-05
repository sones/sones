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
using sones.Plugins.SonesGQL.Aggregates.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Sum
    /// </summary>
    public sealed class SumAggregate : IGQLAggregate, IPluginable
    {
        #region constructor

        /// <summary>
        /// Creates a new sum aggregate
        /// </summary>
        public SumAggregate()
        {
 
        }

        #endregion

        #region describe
        /// <summary>
        /// Returns the description of the aggregate.
        /// </summary>
        public string GetDescribeOutput()
        {
            return "This aggregate will calculate the sum of the given operands. This aggregate is type dependent and will only operate on numbers.";
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

            try
            {
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
            }
            catch (ArithmeticException)
            {
                throw new InvalidArithmeticAggregationException(sumType, this.AggregateName);
            }

            return new FuncParameter(sum, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.sum"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new SumAggregate();
        }

        #endregion

        #region IGQLAggregate Members


        public string AggregateName
        {
            get { return "sum"; }
        }

        #endregion
    }
}
