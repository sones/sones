/* <id name="PandoraDB DBTypeAttribute" />
 * <copyright file="DBTypeAttribute"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>An attribute of a type. Used in functions.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using sones.Lib.NewFastSerializer;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    public class DBTypeAttribute : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID("50");
        public const string Name = DBConstants.DBTypeAttribute;

        #region TypeCode

        public override UInt32 TypeCode { get { return 414; } }

        #endregion

        #region Data

        private TypeAttribute _Value;

        #endregion

        #region Constructors

        public DBTypeAttribute()
        {
            _Value = new TypeAttribute();
        }
        
        public DBTypeAttribute(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBTypeAttribute(Object myValue)
        {
            Value = myValue;
        }

        public DBTypeAttribute(TypeAttribute myValue)
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
            return _Value.CompareTo((TypeAttribute)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                    if (value is TypeAttribute)
                        _Value = ((TypeAttribute)value);
                    else if (value is IDChainDefinition)
                        _Value = ((IDChainDefinition)value).LastAttribute;
                    else
                        _Value = null;
                else
                    _Value = null;
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator +(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator -(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator *(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator /(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Add(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Sub(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Mul(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Div(ADBBaseObject myPandoraObject)
        {
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            if (myObject != null && (myObject is TypeAttribute))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBTypeAttribute.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBTypeAttribute(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBTypeAttribute(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {
            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                case DBObjectInitializeType.MinValue:
                case DBObjectInitializeType.MaxValue:
                default:
                    _Value = new TypeAttribute();
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.Unknown; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBTypeAttribute myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBTypeAttribute myValue)
        {
            myValue._Value.Deserialize(ref mySerializationReader);
            return myValue;
        }

        #region IFastSerializationTypeSurrogate 
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBTypeAttribute)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBTypeAttribute thisObject = (DBTypeAttribute)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        #region ToString(IFormatProvider provider)

        public override string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        #endregion

    }
}
