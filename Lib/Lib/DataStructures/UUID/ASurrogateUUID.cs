/* <id name="GraphFS – UUID" />
 * <copyright file="ASurrogateUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.NewFastSerializer;

namespace sones.Lib.DataStructures.UUID
{

    public abstract class ASurrogateUUID : IFastSerializationTypeSurrogate
    {

        public abstract Boolean SupportsType(Type myType);
        
        public abstract void    Serialize(SerializationWriter mySerializationWriter, Object myValue);

        public abstract Object  Deserialize(SerializationReader mySerializationReader, Type myType);

        public abstract UInt32  TypeCode { get; }

    }

}
