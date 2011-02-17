using System;

namespace sones.Plugins.SonesGQL
{

    #region IGraphDBAggregateVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDBAggregate plugin versions. 
    /// Defines the min and max version for all IGraphDBAggregate implementations which will be activated used this IGraphDBAggregate.
    /// </summary>
    public static class IGQLAggregateVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("1.0.0.1"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("1.0.0.1"); }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all GQL aggregates
    /// </summary>
    public interface IGQLAggregate
    {
        /// <summary>
        /// Returns the name of the aggregate function
        /// </summary>
        String Name { get; }

        //Todo: reactivate functions

        ///// <summary>
        ///// Abstract aggregate function for a attribute index
        ///// </summary>
        ///// <param name="myAttributeIndex">Attribute index</param>
        ///// <param name="myGraphDBType">Underlying type of the index</param>
        ///// <param name="myDBContext">The db context</param>
        ///// <returns>The result of the aggregation</returns>
        //FuncParameter Aggregate(AAttributeIndex myAttributeIndex, GraphDBType myGraphDBType, DBContext myDBContext);

        ///// <summary>
        ///// Abstract aggregate for a list of dbobjects
        ///// </summary>
        ///// <param name="myDBObjects">List of dbobjects</param>
        ///// <param name="myTypeAttribute">The attribute of the dbobject</param>
        ///// <param name="myDBContext">The dbcontext</param>
        ///// <param name="myParameters">Additional optional parameters for own designed aggregates</param>
        ///// <returns>The result of the aggregation</returns>
        //FuncParameter Aggregate(IEnumerable<IVertex> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters);
    }
}