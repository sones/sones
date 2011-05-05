using System;
using System.Collections.Generic;
using System.Linq;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The Aggregate Count
    /// </summary>
    public sealed class CountAggregate :IGQLAggregate, IPluginable
    {
        #region constructor

        /// <summary>
        /// Creates a new count aggregate
        /// </summary>
        public CountAggregate()
        {
 
        }

        #endregion

        #region describe
        /// <summary>
        /// Returns the description of the aggregate.
        /// </summary>
        public string GetDescribeOutput()
        {
            return "This aggregate will operate a count on a MultiEdge / HyperEdge. This aggregate is type dependent and will only operate on Multi- / HyperEdges.";
        }
        #endregion

        #region IGQLAggregate Members

        /// <summary>
        /// Calculates the count
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            return new FuncParameter(myValues.Count(), myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.count"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string,Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new CountAggregate();
        }

        #endregion

        #region IGQLAggregate Members


        public string AggregateName
        {
            get { return "count"; }
        }

        #endregion
    }
}
