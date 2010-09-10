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

/* <id name="GraphDB – DBInt32" />
 * <copyright file="DBInt32.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The Int32.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.NewFastSerializer;
using sones.GraphDBInterface.TypeManagement;

namespace sones.GraphDB.TypeManagement.BasicTypes
{

//    [Serializable]
    public class DBInt32 : DBNumber
    {

        public new static readonly TypeUUID UUID = new TypeUUID(1020);
        public new static readonly string Name = DBConstants.DBInt32;

        #region TypeCode
        public override UInt32 TypeCode { get { return 405; } }
        #endregion

        #region Data

        private Int32 _Value;

        #endregion

        #region Constructors

        public DBInt32()
        {
            _Value = 0;
        }
        
        public DBInt32(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBInt32(Object myValue)
        {
            Value = myValue;
        }

        public DBInt32(Int32 myValue)
        {
            _Value = myValue;
        }

        #endregion

        #region Overrides

        public override int CompareTo(ADBBaseObject obj)
        {
            return CompareTo(obj.Value);
        }

        public override int CompareTo(object obj)
        {
            Int32 val;
            if (obj is DBInt32)
                val = (Int32)((DBInt32)obj).Value;
            else
                val = Convert.ToInt32(obj);
            return _Value.CompareTo(val);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBInt32)
                    _Value = ((DBInt32)value)._Value;
                else 
                    _Value = Convert.ToInt32(value);
            }
        }

        #endregion

        #region Operations

        public static DBInt32 operator +(DBInt32 myGraphObjectA, Int32 myValue)
        {
            myGraphObjectA.Value = (Int32)myGraphObjectA.Value + myValue;
            return myGraphObjectA;
        }

        public static DBInt32 operator -(DBInt32 myGraphObjectA, Int32 myValue)
        {
            myGraphObjectA.Value = (Int32)myGraphObjectA.Value - myValue;
            return myGraphObjectA;
        }

        public static DBInt32 operator *(DBInt32 myGraphObjectA, Int32 myValue)
        {
            myGraphObjectA.Value = (Int32)myGraphObjectA.Value * myValue;
            return myGraphObjectA;
        }

        public static DBInt32 operator /(DBInt32 myGraphObjectA, Int32 myValue)
        {
            myGraphObjectA.Value = (Int32)myGraphObjectA.Value / myValue;
            return myGraphObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {

            Int32 valA = Convert.ToInt32(myGraphObjectA.Value);
            Int32 valB = Convert.ToInt32(myGraphObjectB.Value);
            return new DBInt32(valA + valB);
        }

        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            Int32 valA = Convert.ToInt32(myGraphObjectA.Value);
            Int32 valB = Convert.ToInt32(myGraphObjectB.Value);
            return new DBInt32(valA - valB);
        }

        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            Int32 valA = Convert.ToInt32(myGraphObjectA.Value);
            Int32 valB = Convert.ToInt32(myGraphObjectB.Value);
            return new DBInt32(valA * valB);
        }

        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            Int32 valA = Convert.ToInt32(myGraphObjectA.Value);
            Int32 valB = Convert.ToInt32(myGraphObjectB.Value);
            return new DBInt32(valA / valB);
        }

        public override void Add(ADBBaseObject myGraphObject)
        {
            _Value += Convert.ToInt32(myGraphObject.Value);
        }

        public override void Sub(ADBBaseObject myGraphObject)
        {
            _Value -= Convert.ToInt32(myGraphObject.Value);
        }

        public override void Mul(ADBBaseObject myGraphObject)
        {
            _Value *= Convert.ToInt32(myGraphObject.Value);
        }

        public override void Div(ADBBaseObject myGraphObject)
        {
            _Value /= Convert.ToInt32(myGraphObject.Value);
        }

        #endregion

        #region IsValid

        public new static Boolean IsValid(Object myObject)
        {
            if (myObject == null) return false;

            Int32 newValue;
            Int32.TryParse(myObject.ToString(), out newValue);
            
            return myObject.ToString() == newValue.ToString();
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBInt32.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBInt32(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBInt32(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {

            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                    _Value = 0;
                    break;
                case DBObjectInitializeType.MinValue:
                    _Value = Int32.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = Int32.MaxValue;
                    break;
                default:
                    _Value = 0;
                    break;
            }

        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override BasicType Type
        {
            get { return BasicType.Int32; }
        }
        
        //public override TypeUUID ID
        //{
        //    get { return UUID; }
        //}

        public override string ObjectName
        {
            get { return Name; }
        }

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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBInt32 myValue)
        {
            mySerializationWriter.WriteInt32((Int32)myValue.Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBInt32 myValue)
        {
            myValue._Value = mySerializationReader.ReadInt32();
            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBInt32)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBInt32 thisObject = (DBInt32)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }
    }
}
