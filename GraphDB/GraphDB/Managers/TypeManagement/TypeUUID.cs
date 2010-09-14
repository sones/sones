/* <id name="GraphDB – TypeUUID" />
 * <copyright file="TypeUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region usings

using System;
using System.Text;
using sones.Lib.DataStructures.UUID;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class has been created in favour of getting compile errors when referencing an attribute.
    /// </summary>
    
    public class TypeUUID : UUID
    {

        #region TypeCode 
        public override UInt32 TypeCode { get { return 222; } }
        #endregion

        #region Constructors

        #region TypeUUID()

        public TypeUUID()
            : this(Guid.NewGuid().ToString())
        {
        }

        #endregion

        #region TypeUUID(myUInt64)

        public TypeUUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        {
        }

        #endregion

        #region TypeUUID(myString)

        public TypeUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region TypeUUID(mySerializedData)

        public TypeUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion
        
        #region TypeUUID(ref mySerializationReader)

        public TypeUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        {
        }

        #endregion

        #endregion


        #region NewUUID()

        public new static TypeUUID NewUUID()
        {
            return new TypeUUID(Guid.NewGuid().ToByteArray());
        }

        #endregion

        #region ToString()

        public override string ToString()
        {
            //return _UUID.ToHexString(SeperatorTypes.NONE, true);
            return Encoding.UTF8.GetString(_UUID);
        }

        #endregion

    }
}
