
#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// The aggregate COUNT
    /// </summary>
    public class CountAggregate : ABaseAggregate
    {

        #region Properties

        /// <summary>
        /// Aggregate name
        /// <seealso cref=" ABaseAggregate"/>
        /// </summary>
        public override string FunctionName
        {
            get { return "COUNT"; }
        }

        #endregion

        #region Attribute aggregation

        /// <summary>
        /// Count the elements
        /// <seealso cref=" ABaseAggregate"/>
        /// </summary>
        public override Exceptional<FuncParameter> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            return new Exceptional<FuncParameter>(new FuncParameter(new DBUInt64(myDBObjects.ULongCount())));
        }

        #endregion

        #region Index aggregation

        /// <summary>
        /// Count the index elements
        /// <seealso cref=" ABaseAggregate"/>
        /// </summary>
        public override Exceptional<FuncParameter> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext)
        {
            //if (graphDBType.IsAbstract)
            //{
            //    #region For abstract types, count all attribute idx of the subtypes

            //    UInt64 count = 0;

            //    foreach (var aSubType in dbContext.DBTypeManager.GetAllSubtypes(graphDBType, false))
            //    {
            //        if (!aSubType.IsAbstract)
            //        {
            //            count += aSubType.GetUUIDIndex(dbContext.DBTypeManager).GetValueCount();
            //        }
            //    }

            //    return new Exceptional<IObject>(new DBUInt64(count));

            //    #endregion
            //}
            //else
            //{
            #region Return the count of idx values

            var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);
            var count = attributeIndex.GetValueCount(dbContext, indexRelatedType);


            foreach (var type in dbContext.DBTypeManager.GetAllSubtypes(indexRelatedType, false))
            {
                var idx = type.GetAttributeIndex(dbContext, attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs, attributeIndex.IndexEdition);
                if (idx.Success())
                {
                    count += idx.Value.GetValueCount(dbContext, type);
                }
            }

            return new Exceptional<FuncParameter>(new FuncParameter(new DBUInt64(count)));

            #endregion
            //}
        }

        #endregion


    }

}
