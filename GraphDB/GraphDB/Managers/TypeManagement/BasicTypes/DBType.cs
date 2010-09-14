/* <id name="GraphDB DBType" />
 * <copyright file="DBType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The DBTypeStream</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using sones.Lib.NewFastSerializer;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;


namespace sones.GraphDB.TypeManagement.BasicTypes
{
    public class DBType : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID(40);
        public const string Name = DBConstants.DBType;

        #region TypeCode
        public override UInt32 TypeCode { get { return 413; } }
        #endregion

        #region Data

        private GraphDBType _Value;

        #endregion

        #region Constructors

        public DBType()
        {
            _Value = new GraphDBType();
        }
        
        public DBType(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBType(Object myValue)
        {
            Value = myValue;
        }

        public DBType(GraphDBType myValue)
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
            return _Value.CompareTo((GraphDBType)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                    if (value is DBType)
                        _Value = ((DBType)value)._Value;
                    else if (value is IDChainDefinition)
                        _Value = ((IDChainDefinition)value).LastType;
                    else
                        _Value = null;
                else
                    _Value = null;
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBType operator +(DBType myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBType operator -(DBType myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBType operator *(DBType myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBType operator /(DBType myGraphObjectA, String myValue)
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
            if (myObject != null && (myObject is GraphDBType))
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
            return DBType.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBType(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBType(myValue);
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
                    _Value = new GraphDBType();
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override BasicType Type
        {
            get { return BasicType.Unknown; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBType myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBType myValue)
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
            Serialize(ref writer, (DBType)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBType thisObject = (DBType)Activator.CreateInstance(type);
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
