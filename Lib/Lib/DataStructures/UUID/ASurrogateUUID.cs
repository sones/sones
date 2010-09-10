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

/* <id name="PandoraFS – UUID" />
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
