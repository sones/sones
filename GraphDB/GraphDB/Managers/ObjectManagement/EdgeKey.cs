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

/* <id Name=”GraphDB – edge key information object” />
 * <copyright file=”EdgeKey.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>The Edge Key carries information about backward edges of DBObjects.<summary>
 */

#region Usings

using System;
using sones.GraphDB.TypeManagement;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;


#endregion

namespace sones.GraphDB.ObjectManagement
{

    /// <summary>
    /// The Edge Key carries information about backward edges of DBObjects.
    /// </summary>
    public class EdgeKey : IComparable, IComparable<EdgeKey>, IFastSerialize, IFastSerializationTypeSurrogate
    {

        #region properties

        #region TypeCode
        public UInt32 TypeCode { get { return 205; } }
        #endregion

        #region typeUUID

        private TypeUUID _typeUUID;

        public TypeUUID TypeUUID
        {
            get { return _typeUUID; }
        }

        #endregion

        #region attrUUID

        private AttributeUUID _attrUUID;

        public AttributeUUID AttrUUID
        {
            get { return _attrUUID; }
        }

        #endregion

        #endregion

        #region Constructor

        #region EdgeKey(attr, objectUUID)

        public EdgeKey()
        {
            _typeUUID = null;
            _attrUUID = null;
            _isDirty = false;
        }

        public EdgeKey(TypeUUID typeUUID, AttributeUUID attrUUID)
        {
            _typeUUID = typeUUID;
            _attrUUID = attrUUID;
            _isDirty = false;
        }

        public EdgeKey(ref SerializationReader mySerializationReader)
            : this(new TypeUUID(), new AttributeUUID())
        {
            Deserialize(ref mySerializationReader);
        }

        public EdgeKey(TypeAttribute myTypeAttribute)
            : this(myTypeAttribute.RelatedGraphDBTypeUUID, myTypeAttribute.UUID)
        { }

        public EdgeKey(TypeUUID typeUUID)
        {
            _typeUUID = typeUUID;
            _isDirty = false;
        }

        #endregion

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return CompareTo((EdgeKey)obj);
        }

        #endregion

        #region IComparable<EdgeKey> Members

        public int CompareTo(EdgeKey other)
        {
            if (this._typeUUID.CompareTo(other.TypeUUID) == 0)
            {
                if (this._attrUUID.CompareTo(other.AttrUUID) == 0)
                {
                    return 0;
                }
            }

            return -1;
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is EdgeKey)
            {
                return Equals((EdgeKey)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(EdgeKey p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this._typeUUID == p.TypeUUID) && (this._attrUUID == p.AttrUUID);
        }

        public static Boolean operator ==(EdgeKey a, EdgeKey b)
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

        public static Boolean operator !=(EdgeKey a, EdgeKey b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            if (_attrUUID == null && _typeUUID == null)
                return 0;
            
            if (_attrUUID == null)
            {
                return _typeUUID.GetHashCode();
            }

            if (_typeUUID == null)
            {
                return _attrUUID.GetHashCode();
            }

            return _typeUUID.GetHashCode() ^ _attrUUID.GetHashCode();
        }

        #endregion

        public override string ToString()
        {
            if (_attrUUID == null)
                return String.Concat(_typeUUID.ToString(), FSPathConstants.PathDelimiter, "null");
            else
                return String.Concat(_typeUUID.ToString(), FSPathConstants.PathDelimiter, _attrUUID.ToString());
        }

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }
        private bool _isDirty;

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(ref mySerializationWriter, this);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader, this);
        }

        #endregion

        private void Serialize(ref SerializationWriter mySerializationWriter, EdgeKey myValue)
        {
            myValue._typeUUID.Serialize(ref mySerializationWriter);
            myValue._attrUUID.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, EdgeKey myValue)
        {
            myValue._typeUUID = new TypeUUID();
            myValue._attrUUID = new AttributeUUID();
            myValue._typeUUID.Deserialize(ref mySerializationReader);
            myValue._attrUUID.Deserialize(ref mySerializationReader);

            return myValue;
        }

        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter mySerializationWriter, object value)
        {
            Serialize(ref mySerializationWriter, (EdgeKey)value);
        }

        public object Deserialize(SerializationReader mySerializationReader, Type type)
        {
            EdgeKey thisObject = (EdgeKey)Activator.CreateInstance(type);
            return Deserialize(ref mySerializationReader, thisObject);
        }

        #endregion

        public void SetAttributeUUID(AttributeUUID attributeUUID)
        {
            _attrUUID = attributeUUID;
            
        }

        public Tuple<GraphDBType, TypeAttribute> GetTypeAndAttributeInformation(DBTypeManager myDBTypeManager)
        {
            var type = myDBTypeManager.GetTypeByUUID(this.TypeUUID);
            if (type != null)
            {
                return new Tuple<GraphDBType, TypeAttribute>(type, type.GetTypeAttributeByUUID(this.AttrUUID));
            }
            else
            {
                 return new Tuple<GraphDBType, TypeAttribute>(type, null);
           }
        }
    }

}
