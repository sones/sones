/* <id name="PandoraDB – DBDouble" />
 * <copyright file="DBDouble.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The Double.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBDouble : DBNumber
    {
        public new static readonly TypeUUID UUID = new TypeUUID(1010);
        public new const string Name = DBConstants.DBDouble;

        #region TypeCode
        public override UInt32 TypeCode { get { return 404; } }
        #endregion

        #region Data

        private Double _Value;

        #endregion

        #region Constructors

        public DBDouble()
        {
            _Value = 0;
        }
        
        public DBDouble(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBDouble(Object myValue)
        {
            Value = myValue;
        }

        public DBDouble(Double myValue)
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
            Double val;
            if (obj is DBDouble)
                val = (Double)((DBDouble)obj).Value;
            else
                val = Convert.ToDouble(obj);
            return _Value.CompareTo(val);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBDouble)
                {
                    _Value = ((DBDouble)value)._Value;
                }
                else
                {
                    if (value is ADBBaseObject)
                    {
                        _Value = Convert.ToDouble(((ADBBaseObject)value).Value);
                    }
                    else
                    {
                        _Value = Convert.ToDouble(value);
                    }
                }
            }
        }

        #endregion

        #region Operations

        public static DBDouble operator +(DBDouble myPandoraObjectA, Double myValue)
        {
            myPandoraObjectA.Value = (Double)myPandoraObjectA.Value + myValue;
            return myPandoraObjectA;
        }

        public static DBDouble operator -(DBDouble myPandoraObjectA, Double myValue)
        {
            myPandoraObjectA.Value = (Double)myPandoraObjectA.Value - myValue;
            return myPandoraObjectA;
        }

        public static DBDouble operator *(DBDouble myPandoraObjectA, Double myValue)
        {
            myPandoraObjectA.Value = (Double)myPandoraObjectA.Value * myValue;
            return myPandoraObjectA;
        }

        public static DBDouble operator /(DBDouble myPandoraObjectA, Double myValue)
        {
            myPandoraObjectA.Value = (Double)myPandoraObjectA.Value / myValue;
            return myPandoraObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {

            Double valA = Convert.ToDouble(myPandoraObjectA.Value);
            Double valB = Convert.ToDouble(myPandoraObjectB.Value);
            return new DBDouble(valA + valB);
        }

        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            Double valA = Convert.ToDouble(myPandoraObjectA.Value);
            Double valB = Convert.ToDouble(myPandoraObjectB.Value);
            return new DBDouble(valA - valB);
        }

        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            Double valA = Convert.ToDouble(myPandoraObjectA.Value);
            Double valB = Convert.ToDouble(myPandoraObjectB.Value);
            return new DBDouble(valA * valB);
        }

        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            Double valA = Convert.ToDouble(myPandoraObjectA.Value);
            Double valB = Convert.ToDouble(myPandoraObjectB.Value);
            return new DBDouble(valA / valB);
        }

        public override void Add(ADBBaseObject myPandoraObject)
        {
            _Value += Convert.ToDouble(myPandoraObject.Value);
        }

        public override void Sub(ADBBaseObject myPandoraObject)
        {
            _Value -= Convert.ToDouble(myPandoraObject.Value);
        }

        public override void Mul(ADBBaseObject myPandoraObject)
        {
            _Value *= Convert.ToDouble(myPandoraObject.Value);
        }

        public override void Div(ADBBaseObject myPandoraObject)
        {
            _Value /= Convert.ToDouble(myPandoraObject.Value);
        }

        #endregion

        #region IsValid

        public static new Boolean IsValid(Object myObject)
        {
            if (myObject == null) return false;

            Double newValue;
            try
            {
                newValue = Convert.ToDouble(myObject);
            }
            catch
            {
                return false;
            }

            return true;// (myObject.ToString() == newValue.ToString());
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBDouble.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBDouble(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBDouble(myValue);
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
                    _Value = Double.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = Double.MaxValue;
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

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.Double; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBDouble myValue)
        {
            mySerializationWriter.WriteDouble((Double)myValue.Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBDouble myValue)
        {
            myValue._Value = mySerializationReader.ReadDouble();
            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBDouble)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBDouble thisObject = (DBDouble)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }
    }
}
