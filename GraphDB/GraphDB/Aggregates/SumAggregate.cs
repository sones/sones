
#region Usings

using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// The aggregate SUM
    /// </summary>
    public class SumAggregate : ABaseAggregate
    {

        #region Properties

        public override string FunctionName
        {
            get { return "SUM"; }
        }

        #endregion

        #region Attribute aggregate

        public override Exceptional<FuncParameter> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            var aggregateResult = myTypeAttribute.GetADBBaseObjectType(myDBContext.DBTypeManager);
            foreach (var dbo in myDBObjects)
            {
                var attrResult = dbo.GetAttribute(myTypeAttribute, myTypeAttribute.GetDBType(myDBContext.DBTypeManager), myDBContext);
                if (attrResult.Failed())
                {
                    return new Exceptional<FuncParameter>(attrResult);
                }
                var attr = attrResult.Value;

                if (attr != null && attr is ADBBaseObject && aggregateResult.IsValidValue((attr as ADBBaseObject).Value))
                {
                    aggregateResult.Add((attr as ADBBaseObject));
                }
                else if (attr != null) // if null, this value will be skipped
                {
                    return new Exceptional<FuncParameter>(new Error_AggregateIsNotValidOnThisAttribute(myTypeAttribute.Name));
                }
            }
            return new Exceptional<FuncParameter>(new FuncParameter(aggregateResult));
        }

        #endregion

        #region Index aggregate

        public override Exceptional<FuncParameter> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext)
        {

            if (attributeIndex.IsUUIDIndex)
            {
                return new Exceptional<FuncParameter>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Aggregating attribute UUID is not implemented!"));
            }
            else
            {
                var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

                // HACK: rewrite as soon as we have real attribute index keys
                if (attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.Count != 1)
                {
                    return new Exceptional<FuncParameter>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                var typeAttr = graphDBType.GetTypeAttributeByUUID(attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.First());
                ADBBaseObject oneVal = typeAttr.GetADBBaseObjectType(dbContext.DBTypeManager);

                return new Exceptional<FuncParameter>(new FuncParameter(attributeIndex.GetKeyValues(indexRelatedType, dbContext).AsParallel().Select(kv =>
                {
                    var mul = oneVal.Clone(kv.Key);
                    mul.Mul(oneVal.Clone(kv.Value.Count()));
                    return mul;

                }).Aggregate(oneVal.Clone(), (elem, result) => { result.Add(elem); return result; })));
            }

        }

        #endregion

    }

}
