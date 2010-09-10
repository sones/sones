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

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphDB.Managers.TypeManagement.BasicTypes
{
    public class DBVertex : ADBBaseObject
    {

        public static readonly TypeUUID UUID = new TypeUUID(DBConstants.DBVertexID);
        public const string Name = DBConstants.DBVertexName;
        
        #region TypeCode

        public override UInt32 TypeCode { get { return 411; } }

        #endregion

        #region Data

        private ObjectUUID _Value;

        #endregion

        #region constructors

        public DBVertex()
        {
            _Value = new ObjectUUID();
        }

        public DBVertex(DBObjectInitializeType myDBObjectInitializeType)
        {
            SetValue(myDBObjectInitializeType);
        }

        public DBVertex(Object myValue)
        {
            Value = myValue;
        }

        public DBVertex(ObjectUUID myValue)
        {
            _Value = myValue;
        }
        
        #endregion

        #region overrides        

        #region operators

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public static DBVertex operator +(DBVertex myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public static DBVertex operator -(DBVertex myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public static DBVertex operator *(DBVertex myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public static DBVertex operator /(DBVertex myGraphObjectA, String myValue)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override ADBBaseObject Add(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override ADBBaseObject Sub(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override ADBBaseObject Mul(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override ADBBaseObject Div(ADBBaseObject myGraphObjectA, ADBBaseObject myGraphObjectB)
        {
            return myGraphObjectA;
        }

        [Obsolete("Operator '+' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override void Add(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '-' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override void Sub(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '*' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override void Mul(ADBBaseObject myGraphObject)
        {
        }

        [Obsolete("Operator '/' cannot be applied to operands of type 'DBVertex' and 'DBVertex'")]
        public override void Div(ADBBaseObject myGraphObject)
        {
        }

        #endregion

        public override BasicType Type
        {
            get { return BasicType.Vertex; }
        }        

        public override string ObjectName
        {
            get { return Name; }
        }

        #region clone
        
        public override ADBBaseObject Clone()
        {
            return new DBVertex(_Value);
        }

        public override ADBBaseObject Clone(object myValue)
        {
            return new DBVertex(myValue);
        }

        #endregion

        #region values        

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
        }

        public override void SetValue(object myValue)
        {
            Value = myValue;
        }
        
        public override object Value
        {
            get { return _Value; }
            set
            {
                if (value != null)
                {
                    if (value is DBVertex)
                    {
                        _Value = ((DBVertex)value)._Value;
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

        public static Boolean IsValid(Object myObject)
        {
            if (myObject != null || (myObject is DBVertex) || (myObject is String))
                return true;

            return false;
        }

        public override bool IsValidValue(Object myValue)
        {
            return DBVertex.IsValid(myValue);
        }

        #endregion

        #region compare

        public override int CompareTo(ADBBaseObject obj)
        {
            return CompareTo(obj.Value);
        }

        public override int CompareTo(object obj)
        {
            return _Value.CompareTo((ObjectUUID)obj);
        }

        #endregion

        #region ToString
        
        public override string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        #endregion

        #region serialize

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(ref mySerializationWriter, this);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            Deserialize(ref mySerializationReader, this);
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (DBVertex)value);
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            DBVertex thisObject = (DBVertex)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        public override bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        private void Serialize(ref SerializationWriter mySerializationWriter, DBVertex myValue)
        {
            myValue._Value.Serialize(ref mySerializationWriter);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, DBVertex myValue)
        {
            myValue._Value.Deserialize(ref mySerializationReader);
            return myValue;
        }

        #endregion

        #endregion
    }
}
