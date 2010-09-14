
#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// This is the base aggregate class. Each aggregate mus derive this class.
    /// </summary>
    public abstract class ABaseAggregate
    {

        public abstract String                FunctionName  { get; }

        #region (abstract) Methods

        public abstract Exceptional<IObject> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext myDBContext);

        public abstract Exceptional<IObject> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters);

        #endregion

    }

}
