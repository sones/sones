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


/* <id Name=”sones GraphDB – SumAggregate” />
 * <copyright file=”SumAggregate.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The aggregate SUM.<summary>
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
    public class SumAggregate : ABaseAggregate
    {

        public override string FunctionName
        {
            get { return "SUM"; }
        }

        public override AggregateType AggregateType
        {
            get { return AggregateType.SUM; }
        }
        
        public override TypesOfOperatorResult TypeOfResult
        {
            get { return TypesOfOperatorResult.Double; }
        }

        public override Exceptional<Object> Aggregate(IEnumerable<DBObjectReadout> myDBObjectReadouts, TypeAttribute myTypeAttribute, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            ADBBaseObject pandoraObject = myTypeAttribute.GetADBBaseObjectType(dbContext.DBTypeManager);
            foreach (DBObjectReadout dbo in myDBObjectReadouts)
            {
                if (HasAttribute(dbo.Attributes, myTypeAttribute.Name, dbContext))
                {
                    pandoraObject.Add(pandoraObject.Clone(GetAttribute(dbo.Attributes, myTypeAttribute.Name, dbContext)));
                }
            }
            return new Exceptional<Object>(pandoraObject.Value);
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

        public override Exceptional<object> Aggregate(AttributeIndex attributeIndex, GraphDBType graphDBType, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {

            if (attributeIndex.IsUuidIndex)
            {

                //String settingEncoding = (String)graphDBType.GetSettingValue(DBConstants.SettingUUIDEncoding, mySessionToken, dbContext.DBTypeManager).Value.Value;
                //if (settingEncoding == null)
                //    return new Exceptional<object>(new Error_ArgumentNullOrEmpty("settingEncoding"));
                
                var idxRef = attributeIndex.GetIndexReference(dbContext.DBIndexManager);
                if (!idxRef.Success)
                {
                    return new Exceptional<object>(idxRef);
                }

                throw new NotImplementedException("Aggregating attribute UUID is not implemented!");
                //return new Exceptional<Object>((from val in idxRef.Value.Values().AsParallel() select Convert.ToInt64(SpecialTypeAttribute_UUID.ConvertFromUUID(val.First(), settingEncoding.ToLower()))).Sum());

            }

            else
            {
                // HACK: rewrite as soon as we have real attribute index keys
                if (attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.Count != 1)
                    return new Exceptional<object>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                var typeAttr = graphDBType.GetTypeAttributeByUUID(attributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.First());
                ADBBaseObject oneVal = typeAttr.GetADBBaseObjectType(dbContext.DBTypeManager);

                //return (from val in attributeIndex.IndexReference.GetIDictionary().AsParallel() select oneVal.Clone(val.Key).Mul(oneVal.Clone(val.Value.Count)));
                var idxRef = attributeIndex.GetIndexReference(dbContext.DBIndexManager);
                if (!idxRef.Success)
                {
                    return new Exceptional<object>(idxRef);
                }

                return new Exceptional<Object>(idxRef.Value.GetIDictionary().AsParallel().Select(kv =>
                {
                    var mul = oneVal.Clone(kv.Key);
                    mul.Mul(oneVal.Clone(kv.Value.Count));
                    return mul;

                }).Aggregate(oneVal.Clone(), (elem, result) => { result.Add(elem); return result; }).Value);
                
            }
               
        }
    }
}
