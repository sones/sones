#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// This is the abstract base class for all aggregates.
    /// </summary>
    public abstract class ABaseAggregate : IGraphDBAggregate
    {
        /// <summary>
        /// The aggregate name
        /// </summary>
        public abstract String                FunctionName  { get; }

        #region (abstract) Methods

        /// <summary>
        /// Abstract aggregate function for a attribute index
        /// </summary>
        /// <param name="myAttributeIndex">Attribute index</param>
        /// <param name="myGraphDBType">Underlying type of the index</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>The result of the aggregation</returns>
        public abstract Exceptional<FuncParameter> Aggregate(AAttributeIndex myAttributeIndex, GraphDBType myGraphDBType, DBContext myDBContext);

        /// <summary>
        /// Abstract aggregate for a list of dbobjects
        /// </summary>
        /// <param name="myDBObjects">List of dbobjects</param>
        /// <param name="myTypeAttribute">The attribute of the dbobject</param>
        /// <param name="myDBContext">The dbcontext</param>
        /// <param name="myParameters">Additional optional parameters for own designed aggregates</param>
        /// <returns>The result of the aggregation</returns>
        public abstract Exceptional<FuncParameter> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters);

        #endregion

    }

}
