/* <id name="GraphDB – DBDateTime" />
 * <copyright file="DBDateTime.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The DateTime.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.DataStructures.Timestamp;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;
using sones.Lib;


namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBDateTime : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID(3000);
        public const string Name = DBConstants.DBDateTime;

        private UInt64 _estimatedSize = 0;


        #region TypeCode
        public override UInt32 TypeCode { get { return 403; } }
        #endregion

        #region Data

        private DateTime _Value;

        #endregion

        #region Constructors

        public DBDateTime()
        {
            _Value = TimestampNonce.Now;
            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only
        }
        
        public DBDateTime(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)

        }

        public DBDateTime(Object myValue)
        {
            Value = myValue;

            CalcEstimatedSize(this);

        }

        public DBDateTime(DateTime myValue)
        {
            _Value = myValue;

            CalcEstimatedSize(this);

        }

        #endregion

        #region Overrides

        public override int CompareTo(ADBBaseObject obj)
        {
            return CompareTo(obj.Value);
        }

        public override int CompareTo(Object obj)
        {
            DateTime val;
            if (obj is DBDateTime)
                val = (DateTime)((DBDateTime)obj).Value;
            else
                val = Convert.ToDateTime(obj);
            return _Value.CompareTo(val);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBDateTime)
                    _Value = ((DBDateTime)value)._Value;
                else if (value != null)
                    _Value = DateTime.Parse(value.ToString());
                else
                    _Value = TimestampNonce.Now;

                CalcEstimatedSize(this);
            }
        }

        #endregion

        #region Operations

        public static DBDateTime operator +(DBDateTime myGraphObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public static DBDateTime operator -(DBDateTime myGraphObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public static DBDateTime operator *(DBDateTime myGraphObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public static DBDateTime operator /(DBDateTime myGraphObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {

            throw new NotImplementedException();
        }

        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            throw new NotImplementedException();
        }

        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            throw new NotImplementedException();
        }

        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            throw new NotImplementedException();
        }

        public override void Add(ADBBaseObject myGraphObject)
        {
            throw new NotImplementedException();
        }

        public override void Sub(ADBBaseObject myGraphObject)
        {
            throw new NotImplementedException();
        }

        public override void Mul(ADBBaseObject myGraphObject)
        {
            throw new NotImplementedException();
        }

        public override void Div(ADBBaseObject myGraphObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            if (myObject == null) return false;

            DateTime newValue;
            return DateTime.TryParse(myObject.ToString(), out newValue);
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBDateTime.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBDateTime(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBDateTime(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {
            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                    _Value = TimestampNonce.Now;
                    break;
                case DBObjectInitializeType.MinValue:
                    _Value = DateTime.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = DateTime.MaxValue;
                    break;
                default:
                    _Value = TimestampNonce.Now;
                    break;
            }

            CalcEstimatedSize(this);

        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override BasicType Type
        {
            get { return BasicType.DateTime; }
        }

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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBDateTime myValue)
        {
            mySerializationWriter.WriteDateTime((DateTime)myValue.Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBDateTime myValue)
        {
            myValue._Value = mySerializationReader.ReadDateTimeOptimized();

            CalcEstimatedSize(myValue);

            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBDateTime)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBDateTime thisObject = (DBDateTime)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        #region ToString(IFormatProvider provider)

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }

        #endregion

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(DBDateTime myTypeAttribute)
        {
            //DateTime + TypeCode + EstimatedSize
            _estimatedSize = EstimatedSizeConstants.DateTime + GetBaseSize();
        }

        #endregion
    }
}
