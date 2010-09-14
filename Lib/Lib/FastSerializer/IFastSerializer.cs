/* IFastSerialize
 * (c) Daniel Kirstenpfad, 2007 - 2008
 * (c) Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 *      Achim Friedland
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.Lib.Serializer
{

    #region NotIFastSerialized Attribute

    /// <summary>
    /// Use this attribute to mark member properties that are not serialized by the IFastSerialize Interface Implementation
    /// </summary>
    public class NotIFastSerialized : Attribute
    {
    }

    #endregion

    #region AllowNonEmptyConstructor for IFastSerialized implementations

    /// <summary>
    /// Use this attribute to mark member properties that are serialized by the IFastSerialize Interface Implementation
    /// but have no empty constructor
    /// </summary>
    public class AllowNonEmptyConstructor : Attribute
    {
    }

    #endregion

    /// <summary>
    /// This interface has to be implemented by any object that should
    /// be serialized by the new FastSerializer.
    /// </summary>

    public interface IFastSerialize
    {

        /// <summary>
        /// This boolean is true when the object was changed since it was last serialized
        /// </summary>
        [NotIFastSerialized]
        Boolean isDirty { get; set; }

        /// <summary>
        /// This is the timestamp of the last object modification or its creation
        /// </summary>
        DateTime ModificationTime { get; }

        /// <summary>
        /// This method serializes the implementing object
        /// </summary>
        /// <param name="mySerializationWriter"></param>
        /// <returns>the serialized data as byte array</returns>
        void Serialize(ref SerializationWriter mySerializationWriter);

        /// <summary>
        /// This method deserializes the given array of bytes into the object that implements this interface
        /// </summary>
        /// <param name="mySerializationReader"></param>
        void Deserialize(ref SerializationReader mySerializationReader);

    }

}
