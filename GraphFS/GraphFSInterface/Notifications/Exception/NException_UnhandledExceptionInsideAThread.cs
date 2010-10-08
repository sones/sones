using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphFS.Notification
{
    public class NException_UnhandledExceptionInsideAThread : NException
    {

        public new class Arguments : INotificationArguments
        {

            public String ThrowedException;

            #region Constructors

            public Arguments() { }

            public Arguments(Exception myThrowedException)
            {
                ThrowedException = myThrowedException.Message + Environment.NewLine + myThrowedException.StackTrace;
            }

            #endregion

            #region INotificationArguments Members

            public byte[] Serialize()
            {
                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(ThrowedException);

                return _SerializationWriter.ToArray();
            }

            public void Deserialize(byte[] mySerializedBytes)
            {
                var _SerializationReader = new SerializationReader(mySerializedBytes);
                ThrowedException = _SerializationReader.ReadString();
            }

            #endregion
        
        }

        #region ANotificationType

        public override string Description
        {
            get { return "Notifies about any exception which is unhandled and throwed in any spawned thread."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion
    
    }

}
