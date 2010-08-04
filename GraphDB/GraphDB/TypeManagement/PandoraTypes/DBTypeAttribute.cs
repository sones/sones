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

/* <id name="sones GraphDB DBTypeAttribute" />
 * <copyright file="DBTypeAttribute"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>An attribute of a type. Used in functions.</summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using sones.Lib.NewFastSerializer;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.TypeManagement.PandoraTypes
{
    public class DBTypeAttribute : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID("50");
        public const string Name = DBConstants.DBTypeAttribute;

        #region TypeCode

        public override UInt32 TypeCode { get { return 414; } }

        #endregion

        #region Data

        private TypeAttribute _Value;

        #endregion

        #region Constructors

        public DBTypeAttribute()
        {
            _Value = new TypeAttribute();
        }
        
        public DBTypeAttribute(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBTypeAttribute(Object myValue)
        {
            Value = myValue;
        }

        public DBTypeAttribute(TypeAttribute myValue)
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
            return _Value.CompareTo((TypeAttribute)obj);
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                    if (value is TypeAttribute)
                        _Value = ((TypeAttribute)value);
                    else if (value is IDNode)
                        _Value = ((IDNode)value).IDChainDefinition.LastAttribute;
                    else if (value is IDChainDefinition)
                        _Value = ((IDChainDefinition)value).LastAttribute;
                    else
                        _Value = null;
                else
                    _Value = null;
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator +(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator -(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator *(DBTypeAttribute myPandoraObjectA, String myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBReference' and 'DBReference'")]
        public static DBTypeAttribute operator /(DBTypeAttribute myPandoraObjectA, String myValue)
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
            if (myObject != null && ((myObject is IDNode) || myObject is TypeAttribute))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBTypeAttribute.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBTypeAttribute(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBTypeAttribute(myValue);
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
                    _Value = new TypeAttribute();
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.Unknown; }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBTypeAttribute myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBTypeAttribute myValue)
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
            Serialize(ref writer, (DBTypeAttribute)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBTypeAttribute thisObject = (DBTypeAttribute)Activator.CreateInstance(type);
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
