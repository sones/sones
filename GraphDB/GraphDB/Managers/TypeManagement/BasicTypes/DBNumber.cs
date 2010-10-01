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

/* <id name="GraphDB – DBNumber" />
 * <copyright file="DBNumber.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The Number.</summary>
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
    /// <summary>
    /// This identifies just a number. We currently don't want to specify whether or not it is a Int64, UInt64, Double or whatever
    /// </summary>
    
//    [Serializable]
    public class DBNumber : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID(1000);
        public const string Name = "Number";

        private UInt64 _estimatedSize = 0;

        #region TypeCode
        public override UInt32 TypeCode { get { return 408; } }
        #endregion

        #region Data

        private Object _Value;

        #endregion

        #region Constructors

        public DBNumber()
        {
            _Value = 0;

            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only
        }
        
        public DBNumber(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)
        }

        public DBNumber(Object myValue)
        {
            Value = myValue;
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
            return ((IComparable)_Value).CompareTo(obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBNumber)
                    _Value = ((DBNumber)value)._Value;
                else 
                    _Value = value;

                CalcEstimatedSize(this);
            }
        }

        #endregion

        #region Operations
        /*

        public static DBNumber operator +(DBNumber myGraphObjectA, Object myValue)
        {
            throw new NotImplementedException();
        }

        public static DBNumber operator -(DBNumber myGraphObjectA, Object myValue)
        {
            throw new NotImplementedException();
        }

        public static DBNumber operator *(DBNumber myGraphObjectA, Object myValue)
        {
            throw new NotImplementedException();
        }

        public static DBNumber operator /(DBNumber myGraphObjectA, Object myValue)
        {
            throw new NotImplementedException();
        }
        */
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
            if (myObject == null)// || !(myObject is ADBBaseObject)) <- the value is never  a ADBBaseObject
                return false;

            if (DBUInt64.IsValid(myObject) || DBInt64.IsValid(myObject) || DBDouble.IsValid(myObject))
                return true;

            return false;
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBNumber.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBNumber(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBNumber(myValue);
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
                    _Value = 0;
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBNumber myValue)
        {
            mySerializationWriter.WriteObject(myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBNumber myValue)
        {
            myValue._Value = mySerializationReader.ReadObject();

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
            Serialize(ref writer, (DBNumber)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBNumber thisObject = (DBNumber)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }


        #region ToString(IFormatProvider provider)

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString();
        }

        #endregion

        #region IObject

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(DBNumber myTypeAttribute)
        {
            //Number(worst case) + TypeCode + EstimatedSize
            _estimatedSize = EstimatedSizeConstants.UInt64 + GetBaseSize();
        }

        #endregion
    }
}
