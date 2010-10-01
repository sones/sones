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
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement;
using sones.Lib;


namespace sones.GraphDB.TypeManagement.BasicTypes
{

    public class DBReference : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID("30");
        public const string Name = DBConstants.DBObject;

        private UInt64 _estimatedSize = 0;

        #region TypeCode
        public override UInt32 TypeCode { get { return 409; } }
        #endregion

        #region Data

        private ObjectUUID _Value;

        #endregion

        #region Constructors

        public DBReference()
        {
            _Value = new ObjectUUID();

            //DO NOT ESTIMATE THE SIZE!!! this constructor is for IFastSerializer purpose only
        }
        
        public DBReference(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);

            //DO NOT ESTIMATE THE SIZE!!! it's done in SetValue(...)

        }

        public DBReference(Object myValue)
        {
            Value = myValue;

            CalcEstimatedSize(this);
        }

        public DBReference(ObjectUUID myValue)
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
            return _Value.CompareTo((ObjectUUID)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                {
                    if (value is DBReference)
                    {
                        _Value = ((DBReference)value)._Value;
                    }
                    else
                    {
                        if (value is DBNumber)
                        {
                            _Value = new ObjectUUID(Convert.ToUInt64(((DBNumber)value).Value));
                        }
                        else
                        {
                            if (value is DBString)
                            {
                                _Value = new ObjectUUID((String)((DBString)value).Value);
                            }
                            else
                            {
                                if (value is ObjectUUID)
                                {
                                    _Value = (ObjectUUID)value;
                                }
                                else
                                {
                                    _Value = new ObjectUUID(Convert.ToString(value));
                                }
                            }
                        }
                    }
                }
                else
                {
                    _Value = null;
                }

                CalcEstimatedSize(this);
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator +(DBReference myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator -(DBReference myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator *(DBReference myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator /(DBReference myGraphObjectA, String myValue)
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
            if (myObject != null || (myObject is DBReference) || (myObject is String))
                return true;

            return false;
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBReference.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBReference(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBReference(myValue);
        }

        #endregion

        public override void SetValue(DBObjectInitializeType myDBObjectInitializeType)
        {
            switch (myDBObjectInitializeType)
            {
                case DBObjectInitializeType.Default:
                    _Value = new ObjectUUID(0);
                    break;
                case DBObjectInitializeType.MinValue:
                    _Value = new ObjectUUID(0);
                    break;
                case DBObjectInitializeType.MaxValue:
                    _Value = new ObjectUUID();
                    break;
                default:
                    _Value = new ObjectUUID(0);
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBReference myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBReference myValue)
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
            Serialize(ref writer, (DBReference)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBReference thisObject = (DBReference)Activator.CreateInstance(type);
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

        private void CalcEstimatedSize(DBReference myDBReference)
        {
            //ObjectUUID + BaseSize
            _estimatedSize = EstimatedSizeConstants.CalcUUIDSize(_Value) + GetBaseSize();
        }

        #endregion
    }
}
