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
