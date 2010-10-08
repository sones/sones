/* <id name="GraphDB – DBUInt64" />
 * <copyright file="DBUInt64.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The UInt64.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;


namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBUInt64 : DBNumber
    {
        public new static readonly TypeUUID UUID = new TypeUUID(1040);
        public new const string Name = DBConstants.DBUnsignedInteger;

        #region TypeCode 
        public override UInt32 TypeCode { get { return 412; } }
        #endregion

        #region Data

        private UInt64 _Value;

        #endregion

        #region Constructors

        public DBUInt64()
        {
            _Value = 0;
        }

        public DBUInt64(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBUInt64(Object myValue)
        {
            Value = myValue;
        }

        public DBUInt64(UInt64 myValue)
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
            UInt64 val;
            if (obj is DBUInt64)
                val = (UInt64)((DBUInt64)obj).Value;
            else
                val = Convert.ToUInt64(obj);
            return _Value.CompareTo(val);
        }
        
        public override Object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBUInt64)
                {
                    _Value = ((DBUInt64)value)._Value;
                }
                else
                {
                    if (value is ADBBaseObject)
                    {
                        _Value = Convert.ToUInt64(((ADBBaseObject)value).Value);
                    }
                    else
                    {
                        _Value = Convert.ToUInt64(value);
                    }
                }
            }
        }
        
        #endregion

        #region Operations

        public static DBUInt64 operator +(DBUInt64 myGraphObjectA, UInt64 myValue)
        {
            myGraphObjectA.Value = (UInt64)myGraphObjectA.Value + myValue;
            return myGraphObjectA;
        }

        public static DBUInt64 operator -(DBUInt64 myGraphObjectA, UInt64 myValue)
        {
            myGraphObjectA.Value = (UInt64)myGraphObjectA.Value - myValue;
            return myGraphObjectA;
        }

        public static DBUInt64 operator *(DBUInt64 myGraphObjectA, UInt64 myValue)
        {
            myGraphObjectA.Value = (UInt64)myGraphObjectA.Value * myValue;
            return myGraphObjectA;
        }

        public static DBUInt64 operator /(DBUInt64 myGraphObjectA, UInt64 myValue)
        {
            myGraphObjectA.Value = (UInt64)myGraphObjectA.Value / myValue;
            return myGraphObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {

            UInt64 valA = Convert.ToUInt64(myGraphObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myGraphObjectB.Value);
            return new DBUInt64(valA + valB);
        }

        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            UInt64 valA = Convert.ToUInt64(myGraphObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myGraphObjectB.Value);
            return new DBUInt64(valA - valB);
        }

        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            UInt64 valA = Convert.ToUInt64(myGraphObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myGraphObjectB.Value);
            return new DBUInt64(valA * valB);
        }

        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            UInt64 valA = Convert.ToUInt64(myGraphObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myGraphObjectB.Value);
            return new DBUInt64(valA / valB);
        }

        public override void Add(ADBBaseObject myGraphObject)
        {
            _Value += Convert.ToUInt64(myGraphObject.Value);
        }

        public override void Sub(ADBBaseObject myGraphObject)
        {
            _Value -= Convert.ToUInt64(myGraphObject.Value);
        }

        public override void Mul(ADBBaseObject myGraphObject)
        {
            _Value *= Convert.ToUInt64(myGraphObject.Value);
        }

        public override void Div(ADBBaseObject myGraphObject)
        {
            _Value /= Convert.ToUInt64(myGraphObject.Value);
        }

        #endregion


        #region IsValid

        public new static Boolean IsValid(Object myObject)
        {
            if (myObject == null) return false;
            
            UInt64 newValue;
            UInt64.TryParse(myObject.ToString(), out newValue);

            return myObject.ToString() == newValue.ToString();
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBUInt64.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBUInt64(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBUInt64(myValue);
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
                    _Value = UInt64.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = UInt64.MaxValue;
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
            get { return BasicType.UInt64; }
        }

        //public override TypeUUID ID
        //{
        //    get { return UUID; }
        //}

        public override string ObjectName
        {
            get { return Name; }
        }

        private void Serialize(ref SerializationWriter mySerializationWriter, DBUInt64 myValue)
        {
            mySerializationWriter.WriteUInt64((UInt64)myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBUInt64 myValue)
        {
            myValue._Value = mySerializationReader.ReadUInt64();
            return myValue;
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

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            DBUInt64 thisObject = (DBUInt64)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBUInt64 thisObject = (DBUInt64)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }
    }
}
