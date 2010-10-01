/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
using sones.GraphDB.TypeManagement;
using sones.Lib;

namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBBoolean : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID(2000);
        public const string Name = DBConstants.DBBoolean;

        private UInt64 _estimatedSize = 0;

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
            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only
        }
        
        public DBBoolean(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)

        }

        public DBBoolean(Object myValue)
        {
            Value = myValue;

            CalcEstimatedSize(this);

        }

        public DBBoolean(Boolean myValue)
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

                CalcEstimatedSize(this);

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

            CalcEstimatedSize(this);

        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override BasicType Type
        {
            get { return BasicType.Boolean; }
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

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(DBBoolean myTypeAttribute)
        {
            //DateTime + TypeCode + EstimatedSize
            _estimatedSize = EstimatedSizeConstants.Boolean + GetBaseSize();
        }

        #endregion
    }
}
