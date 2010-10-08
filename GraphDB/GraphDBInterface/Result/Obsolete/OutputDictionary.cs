/* <id name="GraphDB – OutputDictionary" />
 * <copyright file="OutputDictionary.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Is a mapping to the generic Dictionary<,>.</summary>
 */

using System;
using System.Collections.Generic;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Result
{   
 
    [Serializable]
    public class OutputDictionary : IObject
    {
        
        public Dictionary<String, IObject> Dictionary { get; private set; }
        private UInt64 _estimatedSize = 0;

        public OutputDictionary() { }

        public OutputDictionary(Dictionary<String, IObject> myDict)
        {
            Dictionary = myDict;

            foreach (var aItem in myDict)
            {
                _estimatedSize += aItem.Value.GetEstimatedSize();
            }
        }

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IFastSerialize Members


        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            throw new NotImplementedException();
        }

        public void Serialize(SerializationWriter writer, object value)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(SerializationReader reader, Type type)
        {
            throw new NotImplementedException();
        }

        public uint TypeCode
        {
            get { return UInt32.MaxValue - 2; }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IObject

        public ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        #endregion
    }

}
