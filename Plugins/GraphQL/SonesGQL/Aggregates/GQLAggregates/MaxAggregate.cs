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
    public sealed class MaxAggregate : IGQLAggregate, IPluginable
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
        /// Calculates the maximum
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            IComparable max = null;

            foreach (var value in myValues)
            {
                if (max == null)
                {
                    max = value;
                }
                else if (max.CompareTo(value) < 0)
                {
                    max = value;
                }
            }

            return new FuncParameter(max, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.max"; }
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

        #region IGQLAggregate Members


        public string AggregateName
        {
            get { return "max"; }
        }

        #endregion
    }
}
