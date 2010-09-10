/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/


#region Usings

using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDBInterface.TypeManagement;

using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;

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

        public override Exceptional<IObject> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            var aggregateResult = myTypeAttribute.GetADBBaseObjectType(myDBContext.DBTypeManager);
            foreach (var dbo in myDBObjects)
            {
                var attrResult = dbo.GetAttribute(myTypeAttribute, myTypeAttribute.GetDBType(myDBContext.DBTypeManager), myDBContext);
                if (attrResult.Failed())
                {
                    return attrResult;
                }
                var attr = attrResult.Value;

                if (attr != null && attr is ADBBaseObject && aggregateResult.IsValidValue((attr as ADBBaseObject).Value))
                {
                    aggregateResult.Add((attr as ADBBaseObject));
                }
                else if (attr != null) // if null, this value will be skipped
                {
                    return new Exceptional<IObject>(new Error_AggregateIsNotValidOnThisAttribute(myTypeAttribute.Name));
                }
            }
            return new Exceptional<IObject>(aggregateResult);
        }

        #endregion

        #region Index aggregate

        public override Exceptional<IObject> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext)
        {

            if (attributeIndex is UUIDIndex)
            {
                return new Exceptional<IObject>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Aggregating attribute UUID is not implemented!"));
            }
            else
            {
                var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

                // HACK: rewrite as soon as we have real attribute index keys
                if (attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.Count != 1)
                {
                    return new Exceptional<IObject>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                var typeAttr = graphDBType.GetTypeAttributeByUUID(attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.First());
                ADBBaseObject oneVal = typeAttr.GetADBBaseObjectType(dbContext.DBTypeManager);

                return new Exceptional<IObject>(attributeIndex.GetKeyValues(indexRelatedType, dbContext).AsParallel().Select(kv =>
                {
                    var mul = oneVal.Clone(kv.Key);
                    mul.Mul(oneVal.Clone(kv.Value.Count()));
                    return mul;

                }).Aggregate(oneVal.Clone(), (elem, result) => { result.Add(elem); return result; }));
            }

        }

        #endregion

    }

}
