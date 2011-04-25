using System;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;
using sones.GraphDB.TypeSystem;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;
using ISonesGQLFunction.Structure;

namespace sones.Plugins.SonesGQL.Aggregates
{

    #region IQLAggregateVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IQLAggregate plugin versions. 
    /// Defines the min and max version for all IQLAggregate implementations which will be activated used this IQLAggregate.
    /// </summary>
    public static class IGQLAggregateVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all GQL aggregates
    /// </summary>
    public interface IGQLAggregate : IPluginable
    {
        /// <summary>
        /// Abstract aggregate function for a attribute index
        /// </summary>
        /// <param name="myAttributeIndex">Attribute index</param>
        /// <param name="myGraphDBType">Underlying type of the index</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>The result of the aggregation</returns>
        FuncParameter Aggregate(
            IIndex<IComparable, Int64> myAttributeIndex,
            IVertexType myGraphDBType,
            IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        /// <summary>
        /// Abstract aggregate for a list of dbobjects
        /// </summary>
        /// <param name="myDBObjects">List of dbobjects</param>
        /// <param name="myTypeAttribute">The attribute of the dbobject</param>
        /// <param name="myDBContext">The dbcontext</param>
        /// <param name="myParameters">Additional optional parameters for own designed aggregates</param>
        /// <returns>The result of the aggregation</returns>
        FuncParameter Aggregate(
            IEnumerable<IVertex> myDBObjects, 
            IAttributeDefinition myTypeAttribute, 
            IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, 
            params ParameterValue[] myParameters);
    }
}