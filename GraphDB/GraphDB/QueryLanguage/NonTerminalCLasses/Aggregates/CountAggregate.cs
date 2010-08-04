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


/* <id Name=”sones GraphDB – CountAggregate” />
 * <copyright file=”CountAggregate.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The aggregate COUNT.<summary>
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.Indices;
using sones.Lib;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates
{
    public class CountAggregate : ABaseAggregate
    {

        public override string FunctionName
        {
            get { return "COUNT"; }
        }

        public override AggregateType AggregateType
        {
            get { return AggregateType.COUNT; }
        }

        public override TypesOfOperatorResult TypeOfResult
        {
            get { return TypesOfOperatorResult.UInt64; }
        }

        public override Exceptional<Object> Aggregate(IEnumerable<DBObjectReadout> myDBObjectReadouts, TypeAttribute myTypeAttribute, DBContext myDBContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            DBUInt64 myUInt64 = new DBUInt64((UInt64)myDBObjectReadouts.Count());
            return new Exceptional<Object>(myUInt64.Value);
        }

        public override Exceptional<Object> Aggregate(IEnumerable<ObjectUUID> myObjectUUIDs, TypeAttribute myTypeAttribute, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            DBUInt64 myUInt64 = new DBUInt64((UInt64)myObjectUUIDs.Count());
            return new Exceptional<Object>(myUInt64.Value);
        }

        public override Exceptional<Object> Aggregate(IEnumerable<Exceptional<ObjectUUID>> myObjectStreams, TypeAttribute myTypeAttribute, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            DBUInt64 myUInt64 = new DBUInt64(myObjectStreams.ULongCount());
            return new Exceptional<Object>(myUInt64.Value);
        }

        public override Exceptional<Object> Aggregate(AListEdgeType myAListEdgeType, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            UInt64 count = myAListEdgeType.Count();
            return new Exceptional<Object>(count);
        }


        public override Exceptional<object> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            if (graphDBType.IsAbstract)
            {
                #region For abstract types, count all attribute idx of the subtypes

                UInt64 count = 0;

                foreach (var aSubType in dbContext.DBTypeManager.GetAllSubtypes(graphDBType, false))
                {
                    if (!aSubType.IsAbstract)
                    {
                        count += aSubType.GetUUIDIndex(dbContext.DBTypeManager).GetValueCount(aSubType, dbContext);
                    }
                }

                return new Exceptional<object>(count);
                
                #endregion
            }
            else
            {
                #region Return the count of idx values

                var indexRelatedType = dbContext.DBTypeManager.GetTypeByUUID(attributeIndex.IndexRelatedTypeUUID);

                return new Exceptional<object>(attributeIndex.GetValueCount(indexRelatedType, dbContext));
                
                #endregion
            }
        }
    }
}
