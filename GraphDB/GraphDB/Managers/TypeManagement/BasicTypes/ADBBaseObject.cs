/* <id name="PandoraDB – ADBBaseObject" />
 * <copyright file="ADBBaseObject.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This is the base object. Each DataObject like Int, String, Double must derive from this class.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Structures.Result;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement.BasicTypes
{

    public enum DBObjectInitializeType : byte
    {
        Default,
        MinValue,
        MaxValue
    }

//    [Serializable]
    public abstract class ADBBaseObject : AObject, IComparable, IFastSerialize, IFastSerializationTypeSurrogate
    {

        public abstract ADBBaseObject Add(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB);
        public abstract ADBBaseObject Sub(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB);
        public abstract ADBBaseObject Mul(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB);
        public abstract ADBBaseObject Div(ADBBaseObject myPandoraObjectA, ADBBaseObject myPandoraObjectB);

        public abstract void Add(ADBBaseObject myPandoraObject);
        public abstract void Sub(ADBBaseObject myPandoraObject);
        public abstract void Mul(ADBBaseObject myPandoraObject);
        public abstract void Div(ADBBaseObject myPandoraObject);

        #region IComparable Members

        public abstract int CompareTo(ADBBaseObject obj);
        public abstract int CompareTo(object obj);

        #endregion

        public abstract TypesOfOperatorResult Type { get; } 

        public abstract void SetValue(DBObjectInitializeType myDBObjectInitializeType);
        public abstract void SetValue(Object myValue);

        public abstract TypeUUID ID { get; }
        public abstract string ObjectName { get; }

        /// <summary>
        /// Create an exact clone
        /// </summary>
        /// <returns></returns>
        public abstract ADBBaseObject Clone();

        /// <summary>
        /// Create a clone and set the value
        /// </summary>
        /// <param name="myValue">The new value for this cloned type</param>
        /// <returns></returns>
        public abstract ADBBaseObject Clone(Object myValue);

        public abstract Object Value
        {
            get;
            set;
        }

        public abstract Boolean IsValidValue(Object myValue);

        public override string ToString()
        {
            return Value.ToString();// +" {" + Value.GetType().Name + "}";
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public abstract String ToString(IFormatProvider provider);

        public override bool Equals(object obj)
        {
            if (obj is ADBBaseObject)
                return Value.Equals(((ADBBaseObject)obj).Value);
            else
                return Value.Equals(obj);

        }
    }
}
