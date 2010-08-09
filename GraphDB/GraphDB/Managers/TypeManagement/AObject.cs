/* <id name="PandoraDB – AObject" />
 * <copyright file="AObject.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Each Object which go threw the WebService need to derive from AObject. The main transforming is currently done by PandoraDatabaseHost.TransformSelectionListForCustomer.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.Structures.Result
{

    /// <summary>
    /// Each Object which go threw the WebService need to derive from AObject. The main transforming is currently done by PandoraDatabaseHost.TransformSelectionListForCustomer.
    /// </summary>
    [Serializable]
    public abstract class AObject
    {

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

        public abstract void Serialize(ref SerializationWriter mySerializationWriter);

        public abstract void Deserialize(ref SerializationReader mySerializationReader);

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public abstract bool SupportsType(Type type);

        public abstract void Serialize(SerializationWriter writer, object value);

        public abstract object Deserialize(SerializationReader reader, Type type);

        public abstract UInt32 TypeCode { get; }

        #endregion

    }

}
