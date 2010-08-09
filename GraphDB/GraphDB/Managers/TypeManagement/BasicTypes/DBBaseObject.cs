/* <id name="PandoraDB DBLink DBLink" />
 * <copyright file="DBDouble.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The String.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    public class DBBaseObject : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID(0);
        public const string Name = DBConstants.DBBaseObject;

        #region TypeCode
        public override UInt32 TypeCode { get { return 401; } }
        #endregion

        #region Data

        private String _Value;

        #endregion

        #region Constructors

        public DBBaseObject()
        {
            _Value = String.Empty;
        }

        public DBBaseObject(DBObjectInitializeType DBObjectInitializeType)
        {
            SetValue(DBObjectInitializeType);
        }

        public DBBaseObject(Object myValue)
        {
            Value = myValue;
        }

        public DBBaseObject(String myValue)
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
            return _Value.CompareTo((String)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBBaseObject)
                    _Value = ((DBBaseObject)value)._Value;
                else if (value != null)
                    _Value = Convert.ToString(value);
                else
                    _Value = String.Empty;
            }
        }

        #endregion

        #region Operations

        public static DBBaseObject operator +(DBBaseObject myPandoraObjectA, String myValue)
        {
            myPandoraObjectA.Value = (String)myPandoraObjectA.Value + myValue;
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'string' and 'string'")]
        public static DBBaseObject operator -(DBBaseObject myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'string' and 'string'")]
        public static DBBaseObject operator *(DBBaseObject myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'string' and 'string'")]
        public static DBBaseObject operator /(DBBaseObject myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {

            String valA = Convert.ToString(myPandoraObjectA.Value);
            String valB = Convert.ToString(myPandoraObjectB.Value);
            return new DBBaseObject(valA + valB);
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'string' and 'string'")]
        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'string' and 'string'")]
        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'string' and 'string'")]
        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        public override void Add(ADBBaseObject myPandoraObject)
        {
            _Value += Convert.ToString(myPandoraObject.Value);
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'string' and 'string'")]
        public override void Sub(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'string' and 'string'")]
        public override void Mul(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'string' and 'string'")]
        public override void Div(ADBBaseObject myPandoraObject)
        {
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            if (myObject == null || !(myObject is DBBaseObject))
                return false;

            return true;
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBBaseObject.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBBaseObject(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBBaseObject(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType DBObjectInitializeType)
        {
            switch (DBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                case DBObjectInitializeType.MinValue:
                case DBObjectInitializeType.MaxValue:
                default:
                    _Value = String.Empty;
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.Reference; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBBaseObject myValue)
        {
            mySerializationWriter.WriteObject(myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBBaseObject myValue)
        {
            myValue._Value = (String)mySerializationReader.ReadObject();
            return myValue;
        }

        #region IFastSerializationTypeSurrogate

        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBBaseObject)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBBaseObject thisObject = (DBBaseObject)Activator.CreateInstance(type);
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
