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


/* <id name="sones GraphDB DBLink DBLink" />
 * <copyright file="DBDouble.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The String.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;

using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.TypeManagement.PandoraTypes
{
    public class DBReference : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID("30");
        public const string Name = DBConstants.DBObject;

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
        }
        
        public DBReference(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBReference(Object myValue)
        {
            Value = myValue;
        }

        public DBReference(ObjectUUID myValue)
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
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator +(DBReference myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator -(DBReference myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator *(DBReference myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBReference operator /(DBReference myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Add(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Sub(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Mul(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public override void Div(ADBBaseObject myPandoraObject)
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
                case DBObjectInitializeType.MinValue:
                case DBObjectInitializeType.MaxValue:
                default:
                    _Value = new ObjectUUID();
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.Reference; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBReference myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBReference myValue)
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
    }
}
