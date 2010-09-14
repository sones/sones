/* <id name="GraphDB – OutputList" />
 * <copyright file="OutputList.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Is a mapping to the generic List<>.</summary>
 */

using System;
using System.Collections.Generic;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Result
{
    
    public class OutputList : IObject
    {
            
        public List<IObject> List { get; set; }
        public OutputList()
        {
            List = new List<IObject>();
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
            get { return UInt32.MaxValue - 1; }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
