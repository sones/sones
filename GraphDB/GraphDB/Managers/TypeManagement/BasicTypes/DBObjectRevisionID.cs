/*
 * IGraphDBInterface
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.Lib;

#endregion

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBObjectRevisionID : ADBBaseObject
    {

        public new static readonly TypeUUID UUID = new TypeUUID(2030);
        public new const string Name = DBConstants.DBObjectRevisionID;

        private UInt64 _estimatedSize = 0;

        #region TypeCode
        
        public override UInt32 TypeCode { get { return 1406; } }

        #endregion

        #region Data

        private ObjectRevisionID _Value;

        #endregion

        #region Constructors

        public DBObjectRevisionID()
        {
            _Value = null;

            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only
        }
        
        public DBObjectRevisionID(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)

        }

        public DBObjectRevisionID(Object myValue)
        {
            Value = myValue;

            CalcEstimatedSize(this);
        }

        public DBObjectRevisionID(ObjectRevisionID myValue)
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

            ObjectRevisionID val;
            if (obj is DBObjectRevisionID)
                val = (ObjectRevisionID)((DBObjectRevisionID)obj).Value;
            else
                val = (ObjectRevisionID) obj;

            return _Value.CompareTo(val);

        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBObjectRevisionID)
                {
                    _Value = ((DBObjectRevisionID)value)._Value;
                }

                CalcEstimatedSize(this);
            }
        }

        #endregion

        #region Operations

        public static DBObjectRevisionID operator +(DBObjectRevisionID myGraphObjectA, Int64 myValue)
        {
            throw new NotImplementedException();
        }

        public static DBObjectRevisionID operator -(DBObjectRevisionID myGraphObjectA, Int64 myValue)
        {
            throw new NotImplementedException();
        }

        public static DBObjectRevisionID operator *(DBObjectRevisionID myGraphObjectA, Int64 myValue)
        {
            throw new NotImplementedException();
        }

        public static DBObjectRevisionID operator /(DBObjectRevisionID myGraphObjectA, Int64 myValue)
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

        public new static Boolean IsValid(Object myObject)
        {

            if (myObject == null) return false;

            Int64 newValue;
            Int64.TryParse(myObject.ToString(), out newValue);

            return myObject.ToString() == newValue.ToString();

        }

        public override bool IsValidValue(Object myValue)
        {
            return DBInt64.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBObjectRevisionID(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBObjectRevisionID(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {

            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                    _Value = null;
                    break;
                case DBObjectInitializeType.MinValue:
                    _Value = null;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = null;
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
            get { return BasicType.ObjectRevisionID; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBObjectRevisionID myValue)
        {
            mySerializationWriter.WriteObject((ObjectRevisionID)myValue.Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBObjectRevisionID myValue)
        {
            myValue._Value = (ObjectRevisionID) mySerializationReader.ReadObject();

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
            Serialize(ref writer, (DBObjectRevisionID) value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBObjectRevisionID thisObject = (DBObjectRevisionID)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString();
        }

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(DBObjectRevisionID myTypeAttribute)
        {
            if (_Value != null)
            {
                //ObjectRevisionID(UUID + UInt64) + BaseSize
                _estimatedSize = EstimatedSizeConstants.CalcUUIDSize(_Value.UUID) + EstimatedSizeConstants.UInt64 + GetBaseSize();
            }
            else
            {
                 //BaseSize
                _estimatedSize = GetBaseSize();
            }
        }

        #endregion

    }
}
