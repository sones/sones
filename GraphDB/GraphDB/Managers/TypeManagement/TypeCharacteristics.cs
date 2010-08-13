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

/* <id name="sones GraphDB – TypeCharacteristics" />
 * <copyright file="TypeCharacteristics.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class holds all characteristics of an specific attribute of a DB type.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;

using System.Runtime.Serialization;
using sones.Lib.DataStructures;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// This class holds all characteristics of an specific attribute of a DB type.
    /// </summary>
    
    public class TypeCharacteristics : IFastSerialize
    {
        
        /// <summary>
        /// This attribute is a BackwardEdge
        /// </summary>
        public Boolean IsBackwardEdge { get; set; }

        /// <summary>
        /// If set to true, all object of the RelatedGraphType with this attribute a unique for this RelatedGraphType.
        /// If more attribute are set to UNIQUE, all together are unique.
        /// </summary>
        public Boolean IsUnique { get; set; }

                /// <summary>
        /// The Mandatory flag
        /// </summary>
        public Boolean IsMandatory { get; set; }

        /// <summary>
        /// Create standard Unique typeCharacteristic (no Queue, no Weighted)
        /// </summary>
        public TypeCharacteristics()
        {

        }

        public override string ToString()
        {
            if (!IsBackwardEdge && !IsUnique && !IsMandatory)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            if (IsBackwardEdge) sb.Append("IsBackwardEdge,");
            if (IsUnique) sb.Append("IsUnique,");
            if (IsMandatory) sb.Append("IsMandatory,");

            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return sb.ToString();
        }

        #region IFastSerialize Members

        public Boolean isDirty
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
            mySerializationWriter.WriteBoolean(IsBackwardEdge);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            IsBackwardEdge = mySerializationReader.ReadBoolean();
        }

        #endregion
 
    }

}
