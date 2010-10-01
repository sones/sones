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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.NewFastSerializer
{

    /// <summary>
    /// Allows a class to save/retrieve their internal data to/from an existing SerializationWriter/SerializationReader.
    /// </summary>
    public interface IOwnedDataSerializable
    {
        /// <summary>
        /// Lets the implementing class store internal data directly into a SerializationWriter.
        /// </summary>
        /// <param name="writer">The SerializationWriter to use</param>
        /// <param name="context">Optional context to use as a hint as to what to store (BitVector32 is useful)</param>
        void SerializeOwnedData(SerializationWriter writer, object context);

        /// <summary>
        /// Lets the implementing class retrieve internal data directly from a SerializationReader.
        /// </summary>
        /// <param name="reader">The SerializationReader to use</param>
        /// <param name="context">Optional context to use as a hint as to what to retrieve (BitVector32 is useful) </param>
        void DeserializeOwnedData(SerializationReader reader, object context);
    }

}
