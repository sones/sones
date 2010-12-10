
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;

using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;


#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// The aggregate Min
    /// </summary>
    public class MinAggregate : ABaseAggregate
    {

        #region Properties

        public override string FunctionName
        {
            get { return "MIN"; }
        }

        #endregion

        #region Attribute aggregate

        public override Exceptional<FuncParameter> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            var aggregateResult = myTypeAttribute.GetADBBaseObjectType(myDBContext.DBTypeManager);
            var foundFirstMin = false;
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
                    if (foundFirstMin == false)
                    {
                        aggregateResult.Value = (attr as ADBBaseObject).Value;
                        foundFirstMin = true;
                    }
                    else
                    {

                        #region Compare current with min value

                        if (aggregateResult.CompareTo((attr as ADBBaseObject).Value) > 0)
                        {
                            aggregateResult.Value = (attr as ADBBaseObject).Value;
                        }

                        #endregion

                    }
                }
                else
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
            var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

            return new Exceptional<FuncParameter>(new FuncParameter(attributeIndex.GetKeys(indexRelatedType, dbContext).Min().IndexKeyValues[0]));
        }
        
        #endregion

    }

}
