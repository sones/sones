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

/* <id name="PandoraDB – OutputList" />
 * <copyright file="OutputList.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Is a mapping to the generic List<>.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.QueryLanguage.Result
{
    
    public class OutputList : AObject
    {
                
        #region TypeCode
        public override UInt32 TypeCode { get { throw new NotImplementedException(); } }
        #endregion
        
        public List<AObject> List { get; set; }
        public OutputList()
        {
            List = new List<AObject>();
        }

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            throw new NotImplementedException();
        }

        public override bool SupportsType(Type type)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(SerializationWriter writer, object value)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(SerializationReader reader, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
