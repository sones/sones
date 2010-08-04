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


/* <id Name=”sones GraphDB – ABaseAggregate” />
 * <copyright file=”ABaseAggregate.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This is the base aggregate class. Each aggregate mus derive this class.<summary>
 */

using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

using sones.Lib.Session;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates
{
    public abstract class ABaseAggregate
    {

        public abstract String FunctionName { get; }
        public abstract AggregateType AggregateType { get; }
        public abstract TypesOfOperatorResult TypeOfResult { get; }
    
        #region (public) Methods

        /// <summary>
        /// Aggregates the final ObjectReadouts after an select - FROM User U SELECT COUNT(U.Salary).
        /// </summary>
        /// <param name="myDBObjectReadouts">A set of DBObjectReadout or any derived types</param>
        /// <param name="myEdgeKeysToAttributes">The edge</param>
        /// <param name="myTypeManager">TypeManager reference</param>
        /// <returns>The Value of the PandoraResult contains the plain result of the aggregate</returns>
        public abstract Exceptional<Object> Aggregate(IEnumerable<DBObjectReadout> myDBObjectReadouts, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken);

        /// <summary>
        /// Aggregate on a set of ObjectUUIDs - e.g. the objectsUUIDs of an backwardEdge
        /// </summary>
        /// <param name="myObjectUUIDs">A set of ObjectUUIDs</param>
        /// <param name="myTypeAttribute">The TypeAttribute</param>
        /// <param name="myTypeManager">TypeManager reference</param>
        /// <returns>The Value of the PandoraResult contains the plain result of the aggregate</returns>
        public abstract Exceptional<Object> Aggregate(IEnumerable<ObjectUUID> myObjectUUIDs, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken);

        /// <summary>
        /// An Aggregate on a ListEdgeType attribute like U.Friends
        /// </summary>
        /// <param name="myAListEdgeType">A ListEdgeType</param>
        /// <param name="myTypeAttribute">The TypeAttribute</param>
        /// <param name="myTypeManager">TypeManager reference</param>
        /// <returns>The Value of the PandoraResult contains the plain result of the aggregate</returns>
        public abstract Exceptional<Object> Aggregate(AListEdgeType myAListEdgeType, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken);

        /// <summary>
        /// Aggregate on a set of ObejctStreams
        /// </summary>
        /// <param name="myObjectStreams">A set of ObjectStreams</param>
        /// <param name="myTypeAttribute">The TypeAttribute</param>
        /// <param name="myTypeManager">TypeManager reference</param>
        /// <returns>The Value of the PandoraResult contains the plain result of the aggregate</returns>
        public abstract Exceptional<Object> Aggregate(IEnumerable<Exceptional<ObjectUUID>> myObjectStreams, TypeAttribute myTypeAttribute, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken);

        public abstract Exceptional<Object> Aggregate(AAttributeIndex attributeIndex, GraphDBType graphDBType, DBContext myTypeManager, DBObjectCache myDBObjectCache, SessionSettings mySessionToken);

        #endregion

        protected bool HasAttribute(IDictionary<string, object> dboAttr, string attributeName, DBContext myTypeManager)
        {
            return dboAttr.ContainsKey(attributeName);
        }

        protected object GetAttribute(IDictionary<string, object> dboAttr, string attributeName, DBContext myTypeManager)
        {
            return dboAttr[attributeName];
        }

        public Exceptional<Object> Aggregate(Object myDBObject, TypeAttribute myTypeAttribute, DBContext dbContext, DBObjectCache myDBObjectCache, SessionSettings mySessionToken)
        {
            if (myDBObject is IEnumerable<DBObjectReadout>)
            {
                return Aggregate((IEnumerable<DBObjectReadout>)myDBObject, myTypeAttribute, dbContext, myDBObjectCache, mySessionToken);
            }
            // from backward edges
            else if (myDBObject is IEnumerable<ObjectUUID>)
            {
                return Aggregate((IEnumerable<ObjectUUID>)myDBObject, myTypeAttribute, dbContext, myDBObjectCache, mySessionToken);
            }
            else if (myDBObject is IEnumerable<Exceptional<ObjectUUID>>) // from Expression graph
            {
                return Aggregate((IEnumerable<Exceptional<ObjectUUID>>)myDBObject, myTypeAttribute, dbContext, myDBObjectCache, mySessionToken);
            }
            else if (myDBObject is AListEdgeType)
            {
                return Aggregate((AListEdgeType)myDBObject, myTypeAttribute, dbContext, myDBObjectCache, mySessionToken);
            }
            else if (myDBObject is AttributeIndex)
            {
                return Aggregate((AttributeIndex)myDBObject, myTypeAttribute, dbContext, myDBObjectCache, mySessionToken);
            }
            else if (myDBObject is DBObjectStream)
            {
                if (((DBObjectStream)myDBObject).HasAttribute(myTypeAttribute.UUID, myTypeAttribute.GetDBType(dbContext.DBTypeManager)))
                    return Aggregate(((DBObjectStream)myDBObject).GetAttribute(myTypeAttribute.UUID), myTypeAttribute, dbContext, myDBObjectCache, mySessionToken);
                else
                    return new Exceptional<object>((object)null);
            }
            else
            {
                return new Exceptional<object>(new Error_NotImplementedAggregateTarget(myDBObject.GetType()));
            }
        }
    }
}
