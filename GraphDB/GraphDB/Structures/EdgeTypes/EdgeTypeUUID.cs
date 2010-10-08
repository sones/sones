/* <id name="GraphDB – EdgeTypeUUID" />
 * <copyright file="EdgeTypeUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class has been created in favour of getting compile errors when referencing a type.</summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class has been created in favour of getting compile errors when referencing an attribute.
    /// </summary>
    
    public class EdgeTypeUUID : UUID
    {

        #region TypeCode
        
        public override UInt32 TypeCode { get { return 456; } }

        #endregion

        #region Constructor(s)

        #region EdgeTypeUUID()

        public EdgeTypeUUID()
            : base()
        {
        }

        #endregion

        #region EdgeTypeUUID(myInt32)

        public EdgeTypeUUID(UInt64 myInt32)
            : base(myInt32.ToString())
        {
        }

        #endregion

        #region EdgeTypeUUID(myString)

        public EdgeTypeUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region EdgeTypeUUID(mySerializedData)

        public EdgeTypeUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region EdgeTypeUUID(ref mySerializationReader)

        public EdgeTypeUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        { }

        #endregion

        #endregion

    }
}
