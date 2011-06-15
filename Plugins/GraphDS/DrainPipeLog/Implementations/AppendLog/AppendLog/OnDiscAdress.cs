/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Serializer;

namespace sones.Plugins.GraphDS.DrainPipeLog.Storage
{
    public class OnDiscAdress : IFastSerialize
    {
        public Int64 CreationTime;
        public Int64 Start;
        public Int64 End;

        public OnDiscAdress()
        {
            CreationTime = DateTime.Now.Ticks;
        }

        #region IFastSerialize Members
        virtual public void Serialize(ref Library.NewFastSerializer.SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteInt64(CreationTime);
            mySerializationWriter.WriteInt64(Start);
            mySerializationWriter.WriteInt64(End);
        }

        public byte[] SerializeAligned(ref Library.NewFastSerializer.SerializationWriter mySerializationWriter)
        {
            this.Serialize(ref mySerializationWriter);

            // align it...for the sake of performance we do not check upfront, it gets copied 
            // already two times...
            byte[] Output = new byte[33];
            mySerializationWriter.ToArray().CopyTo(Output, 0);

            return Output;
        }


        virtual public void Deserialize(ref Library.NewFastSerializer.SerializationReader mySerializationReader)
        {
            CreationTime = mySerializationReader.ReadInt64();
            Start = mySerializationReader.ReadInt64();
            End = mySerializationReader.ReadInt64();
        }
        #endregion
    }
}
