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
