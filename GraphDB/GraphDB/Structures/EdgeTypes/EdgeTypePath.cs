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

/* <id name="EdgeTypePath" />
 * <copyright file="AListReferenceEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Special edge to store paths and create the readout of them.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.QueryLanguage;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// Sepecial edge to store paths and create the readout of them
    /// </summary>
    public class EdgeTypePath : ASetReferenceEdgeType
    {

        #region TypeCode
        public override UInt32 TypeCode { get { return 454; } }
        #endregion

        #region Data and Ctors

        private IEnumerable<List<ObjectUUID>> _Paths;
        private TypeAttribute _pathAttribute;
        private GraphDBType _typeOfObjects;

        public EdgeTypePath() { }

        public EdgeTypePath(IEnumerable<List<ObjectUUID>> myPaths, TypeAttribute myTypeAttribute, GraphDBType myTypeOfObjects)
        {
            _Paths = myPaths;
            _pathAttribute = myTypeAttribute;
            _typeOfObjects = myTypeOfObjects;
        }
        
        #endregion

        #region AEdgeType Members

        public override String EdgeTypeName { get { return "PATH"; } }

        public override EdgeTypeUUID EdgeTypeUUID { get { return new EdgeTypeUUID("1002"); } }

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {
            throw new NotImplementedException();
        }

        public override AEdgeType GetNewInstance()
        {
            throw new NotImplementedException();
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable)
        {
            throw new NotImplementedException();
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable)
        {
            throw new NotImplementedException();
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return String.Concat(EdgeTypeName.ToUpper(), GraphQL.TERMINAL_LT, myGraphDBType.Name, GraphQL.TERMINAL_GT);
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }

        #endregion

        public override IEnumerable<ObjectUUID> GetAllUUIDs()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetEdges()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            var result = new List<DBObjectReadout>();

            foreach (var path in _Paths)
            {
                var keyVal = new Dictionary<String, Object>();

                result.Add(resolvePath(null, path.Skip(1), GetAllAttributesFromDBO));
            }
            return result;
        }

        private DBObjectReadout resolvePath(DBObjectReadout myReadoutPath, IEnumerable<ObjectUUID> myPathEntries, Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            var dbReadout = GetAllAttributesFromDBO(myPathEntries.First());

            if (myPathEntries.Count() > 1)
            {

                var res = resolvePath(myReadoutPath, myPathEntries.Skip(1), GetAllAttributesFromDBO);
                if (res.Attributes == null)
                    return null;

                if (dbReadout.Attributes.ContainsKey(_pathAttribute.Name))
                    if ((dbReadout.Attributes[_pathAttribute.Name] as List<DBObjectReadout>) == null)
                        dbReadout.Attributes[_pathAttribute.Name] = new Edge( new List<DBObjectReadout>() { res }, _typeOfObjects.Name );
                    else
                        ((List<DBObjectReadout>)(dbReadout.Attributes[_pathAttribute.Name] as Edge)).Add(res);
                else
                    dbReadout.Attributes.Add(_pathAttribute.Name, new Edge(new List<DBObjectReadout>() { res }, _typeOfObjects.Name));

            }
            else
            {
                dbReadout.Attributes.Remove(_pathAttribute.Name);
            }

            return dbReadout;
        }

        public override IEnumerable<DBObjectReadout> GetReadouts(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO, IEnumerable<Exceptional<DBObjectStream>> myDBObjectStreams)
        {
            var result = new List<DBObjectReadout>();

            foreach (var path in _Paths)
            {
                var keyVal = new Dictionary<String, Object>();

                result.Add(resolvePath(null, path, GetAllAttributesFromDBO));
            }
            return result;
        }

        public override void RemoveWhere(Predicate<ObjectUUID> match)
        {
            throw new NotImplementedException();
        }

        public override ObjectUUID FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public override void AddRange(IEnumerable<ObjectUUID> hashSet, params ADBBaseObject[] myParameters)
        {
            throw new NotImplementedException();
        }

        public override void Add(ObjectUUID myValue, params ADBBaseObject[] myParameters)
        {
            throw new NotImplementedException();
        }

        public override void Add(IEnumerable<ObjectUUID> myValue, params ADBBaseObject[] myParameters)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveUUID(ObjectUUID myValue)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(ObjectUUID myValue)
        {
            throw new NotImplementedException();
        }

        public override ulong Count()
        {
            throw new NotImplementedException();
        }

        public override System.Collections.IEnumerable GetAll()
        {
            throw new NotImplementedException();
        }

        public override System.Collections.IEnumerable GetTop(ulong myNumOfEntries)
        {
            throw new NotImplementedException();
        }

        public override void UnionWith(AEdgeType myAListEdgeType)
        {
            throw new NotImplementedException();
        }

        public override void Distinction()
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
            throw new NotImplementedException();
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            throw new NotImplementedException();
        }

        #region Equals

        public override bool Equals(object obj)
        {
            var otherEdge = obj as EdgeTypePath;
            if (otherEdge == null)
            {
                return false;
            }

            if (EdgeTypeName != otherEdge.EdgeTypeName)
            {
                return false;
            }
            if (EdgeTypeUUID != otherEdge.EdgeTypeUUID)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
