
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Session;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;


#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// The aggregate MAX
    /// </summary>
    public class MaxAggregate : ABaseAggregate
    {

        #region Properties

        /// <summary>
        /// Aggregate name
        /// <seealso cref=" ABaseAggregate"/>
        /// </summary>
        public override string FunctionName
        {
            get { return "MAX"; }
        }

        #endregion

        #region Attribute aggregate

        /// <summary>
        /// Calculates the maximum
        /// <seealso cref=" ABaseAggregate"/>
        /// </summary>
        public override Exceptional<FuncParameter> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            var foundFirstMax = false;
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
                    if (foundFirstMax == false)
                    {
                        aggregateResult.Value = (attr as ADBBaseObject).Value;
                        foundFirstMax = true;
                    }
                    else
                    {
                        if (aggregateResult.CompareTo((attr as ADBBaseObject).Value) < 0)
                        {
                            aggregateResult.Value = (attr as ADBBaseObject).Value;
                        }
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

        
        /// <summary>
        /// Calculates the maximum of index attributes
        /// </summary>
        public override Exceptional<FuncParameter> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext)
        {
            var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

            return new Exceptional<FuncParameter>(new FuncParameter(attributeIndex.GetKeys(indexRelatedType, dbContext).Max().IndexKeyValues[0]));
        }

        #endregion

    }

}
