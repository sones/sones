
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// The aggregate AVG
    /// </summary>
    public class AvgAggregate : ABaseAggregate
    {

        #region Properties

        public override string FunctionName
        {
            get { return "AVG"; }
        }

        #endregion

        #region Attribute aggregate

        public override Exceptional<IObject> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            var aggregateResult = new DBDouble(0d);
            var total = 0UL;

            foreach (var dbo in myDBObjects)
            {
                var attr = dbo.GetAttribute(myTypeAttribute, myTypeAttribute.GetDBType(myDBContext.DBTypeManager), myDBContext);
                if (attr.Failed())
                {
                    return attr;
                }
                if (attr.Value != null && attr.Value is ADBBaseObject && aggregateResult.IsValidValue((attr.Value as ADBBaseObject).Value))
                {
                    aggregateResult.Add((attr.Value as ADBBaseObject));
                    total++;
                }
                else
                {
                    return new Exceptional<IObject>(new Error_AggregateIsNotValidOnThisAttribute(myTypeAttribute.Name));
                }
            }
            aggregateResult.Div(new DBUInt64(total));

            return new Exceptional<IObject>(aggregateResult);
        }

        #endregion

        #region Index aggregate

        public override Exceptional<IObject> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext)
        {
            if (attributeIndex is UUIDIndex)
            {
                return new Exceptional<IObject>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "AVG(UUID) is not implemented!"));
            }

            var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

            // HACK: rewrite as soon as we have real attribute index keys
            ADBBaseObject aADBBaseObject = new DBDouble(DBObjectInitializeType.Default);
            DBUInt64 total = new DBUInt64((UInt64)0);

            foreach (var idxEntry in attributeIndex.GetKeyValues(indexRelatedType, dbContext))
            {
                aADBBaseObject.Add(aADBBaseObject.Clone(idxEntry.Key));

                total += (UInt64)idxEntry.Value.LongCount();
            }
            aADBBaseObject.Div(total);

            return new Exceptional<IObject>(aADBBaseObject);
        }

        #endregion


    }

}
