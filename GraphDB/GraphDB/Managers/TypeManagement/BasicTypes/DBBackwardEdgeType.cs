/* <id name="PandoraDB DBBackwardEdgeType" />
 * <copyright file="DBBackwardEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A BackwardEdge.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Enums;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    public class DBBackwardEdgeType : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID(10);
        public const string Name = DBConstants.DBBackwardEdge;

        #region TypeCode
        public override UInt32 TypeCode { get { return 400; } }
        #endregion


        #region Data

        private EdgeKey _Value;

        #endregion

        #region Constructors

        public DBBackwardEdgeType()
        {
            _Value = new EdgeKey();
        }
        
        public DBBackwardEdgeType(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBBackwardEdgeType(Object myValue)
        {
            Value = myValue;
        }

        public DBBackwardEdgeType(EdgeKey myValue)
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
            return _Value.CompareTo((EdgeKey)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                    if (value is DBBackwardEdgeType)
                        _Value = ((DBBackwardEdgeType)value)._Value;
                    else if (value is EdgeKey)
                        _Value = (EdgeKey)value;
                    else
                        throw new NotImplementedException();
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public static DBBackwardEdgeType operator +(DBBackwardEdgeType myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public static DBBackwardEdgeType operator -(DBBackwardEdgeType myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public static DBBackwardEdgeType operator *(DBBackwardEdgeType myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public static DBBackwardEdgeType operator /(DBBackwardEdgeType myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override void Add(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override void Sub(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override void Mul(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBBackwardEdgeType' and 'DBBackwardEdgeType'")]
        public override void Div(ADBBaseObject myPandoraObject)
        {
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            if (myObject == null || !(myObject is DBBackwardEdgeType))
                return false;

            return true;
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBBackwardEdgeType.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBBackwardEdgeType(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBBackwardEdgeType(myValue);
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
                    _Value = new EdgeKey();
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.BackwardEdge; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBBackwardEdgeType myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBBackwardEdgeType myValue)
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
            Serialize(ref writer, (DBBackwardEdgeType)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBBackwardEdgeType thisObject = (DBBackwardEdgeType)Activator.CreateInstance(type);
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
