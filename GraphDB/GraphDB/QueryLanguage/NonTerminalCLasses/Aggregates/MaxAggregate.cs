/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id Name=”sones GraphDB – MaxAggregate” />
 * <copyright file=”MaxAggregate.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The aggregate MAX.<summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

using sones.Lib.Session;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates
{
    public class MaxAggregate : ABaseAggregate
    {

        public override string FunctionName
        {
            get { return "MAX"; }
        }

        public override AggregateType AggregateType
        {
            get { return AggregateType.MAX; }
        }
        
        public override TypesOfOperatorResult TypeOfResult
        {
            get { return TypesOfOperatorResult.Double; }
        }

        public override Exceptional<Object> Aggregate(IEnumerable<DBObjectReadout> myDBObjectReadouts, TypeAttribute myTypeAttribute, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            ADBBaseObject maxValue = myTypeAttribute.GetADBBaseObjectType(dbContext.DBTypeManager);
            maxValue.SetValue(DBObjectInitializeType.MinValue);

            foreach (DBObjectReadout dbo in myDBObjectReadouts)
            {
                if (HasAttribute(dbo.Attributes, myTypeAttribute.Name, dbContext))
                {
                    ADBBaseObject curVal = maxValue.Clone(GetAttribute(dbo.Attributes, myTypeAttribute.Name, dbContext));
                    if (maxValue.CompareTo(curVal) < 0)
                        maxValue.Value = curVal.Value;
                }
            }
            return new Exceptional<object>(maxValue.Value);
        }

        public override Exceptional<Object> Aggregate(IEnumerable<ObjectUUID> myObjectUUIDs, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            return new Exceptional<object>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<Object> Aggregate(AListEdgeType myAListEdgeType, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            return new Exceptional<object>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<Object> Aggregate(IEnumerable<Exceptional<ObjectUUID>> myObjectStreams, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            return new Exceptional<object>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<object> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

            return new Exceptional<Object>((ObjectUUID)attributeIndex.GetKeys(indexRelatedType, dbContext).Max().IndexKeyValues[0].Value);
        }
    }
}
