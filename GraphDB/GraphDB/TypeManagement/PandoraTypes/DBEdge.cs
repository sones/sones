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

/* <id name="sones GraphDB DBList DBList" />
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
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;

using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Exceptions;

namespace sones.GraphDB.TypeManagement.PandoraTypes
{
    public class DBEdge : ADBBaseObject
    {
        public static readonly TypeUUID UUID = new TypeUUID(20);
        public const string Name = "SET";

        #region TypeCode
        public override UInt32 TypeCode { get { return 407; } }
        #endregion

        #region Data

        private IEnumerable<Exceptional<DBObjectStream>> _Value;

        #endregion

        #region Constructors

        public DBEdge()
        {
            _Value = null;
        }
        
        public DBEdge(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBEdge(Object myValue)
        {
            Value = myValue;
        }

        #endregion

        #region Overrides

        public override int CompareTo(ADBBaseObject obj)
        {
            return CompareTo(obj.Value);
        }

        public override int CompareTo(object obj)
        {
            return (_Value == obj) ? 0 : 1;
        }

        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value is DBEdge)
                    _Value = ((DBEdge)value)._Value;
                else if (value is IEnumerable<Exceptional<DBObjectStream>>)
                    _Value = value as IEnumerable<Exceptional<DBObjectStream>>;
                else 
                    throw new GraphDBException(new Errors.Error_DataTypeDoesNotMatch("IEnumerable<Exceptional<DBObjectStream>>", value.GetType().Name));
            }
        }

        #endregion

        #region Operations

        [Obsolete("Operator '+' cannot be applied to operands of type 'Object' and 'Object'")]
        public static DBEdge operator +(DBEdge myPandoraObjectA, Object myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'Object' and 'Object'")]
        public static DBEdge operator -(DBEdge myPandoraObjectA, Object myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'Object' and 'Object'")]
        public static DBEdge operator *(DBEdge myPandoraObjectA, Object myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'Object' and 'Object'")]
        public static DBEdge operator /(DBEdge myPandoraObjectA, Object myValue)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'Object' and 'Object'")]
        public override ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'Object' and 'Object'")]
        public override ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'Object' and 'Object'")]
        public override ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'Object' and 'Object'")]
        public override ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB)
        {
            return myPandoraObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'Object' and 'Object'")]
        public override void Add(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'Object' and 'Object'")]
        public override void Sub(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'Object' and 'Object'")]
        public override void Mul(ADBBaseObject myPandoraObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'Object' and 'Object'")]
        public override void Div(ADBBaseObject myPandoraObject)
        {
        }

        #endregion

        #region IsValid

        public static Boolean IsValid(Object myObject)
        {
            return (myObject != null &&
                (myObject is DBEdge || myObject is EdgeTypeWeightedList || myObject is EdgeTypeSetOfReferences || myObject is HashSet<ObjectUUID> || myObject is IEnumerable<Exceptional<DBObjectStream>>));
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBEdge.IsValid(myValue);
        }

        #endregion

        #region Clone

        public override ADBBaseObject Clone()
        {
            return new DBEdge(_Value);
        }

        public override ADBBaseObject Clone(Object myValue)
        {
            return new DBEdge(myValue);
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
                    _Value = null;
                    break;
            }
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }

        public override TypesOfOperatorResult Type
        {
            get { return TypesOfOperatorResult.SetOfDBObjects; }
        }

        public override TypeUUID ID
        {
            get { return UUID; }
        }

        public override string ObjectName
        {
            get { return Name; }
        }

        public IEnumerable<Exceptional<DBObjectStream>> GetDBObjects()
        {
            if (_Value is IEnumerable<Exceptional<DBObjectStream>>)
            {
                return _Value as IEnumerable<Exceptional<DBObjectStream>>;
            }
            else
            {
                throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
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

        private void Serialize(ref SerializationWriter mySerializationWriter, DBEdge myValue)
        {
            mySerializationWriter.WriteObject(myValue._Value);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBEdge myValue)
        {
            myValue._Value = mySerializationReader.ReadObject() as IEnumerable<Exceptional<DBObjectStream>>;
            return myValue;
        }

        #region IFastSerializationTypeSurrogate 
        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBEdge)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBEdge thisObject = (DBEdge)Activator.CreateInstance(type);
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
