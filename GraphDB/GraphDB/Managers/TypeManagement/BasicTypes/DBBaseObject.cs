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

/* <id name="GraphDB DBLink DBLink" />
 * <copyright file="DBDouble.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The String.</summary>
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
    public class DBBaseObject : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID(0);
        public const string Name = DBConstants.DBBaseObject;

        private UInt64 _estimatedSize = 0;

        #region TypeCode
        public override UInt32 TypeCode { get { return 401; } }
        #endregion

        #region Data

        private String _Value;

        #endregion

        #region Constructors

        public DBBaseObject()
        {
            _Value = String.Empty;

            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only

        }

        public DBBaseObject(DBObjectInitializeType DBObjectInitializeType)
        {
            SetValue(DBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)

        }

        public DBBaseObject(Object myValue)
        {
            Value = myValue;

            CalcEstimatedSize(this);

        }

        public DBBaseObject(String myValue)
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
            return _Value.CompareTo((String)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBBaseObject)
                    _Value = ((DBBaseObject)value)._Value;
                else if (value != null)
                    _Value = Convert.ToString(value);
                else
                    _Value = String.Empty;

                CalcEstimatedSize(this);

            }
        }

        #endregion

        #region Operations

        public static DBBaseObject operator +(DBBaseObject myGraphObjectA, String myValue)
        {
            myGraphObjectA.Value = (String)myGraphObjectA.Value + myValue;
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'string' and 'string'")]
        public static DBBaseObject operator -(DBBaseObject myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'string' and 'string'")]
        public static DBBaseObject operator *(DBBaseObject myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'string' and 'string'")]
        public static DBBaseObject operator /(DBBaseObject myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {

            String valA = Convert.ToString(myGraphObjectA.Value);
            String valB = Convert.ToString(myGraphObjectB.Value);

            return new DBBaseObject(valA + valB);
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'string' and 'string'")]
        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'string' and 'string'")]
        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'string' and 'string'")]
        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        public override void Add(ADBBaseObject myGraphObject)
        {
            _Value += Convert.ToString(myGraphObject.Value);
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'string' and 'string'")]
        public override void Sub(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'string' and 'string'")]
        public override void Mul(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'string' and 'string'")]
        public override void Div(ADBBaseObject myGraphObject)
        {
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            if (myObject == null || !(myObject is DBBaseObject))
                return false;

            return true;
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBBaseObject.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBBaseObject(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBBaseObject(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType DBObjectInitializeType)
        {
            switch (DBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                case DBObjectInitializeType.MinValue:
                case DBObjectInitializeType.MaxValue:
                default:
                    _Value = String.Empty;
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
            get { return BasicType.Reference; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBBaseObject myValue)
        {
            mySerializationWriter.WriteObject(myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBBaseObject myValue)
        {
            myValue._Value = (String)mySerializationReader.ReadObject();

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
            Serialize(ref writer, (DBBaseObject)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBBaseObject thisObject = (DBBaseObject)Activator.CreateInstance(type);

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

        private void CalcEstimatedSize(DBBaseObject myTypeAttribute)
        {
            //String + BaseSize
            _estimatedSize = EstimatedSizeConstants.CalcStringSize(_Value) + GetBaseSize();
        }

        #endregion
    }
}
