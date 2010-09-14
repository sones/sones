/*
 * GraphDS - NGraphDSReady
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Text;

using sones.Notifications;
using sones.Lib.NewFastSerializer;

#endregion


namespace sones.GraphDS.API.CSharp.Notifications
{

    /// <summary>
    /// Notifies about an ready to use GraphDS instance.
    /// </summary>

    public class NGraphDSReady : NGraphDS
    {

        public new class Arguments : INotificationArguments
        {
            
            #region Properties

            public String Message { get; set; }

            #endregion

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteString(Message);

                return _SerializationWriter.ToArray();

            }

            public void Deserialize(Byte[] mySerializedBytes)
            {

                var _SerializationReader = new SerializationReader(mySerializedBytes);
                Message = _SerializationReader.ReadString();

            }

            #endregion

        }


        #region INotificationType Members

        public override String Description
        {
            get { return "Notifies about an ready to use GraphDS instance."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion
    
    }

}
