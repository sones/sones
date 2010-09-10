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

/* <id name="PandoraDB – DBDateTime" />
 * <copyright file="DBDateTime.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The DateTime.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;

using sones.Lib.DataStructures.Timestamp;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.PandoraTypes
{
    
    public class DBDateTime : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID(3000);
        public const string Name = DBConstants.DBDateTime;

        #region TypeCode
        public override UInt32 TypeCode { get { return 403; } }
        #endregion

        #region Data

        private DateTime _Value;

        #endregion

        #region Constructors

        public DBDateTime()
        {
            _Value = TimestampNonce.Now;
        }
        
        public DBDateTime(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBDateTime(Object myValue)
        {
            Value = myValue;
        }

        public DBDateTime(DateTime myValue)
        {
            _Value = myValue;
        }

        #endregion

        #region Overrides

        public override int CompareTo(ADBBaseObject obj)
        {
            return CompareTo(obj.Value);
        }

        public override int CompareTo(Object obj)
        {
            DateTime val;
            if (obj is DBDateTime)
                val = (DateTime)((DBDateTime)obj).Value;
            else
                val = Convert.ToDateTime(obj);
            return _Value.CompareTo(val);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBDateTime)
                    _Value = ((DBDateTime)value)._Value;
                else if (value != null)
                    _Value = DateTime.Parse(value.ToString());
                else
                    _Value = TimestampNonce.Now;
            }
        }

        #endregion

        #region Operations

        public static DBDateTime operator +(DBDateTime myPandoraObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public static DBDateTime operator -(DBDateTime myPandoraObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public static DBDateTime operator *(DBDateTime myPandoraObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public static DBDateTime operator /(DBDateTime myPandoraObjectA, DateTime myValue)
        {
            throw new NotImplementedException();
        }

        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {

            throw new NotImplementedException();
        }

        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            throw new NotImplementedException();
        }

        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            throw new NotImplementedException();
        }

        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            throw new NotImplementedException();
        }

        public override void Add(ADBBaseObject myPandoraObject)
        {
            throw new NotImplementedException();
        }

        public override void Sub(ADBBaseObject myPandoraObject)
        {
            throw new NotImplementedException();
        }

        public override void Mul(ADBBaseObject myPandoraObject)
        {
            throw new NotImplementedException();
        }

        public override void Div(ADBBaseObject myPandoraObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            if (myObject == null) return false;

            DateTime newValue;
            return DateTime.TryParse(myObject.ToString(), out newValue);
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBDateTime.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBDateTime(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBDateTime(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {
            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                    _Value = TimestampNonce.Now;
                    break;
                case DBObjectInitializeType.MinValue:
                    _Value = DateTime.MinValue;
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = DateTime.MaxValue;
                    break;
                default:
                    _Value = TimestampNonce.Now;
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.DateTime; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBDateTime myValue)
        {
            mySerializationWriter.WriteObject(myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBDateTime myValue)
        {
            myValue._Value = (DateTime)mySerializationReader.ReadObject();
            return myValue;
        }

        #region IFastSerializationTypeSurrogate
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBDateTime)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBDateTime thisObject = (DBDateTime)Activator.CreateInstance(type);
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
