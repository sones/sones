/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – DBUInt64" />
 * <copyright file="DBUInt64.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The UInt64.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;

using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.PandoraTypes
{
    
    public class DBUInt64 : DBNumber
    {
        public new static readonly TypeUUID UUID = new TypeUUID(1040);
        public new const string Name = DBConstants.DBUnsignedInteger;

        #region TypeCode 
        public override UInt32 TypeCode { get { return 412; } }
        #endregion

        #region Data

        private UInt64 _Value;

        #endregion

        #region Constructors

        public DBUInt64()
        {
            _Value = 0;
        }

        public DBUInt64(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBUInt64(Object myValue)
        {
            Value = myValue;
        }

        public DBUInt64(UInt64 myValue)
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
            UInt64 val;
            if (obj is DBUInt64)
                val = (UInt64)((DBUInt64)obj).Value;
            else
                val = Convert.ToUInt64(obj);
            return _Value.CompareTo(val);
        }
        
        public override Object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBUInt64)
                {
                    _Value = ((DBUInt64)value)._Value;
                }
                else
                {
                    if (value is ADBBaseObject)
                    {
                        _Value = Convert.ToUInt64(((ADBBaseObject)value).Value);
                    }
                    else
                    {
                        _Value = Convert.ToUInt64(value);
                    }
                }
            }
        }
        
        #endregion

        #region Operations

        public static DBUInt64 operator +(DBUInt64 myPandoraObjectA, UInt64 myValue)
        {
            myPandoraObjectA.Value = (UInt64)myPandoraObjectA.Value + myValue;
            return myPandoraObjectA;
        }

        public static DBUInt64 operator -(DBUInt64 myPandoraObjectA, UInt64 myValue)
        {
            myPandoraObjectA.Value = (UInt64)myPandoraObjectA.Value - myValue;
            return myPandoraObjectA;
        }

        public static DBUInt64 operator *(DBUInt64 myPandoraObjectA, UInt64 myValue)
        {
            myPandoraObjectA.Value = (UInt64)myPandoraObjectA.Value * myValue;
            return myPandoraObjectA;
        }

        public static DBUInt64 operator /(DBUInt64 myPandoraObjectA, UInt64 myValue)
        {
            myPandoraObjectA.Value = (UInt64)myPandoraObjectA.Value / myValue;
            return myPandoraObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {

            UInt64 valA = Convert.ToUInt64(myPandoraObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myPandoraObjectB.Value);
            return new DBUInt64(valA + valB);
        }

        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            UInt64 valA = Convert.ToUInt64(myPandoraObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myPandoraObjectB.Value);
            return new DBUInt64(valA - valB);
        }

        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            UInt64 valA = Convert.ToUInt64(myPandoraObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myPandoraObjectB.Value);
            return new DBUInt64(valA * valB);
        }

        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            UInt64 valA = Convert.ToUInt64(myPandoraObjectA.Value);
            UInt64 valB = Convert.ToUInt64(myPandoraObjectB.Value);
            return new DBUInt64(valA / valB);
        }

        public override void Add(ADBBaseObject myPandoraObject)
        {
            _Value += Convert.ToUInt64(myPandoraObject.Value);
        }

        public override void Sub(ADBBaseObject myPandoraObject)
        {
            _Value -= Convert.ToUInt64(myPandoraObject.Value);
        }

        public override void Mul(ADBBaseObject myPandoraObject)
        {
            _Value *= Convert.ToUInt64(myPandoraObject.Value);
        }

        public override void Div(ADBBaseObject myPandoraObject)
        {
            _Value /= Convert.ToUInt64(myPandoraObject.Value);
        }

        #endregion


        #region IsValid

        public new static Boolean IsValid(Object myObject)
        {
            if (myObject == null) return false;
            
            UInt64 newValue;
            UInt64.TryParse(myObject.ToString(), out newValue);

            return myObject.ToString() == newValue.ToString();
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBUInt64.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBUInt64(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBUInt64(myValue);
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
                    _Value = UInt64.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = UInt64.MaxValue;
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
            get { return TypesOfOperatorResult.UInt64; }
        }

        public override TypeUUID ID
        {
            get { return UUID; }
        }

        public override string ObjectName
        {
            get { return Name; }
        }

        private void Serialize(ref SerializationWriter mySerializationWriter, DBUInt64 myValue)
        {
            mySerializationWriter.WriteObject((UInt64)myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBUInt64 myValue)
        {
            myValue._Value = (UInt64)mySerializationReader.ReadObject();
            return myValue;
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

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            DBUInt64 thisObject = (DBUInt64)value;
            Serialize(ref writer, thisObject);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBUInt64 thisObject = (DBUInt64)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        public override string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }
    }
}
