/* <id name="GraphDB – DBBoolean" />
 * <copyright file="DBBoolean.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The Boolean.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBBoolean : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID(2000);
        public const string Name = DBConstants.DBBoolean;

        #region TypeCode
        public override UInt32 TypeCode { get { return 402; } }
        #endregion

        #region Data

        private Boolean _Value;

        #endregion

        #region Constructors

        public DBBoolean()
        {
            _Value = false;
        }
        
        public DBBoolean(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBBoolean(Object myValue)
        {
            Value = myValue;
        }

        public DBBoolean(Boolean myValue)
        {
            _Value = myValue;
        }

        #endregion

        #region Overrides

        public override int CompareTo(ADBBaseObject obj)
        {
            return CompareTo(obj.Value);
        }

        public override int CompareTo(Object obj)
        {
            Boolean val;
            if (obj is DBBoolean)
                val = (Boolean)((DBBoolean)obj).Value;
            else
                val = Convert.ToBoolean(obj);
            return _Value.CompareTo(val);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBBoolean)
                    _Value = ((DBBoolean)value)._Value;
                else if (value != null)
                    _Value = Convert.ToBoolean(value);
                else
                    _Value = false;
            }
        }

        #endregion

        #region Operations

        public static DBBoolean operator +(DBBoolean myGraphObjectA, Boolean myValue)
        {
            throw new NotImplementedException();
        }

        public static DBBoolean operator -(DBBoolean myGraphObjectA, Boolean myValue)
        {
            throw new NotImplementedException();
        }

        public static DBBoolean operator *(DBBoolean myGraphObjectA, Boolean myValue)
        {
            throw new NotImplementedException();
        }

        public static DBBoolean operator /(DBBoolean myGraphObjectA, Boolean myValue)
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

            if (myObject.ToString() == "0" || myObject.ToString() == "1")
                return true;

            Boolean newValue;
            return Boolean.TryParse(myObject.ToString(), out newValue);
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBBoolean.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBBoolean(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBBoolean(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {
            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                    _Value = false;
                    break;
                case DBObjectInitializeType.MinValue:
                    _Value = false;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = true;
                    break;
                default:
                    _Value = false;
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.Boolean; }
        }

        public override TypeUUID ID
        {
            get { return UUID; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBBoolean myValue)
        {
            mySerializationWriter.WriteBoolean((Boolean)myValue.Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBBoolean myValue)
        {
            myValue._Value = mySerializationReader.ReadBoolean();
            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBBoolean)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBBoolean thisObject = (DBBoolean)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        #region ToString(IFormatProvider provider)

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }

        #endregion
    }
}
