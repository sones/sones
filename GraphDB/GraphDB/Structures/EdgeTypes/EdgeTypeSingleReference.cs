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

/* <id name="sones GraphDB – EdgeTypeSingle" />
 * <copyright file="EdgeTypeSingle.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Holds a single ObjectUUID.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.Lib.Serializer;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.NewFastSerializer;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.Lib;
using System.Diagnostics;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// Holds a single ObjectUUID
    /// </summary>
    
    public class EdgeTypeSingleReference : ASingleReferenceEdgeType
    {
        private Tuple<ObjectUUID, Reference> _ObjectUUID;
        private TypeUUID _typeOfDBObjects;
        
        #region TypeCode
        public override UInt32 TypeCode { get { return 455; } }
        #endregion

        public EdgeTypeSingleReference()
        {
            _ObjectUUID = null;
        }

        public EdgeTypeSingleReference(ObjectUUID dbos, TypeUUID myTypeOfDBObject)
        {
            Debug.Assert(myTypeOfDBObject != null);
            if (dbos != null)
            {
                _ObjectUUID = new Tuple<ObjectUUID, Reference>(dbos, new Reference(dbos, myTypeOfDBObject));
            }
            _typeOfDBObjects = myTypeOfDBObject;
        }


        #region AEdgeType Members

        public override string EdgeTypeName
        {
            get { return "SINGLE"; }
        }

        public override EdgeTypeUUID EdgeTypeUUID
        {
            get { return new EdgeTypeUUID(10); }
        }

        public override void ApplyParams(params EdgeTypeParamNode[] myParams)
        {

        }

        public override AEdgeType GetNewInstance()
        {
            return new EdgeTypeSingleReference();
        }

        public override AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable, TypeUUID typeOfObjects)
        {
            if (iEnumerable.FirstOrDefault() == null || _ObjectUUID.Item1 != iEnumerable.First().Value.ObjectUUID)
            {
                throw new GraphDBException(new Errors.Error_InvalidEdgeType(typeof(EdgeTypeSetOfReferences)));
            }
            else
            {
                var newInst = GetNewInstance() as EdgeTypeSingleReference;
                newInst.Set(_ObjectUUID.Item1, typeOfObjects);
                return newInst;
            }
        }

        public override AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable, TypeUUID typeOfObjects)
        {
            if (iEnumerable.FirstOrDefault() == null || _ObjectUUID.Item1 != iEnumerable.First())
            {
                throw new GraphDBException(new Errors.Error_InvalidEdgeType(typeof(EdgeTypeSetOfReferences)));
            }
            else
            {
                var newInst = GetNewInstance() as EdgeTypeSingleReference;
                newInst.Set(_ObjectUUID.Item1, typeOfObjects);
                return newInst;
            }
        }

        #endregion

        #region ASingleEdgeType Members

        public override ObjectUUID GetUUID()
        {
            return _ObjectUUID.Item1;
        }

        public override IEnumerable<ObjectUUID> GetAllReferenceIDs()
        {
            yield return _ObjectUUID.Item1;

            yield break;
        }

        public override bool RemoveUUID(ObjectUUID myObjectUUID)
        {
            if (_ObjectUUID.Item1 == myObjectUUID)
            {
                _ObjectUUID = null;
                return true;
            }

            return false;
        }

        public override bool RemoveUUID(IEnumerable<ObjectUUID> myObjectUUIDs)
        {
            if (!myObjectUUIDs.IsNullOrEmpty())
            {
                if (myObjectUUIDs.Contains(_ObjectUUID.Item1))
                {
                    _ObjectUUID = null;
                    return true;
                }

                return false;
            }

            return false;
        }

        public override void Set(ObjectUUID myValue, TypeUUID typeOfDBObject, params ADBBaseObject[] myParameters)
        {
            _ObjectUUID = new Tuple<ObjectUUID, Reference>(myValue, new Reference(myValue, typeOfDBObject));
            _typeOfDBObjects = typeOfDBObject;
        }

        public override void Merge(ASingleReferenceEdgeType mySingleEdgeType)
        {
            var aUUID = mySingleEdgeType.GetUUID();

            _ObjectUUID = new Tuple<ObjectUUID, Reference>(aUUID, new Reference(aUUID, mySingleEdgeType.GetTypeUUIDOfReferences()));
        }

        public override DBObjectReadout GetReadout(Func<ObjectUUID, DBObjectReadout> GetAllAttributesFromDBO)
        {
            return GetAllAttributesFromDBO(_ObjectUUID.Item1);
        }

        public override IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted()
        {
            yield return new Tuple<ObjectUUID, ADBBaseObject>(_ObjectUUID.Item1, null);
        }

        #endregion

        #region IFastSerialize Members

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(ref mySerializationWriter, this);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader, this);
        }

        #endregion

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeTypeSingleReference myValue)
        {
            mySerializationWriter.WriteObject(myValue._typeOfDBObjects);
            if (myValue._ObjectUUID != null)
            {
                mySerializationWriter.WriteObject(myValue._ObjectUUID.Item1.GetByteArray());
            }
            else
            {
                mySerializationWriter.WriteObject(null);
            
            }

        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeTypeSingleReference myValue)
        {
            myValue._typeOfDBObjects = (TypeUUID)mySerializationReader.ReadObject();
            var value = mySerializationReader.ReadByteArray();
            if (value == null)
            {
                myValue._ObjectUUID = null;
            }
            else
            {
                var newUUID = new ObjectUUID(value);
                myValue._ObjectUUID = new Tuple<ObjectUUID, Reference>(newUUID, new Reference(newUUID, myValue._typeOfDBObjects));
            }

            return myValue;
        }

        #region IFastSerializationTypeSurrogate 
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            EdgeTypeSingleReference thisObject = (EdgeTypeSingleReference)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            EdgeTypeSingleReference thisObject = (EdgeTypeSingleReference)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }
        #endregion

        public override String ToString()
        {
            return EdgeTypeUUID.ToString() + "," + EdgeTypeName;
        }

        public override String GetDescribeOutput(GraphDBType myGraphDBType)
        {
            return GetGDDL(myGraphDBType);
        }

        public override String GetGDDL(GraphDBType myGraphDBType)
        {
            return myGraphDBType.Name;
        }

        #region Equals



        public Boolean Equals(EdgeTypeSingleReference p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (EdgeTypeName != p.EdgeTypeName)
            {
                return false;
            }
            if (EdgeTypeUUID != p.EdgeTypeUUID)
            {
                return false;
            }
            if (_ObjectUUID.Item2 != p._ObjectUUID.Item2)
            {
                return false;
            }

            return true;
        }

        public static Boolean operator ==(EdgeTypeSingleReference a, EdgeTypeSingleReference b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(EdgeTypeSingleReference a, EdgeTypeSingleReference b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EdgeTypeSingleReference))
            {
                return false;
            }

            var otherEdge = (EdgeTypeSingleReference)obj;

            if (EdgeTypeName != otherEdge.EdgeTypeName)
            {
                return false;
            }
            if (EdgeTypeUUID != otherEdge.EdgeTypeUUID)
            {
                return false;
            }

            if ((_ObjectUUID == null) && (otherEdge._ObjectUUID == null))
            {
                return true;
            }

            return _ObjectUUID.Equals(otherEdge._ObjectUUID);
        }

        #endregion

        public override IEnumerable<Exceptional<DBObjectStream>> GetAllEdgeDestinations(DBObjectCache dbObjectCache)
        {
            yield return _ObjectUUID.Item2.GetDBObjectStream(dbObjectCache);

            yield break;
        }

        public override IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache)
        {
            yield return new Tuple<Exceptional<DBObjectStream>, ADBBaseObject>(_ObjectUUID.Item2.GetDBObjectStream(dbObjectCache), null);

            yield break;
        }

        public override TypeUUID GetTypeUUIDOfReferences()
        {
            return _typeOfDBObjects;
        }
    }
}

