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
using sones.GraphDBInterface.TypeManagement;


#endregion

namespace sones.GraphDB.Aggregates
{

    /// <summary>
    /// The aggregate MAX
    /// </summary>
    public class MaxAggregate : ABaseAggregate
    {

        #region Properties

        public override string FunctionName
        {
            get { return "MAX"; }
        }

        #endregion

        #region Attribute aggregate

        public override Exceptional<IObject> Aggregate(IEnumerable<DBObjectStream> myDBObjects, TypeAttribute myTypeAttribute, DBContext myDBContext, params Functions.ParameterValue[] myParameters)
        {
            var foundFirstMax = false;
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
                    return new Exceptional<IObject>(new Error_AggregateIsNotValidOnThisAttribute(myTypeAttribute.Name));
                }
            }
            return new Exceptional<IObject>(aggregateResult);
        }

        #endregion

        #region Index aggregate

        public override Exceptional<IObject> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext)
        {
            var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

            return new Exceptional<IObject>(attributeIndex.GetKeys(indexRelatedType, dbContext).Max().IndexKeyValues[0]);
        }

        #endregion

    }

}
