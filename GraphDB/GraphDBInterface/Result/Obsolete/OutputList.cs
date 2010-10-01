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
            
        public List<IObject> List { get; private set; }
        private UInt64 _estimatedSize = 0;
        
        public OutputList()
        {
            List = new List<IObject>();
        }

        public OutputList(IEnumerable<IObject> myOutputObjects)
        {
            List = new List<IObject>();

            foreach (var aObject in myOutputObjects)
            {
                List.Add(aObject);
                _estimatedSize += aObject.GetEstimatedSize();
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

        #region IObject Members

        public ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        #endregion
    }

}
