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

/* <id name="sones GraphDB – AttributeUUID" />
 * <copyright file="AttributeUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class has been created in favour of getting compile errors when referencing an attribute.</summary>
 */

#region usings

using System;
using sones.Lib.DataStructures.UUID;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;

#endregion

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class has been created in favour of getting compile errors when referencing an attribute.
    /// </summary>

    public class AttributeUUID : IComparable, IComparable<AttributeUUID>, IFastSerialize, IFastSerializationTypeSurrogate
    {
        #region TypeCode
        public uint TypeCode
        {
            get { return 221; }
        }
        #endregion

        public UInt16 ID { get; private set; }

        #region Constructors

        #region AttributeUUID()

        public AttributeUUID()
        {
        }

        #endregion

        #region AttributeUUID(myUInt64)

        public AttributeUUID(UInt16 myID)
        {
            ID = myID;
        }

        public AttributeUUID(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader);
        }

        #endregion

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            // Check if myObject is null
            if (obj == null)
                throw new ArgumentNullException("obj must not be null!");

            // Check if myObject can be casted to a Reference object
            var myAttributeUUID = obj as AttributeUUID;
            if (myAttributeUUID == null)
                throw new ArgumentException("myObject is not of type AttributeUUID!");

            return CompareTo(myAttributeUUID);
        }

        #endregion

        #region IComparable<AttributeUUID> Members

        public int CompareTo(AttributeUUID other)
        {
            // Check if other is null
            if (other == null)
                throw new ArgumentNullException("other must not be null!");

            return this.ID.CompareTo(other.ID);
        }

        #endregion

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteUInt16(ID);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            ID = mySerializationReader.ReadUInt16();
        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter writer, object value)
        {
            writer.WriteUInt16(((AttributeUUID)value).ID);
        }

        public object Deserialize(SerializationReader reader, Type type)
        {
            AttributeUUID thisObject = (AttributeUUID)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        private object Deserialize(ref SerializationReader reader, AttributeUUID thisObject)
        {
            thisObject.ID = reader.ReadUInt16();

            return thisObject;
        }

        #endregion

        #region override

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is AttributeUUID)
            {
                AttributeUUID p = (AttributeUUID)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(AttributeUUID p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return this.ID == p.ID;
        }

        public static Boolean operator ==(AttributeUUID a, AttributeUUID b)
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

        public static Boolean operator !=(AttributeUUID a, AttributeUUID b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return String.Format("ID: {0}", ID.ToString());
        }

        #endregion

        #endregion

        internal void SetID(ushort myNewID)
        {
            ID = myNewID;
        }
    }
}
