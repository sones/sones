/* <id name="GraphDB DBTypeAttribute" />
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
using sones.GraphDB.TypeManagement;
using sones.Lib;


namespace sones.GraphDB.TypeManagement.BasicTypes
{
    public class DBTypeAttribute : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID("50");
        public const string Name = DBConstants.DBTypeAttribute;

        private UInt64 _estimatedSize = 0;

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

            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only
        }

        public DBTypeAttribute(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)
        }

        public DBTypeAttribute(Object myValue)
        {
            Value = myValue;

            CalcEstimatedSize(this);
        }

        public DBTypeAttribute(TypeAttribute myValue)
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

                CalcEstimatedSize(this);
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator +(DBTypeAttribute myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator -(DBTypeAttribute myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator *(DBTypeAttribute myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator /(DBTypeAttribute myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Add(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Sub(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Mul(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Div(ADBBaseObject myGraphObject)
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

            CalcEstimatedSize(this);

        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override BasicType Type
        {
            get { return BasicType.Unknown; }
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

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(DBTypeAttribute myTypeAttribute)
        {
            //TypeAttribute + TypeCode + EstimatedSize

            _estimatedSize = EstimatedSizeConstants.TypeAttribute + base.GetBaseSize();
        }

        #endregion
    }
}
