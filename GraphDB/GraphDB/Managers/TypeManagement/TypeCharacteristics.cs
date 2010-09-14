/* <id name="GraphDB – TypeCharacteristics" />
 * <copyright file="TypeCharacteristics.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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
            IsBackwardEdge = false;
            IsUnique       = false;
            IsMandatory    = false;
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
