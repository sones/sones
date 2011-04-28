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
    /// The aggregate Min
    /// </summary>
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

        /// <summary>
        /// Calculates the minimum
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            IComparable min = null;

            foreach (var value in myValues)
            {
                if (min == null)
                {
                    min = value;
                }
                else if (min.CompareTo(value) > 0)
                {
                    min = value;
                }
            }

            return new FuncParameter(min, myPropertyDefinition);
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.min"; }
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