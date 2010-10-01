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

/* <id name="GraphDB – DBInt64" />
 * <copyright file="DBInt64.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The Int64.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.Lib.NewFastSerializer;
using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.GraphDB.TypeManagement;


namespace sones.GraphDB.TypeManagement.BasicTypes
{
    
    public class DBInt64 : DBNumber
    {

        public new static readonly TypeUUID UUID = new TypeUUID(1030);
        public new const string Name = DBConstants.DBInteger;

        #region TypeCode
        
        public override UInt32 TypeCode { get { return 406; } }

        #endregion

        #region Data

        private Int64 _Value;

        #endregion

        #region Constructors

        public DBInt64()
        {
            _Value = 0;
        }
        
        public DBInt64(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBInt64(Object myValue)
        {
            Value = myValue;
        }

        public DBInt64(Int64 myValue)
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
            Int64 val;
            if (obj is DBInt64)
                val = (Int64)((DBInt64)obj).Value;
            else
                val = Convert.ToInt64(obj);
            return _Value.CompareTo(val);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBInt64)
                {
                    _Value = ((DBInt64)value)._Value;
                }
                else
                {
                    if (value is ADBBaseObject)
                    {
                        _Value = Convert.ToInt64(((ADBBaseObject)value).Value);
                    }
                    else
                    {
                        if (value is ObjectUUID)
                        {
                            _Value = Convert.ToInt64(((ObjectUUID)value).ToString());
                        }
                        else
                        {
                            _Value = Convert.ToInt64(value);
                        }
                    }
                }
            }
        }

        #endregion

        #region Operations

        public static DBInt64 operator +(DBInt64 myGraphObjectA, Int64 myValue)
        {
            myGraphObjectA.Value = (Int64)myGraphObjectA.Value + myValue;
            return myGraphObjectA;
        }

        public static DBInt64 operator -(DBInt64 myGraphObjectA, Int64 myValue)
        {
            myGraphObjectA.Value = (Int64)myGraphObjectA.Value - myValue;
            return myGraphObjectA;
        }

        public static DBInt64 operator *(DBInt64 myGraphObjectA, Int64 myValue)
        {
            myGraphObjectA.Value = (Int64)myGraphObjectA.Value * myValue;
            return myGraphObjectA;
        }

        public static DBInt64 operator /(DBInt64 myGraphObjectA, Int64 myValue)
        {
            myGraphObjectA.Value = (Int64)myGraphObjectA.Value / myValue;
            return myGraphObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {

            Int64 valA = Convert.ToInt64(myGraphObjectA.Value);
            Int64 valB = Convert.ToInt64(myGraphObjectB.Value);
            return new DBInt64(valA + valB);
        }

        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            Int64 valA = Convert.ToInt64(myGraphObjectA.Value);
            Int64 valB = Convert.ToInt64(myGraphObjectB.Value);
            return new DBInt64(valA - valB);
        }

        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            Int64 valA = Convert.ToInt64(myGraphObjectA.Value);
            Int64 valB = Convert.ToInt64(myGraphObjectB.Value);
            return new DBInt64(valA * valB);
        }

        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            Int64 valA = Convert.ToInt64(myGraphObjectA.Value);
            Int64 valB = Convert.ToInt64(myGraphObjectB.Value);
            return new DBInt64(valA / valB);
        }

        public override void Add(ADBBaseObject myGraphObject)
        {
            _Value += Convert.ToInt64(myGraphObject.Value);
        }

        public override void Sub(ADBBaseObject myGraphObject)
        {
            _Value -= Convert.ToInt64(myGraphObject.Value);
        }

        public override void Mul(ADBBaseObject myGraphObject)
        {
            _Value *= Convert.ToInt64(myGraphObject.Value);
        }

        public override void Div(ADBBaseObject myGraphObject)
        {
            _Value /= Convert.ToInt64(myGraphObject.Value);
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
            return new DBInt64(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBInt64(myValue);
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
                    _Value = Int64.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = Int64.MaxValue;
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

        public override BasicType Type
        {
            get { return BasicType.Int64; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBInt64 myValue)
        {
            mySerializationWriter.WriteInt64((Int64)myValue.Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBInt64 myValue)
        {
            myValue._Value = mySerializationReader.ReadInt64();
            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBInt64)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBInt64 thisObject = (DBInt64)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }

    }
}
