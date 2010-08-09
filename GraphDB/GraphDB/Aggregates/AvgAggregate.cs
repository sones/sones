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


/* <id Name=”sones GraphDB – AvgAggregate” />
 * <copyright file=”AvgAggregate.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The aggregate AVG.<summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

using sones.Lib.Session;

namespace sones.GraphDB.Aggregates
{
    public class AvgAggregate : ABaseAggregate
    {

        public override string FunctionName
        {
            get { return "AVG"; }
        }

        public override AggregateType AggregateType
        {
            get { return AggregateType.AVG; }
        }

        public override TypesOfOperatorResult TypeOfResult
        {
            get { return TypesOfOperatorResult.Double; }
        }


        public override Exceptional<Object> Aggregate(IEnumerable<DBObjectReadout> myDBObjectReadouts, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            ADBBaseObject pandoraObject = new DBDouble(DBObjectInitializeType.Default);
            DBUInt64 total = new DBUInt64((UInt64)0);
            foreach (DBObjectReadout dbo in myDBObjectReadouts)
            {
                if (HasAttribute(dbo.Attributes, myTypeAttribute.Name, myTypeManager))
                {
                    var attrVal = GetAttribute(dbo.Attributes, myTypeAttribute.Name, myTypeManager);
                    if (pandoraObject.IsValidValue(attrVal))
                    {
                        pandoraObject.Add(pandoraObject.Clone(attrVal));
                        total += 1;
                    }
                    else
                    {
                        return new Exceptional<object>(new Error_DataTypeDoesNotMatch(pandoraObject.ObjectName, attrVal.GetType().Name));
                    }
                }
            }
            pandoraObject.Div(total);
            return new Exceptional<object>(pandoraObject.Value);
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
            if (attributeIndex is UUIDIndex)
            {
                return new Exceptional<object>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "AVG(UUID) is not implemented!"));     
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

            return new Exceptional<object>(aADBBaseObject.Value);
        }
    }
}
