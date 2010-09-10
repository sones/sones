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

/* <id name="GraphdbDB – IndexKeyDefinition" />
 * <copyright file="IndexKeyDefinition.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// IndexKeyDefinition for any AttributeIndex
    /// </summary>
    public class IndexKeyDefinition : IFastSerialize
    {
        #region Data

        int _hashCode = 0;

        private List<AttributeUUID> _indexKeyAttributeUUIDs = new List<AttributeUUID>();
        public List<AttributeUUID> IndexKeyAttributeUUIDs { get { return _indexKeyAttributeUUIDs; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Needed for IFastSerialize
        /// </summary>
        public IndexKeyDefinition()
        {

        }

        /// <summary>
        /// Adds a single AttributeUUID to the IndexKeyDefinition
        /// </summary>
        /// <param name="myIndexKeyDefinition">The AttributeUUID that is going to be added</param>
        public IndexKeyDefinition(AttributeUUID myIndexKeyDefinition)
        {
            _indexKeyAttributeUUIDs.Add(myIndexKeyDefinition);

            CalcNewHashCode(myIndexKeyDefinition);
        }

        /// <summary>
        /// Creates a new IndexKeyDefinition with a list of AttributeUUIDs
        /// </summary>
        /// <param name="myIndexKeyDefinitions">List of AttributeUUIDs</param>
        public IndexKeyDefinition(List<AttributeUUID> myIndexKeyDefinitions)
        {
            foreach (var aAttributeUUID in myIndexKeyDefinitions)
            {
                _indexKeyAttributeUUIDs.Add(aAttributeUUID);

                CalcNewHashCode(aAttributeUUID);
            }
        }

        #endregion

        #region private helper

        /// <summary>
        /// Calculates a hashcode with the help of the old hashcode and a new AttributeUUID
        /// </summary>
        /// <param name="aAttributeUUID">The AttributeUUID which should be integrated into the hashcode</param>
        private void CalcNewHashCode(AttributeUUID aAttributeUUID)
        {
            _hashCode = _hashCode ^ aAttributeUUID.GetHashCode();
        }

        #endregion

        #region Overrides

        #region Equals Overrides

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override Boolean Equals(Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is IndexKeyDefinition)
            {
                IndexKeyDefinition p = (IndexKeyDefinition)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(IndexKeyDefinition p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (this._indexKeyAttributeUUIDs.Count != p.IndexKeyAttributeUUIDs.Count)
            {
                return false;
            }

            for (int i = 0; i < _indexKeyAttributeUUIDs.Count; i++)
            {
                if (this._indexKeyAttributeUUIDs[i] != p.IndexKeyAttributeUUIDs[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(IndexKeyDefinition a, IndexKeyDefinition b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(IndexKeyDefinition a, IndexKeyDefinition b)
        {
            return !(a == b);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            sb.AppendFormat("#{0}: ", _indexKeyAttributeUUIDs.Count);

            foreach (var aUUID in _indexKeyAttributeUUIDs)
            {
                sb.AppendFormat("{0}: {1},", counter, aUUID.ToString());
                counter++;
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        #endregion

        #endregion

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
            mySerializationWriter.WriteObject(_indexKeyAttributeUUIDs.Count);
            foreach (var attr in _indexKeyAttributeUUIDs)
                attr.Serialize(ref mySerializationWriter);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Int32 count = (Int32)mySerializationReader.ReadObject();
            _indexKeyAttributeUUIDs = new List<AttributeUUID>();
            for (Int32 i = 0; i < count; i++)
            {
                AttributeUUID AttributeUUID = new AttributeUUID();
                AttributeUUID.Deserialize(ref mySerializationReader);
                _indexKeyAttributeUUIDs.Add(AttributeUUID);

                CalcNewHashCode(AttributeUUID);
            }
        }

        #endregion
    }
}
