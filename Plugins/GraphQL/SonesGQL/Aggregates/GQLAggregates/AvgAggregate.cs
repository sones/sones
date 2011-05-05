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
using sones.Library.Arithmetics;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Avg
    /// </summary>
    public sealed class AvgAggregate : IGQLAggregate, IPluginable
    {
        #region constructor

        /// <summary>
        /// creates a new avg aggregate
        /// </summary>
        public AvgAggregate()
        {
 
        }

        #endregion

        #region describe
        /// <summary>
        /// Returns the description of the aggregate.
        /// </summary>
        public string GetDescribeOutput()
        {
            return "This aggregate will calculate the average of the given operands. This aggregate is type dependent and will only operate on numbers.";
        } 
        #endregion

        #region IGQLAggregate Members

        /// <summary>
        /// Calculates the average
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            var divType = myPropertyDefinition.BaseType;
            IComparable sum = null;
            uint total = 0;

            foreach (var value in myValues)
            {
                if (sum == null)
                {
                    sum = ArithmeticOperations.Add(divType, 0, value);
                }
                else
                {
                    sum = ArithmeticOperations.Add(divType, sum, value);
                }

                total++;
            }

            var aggregateResult = ArithmeticOperations.Div(divType, sum, total);

            return new FuncParameter(aggregateResult, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.avg"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new AvgAggregate();
        }

        #endregion

        #region IGQLAggregate Members

        public string AggregateName
        {
            get { return "avg"; }
        }

        #endregion
    }
}
