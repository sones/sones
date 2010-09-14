/*
 * NObjectStored
 * (c) Achim Friedland, 20
 * 
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.Notifications;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Notification
{

    /// <summary>
    /// Notifies about a stored object
    /// </summary>

    public class NObjectStored : NFileSystem
    {

        #region Nested class Arguments

        public new class Arguments : INotificationArguments
        {

            public ObjectLocation   ObjectLocation      { get; private set; }
            public String           ObjectStream        { get; private set; }
            public String           ObjectEdition       { get; private set; }
            public ObjectRevisionID       ObjectRevisionID    { get; private set; }

            public Arguments()
            {
                ObjectLocation = new ObjectLocation(FSPathConstants.PathDelimiter);
            }

            public Arguments(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID)
            {
                ObjectLocation      = myObjectLocation;
                ObjectStream        = myObjectStream;
                ObjectEdition       = myObjectEdition;
                ObjectRevisionID    = myObjectRevisionID;
            }

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                var _SerializationWriter = new SerializationWriter();

                _SerializationWriter.WriteString(ObjectLocation.ToString());
                _SerializationWriter.WriteString(ObjectStream);
                _SerializationWriter.WriteString(ObjectEdition);
                _SerializationWriter.WriteString(ObjectRevisionID.ToString());

                return _SerializationWriter.ToArray();

            }

            public void Deserialize(Byte[] mySerializedBytes)
            {
                
                var _SerializationReader = new SerializationReader(mySerializedBytes);

                ObjectLocation      = new ObjectLocation(_SerializationReader.ReadString());
                ObjectStream        = _SerializationReader.ReadString();
                ObjectEdition       = _SerializationReader.ReadString();
                ObjectRevisionID    = new ObjectRevisionID(_SerializationReader.ReadString());

            }

            #endregion

        }

        #endregion


        #region Clientside properties for validate()

        public ObjectLocation   ObjectLocation      { get; private set; }
        public String           ObjectStream        { get; private set; }
        public String           ObjectEdition       { get; private set; }
        public ObjectRevisionID       ObjectRevisionID    { get; private set; }

        public NObjectStored()
        { }

        public NObjectStored(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID)
        {
            ObjectLocation      = myObjectLocation;
            ObjectStream        = myObjectStream;
            ObjectEdition       = myObjectEdition;
            ObjectRevisionID    = myObjectRevisionID;
        }

        #endregion



        #region ANotificationType

        public override Boolean Validate(INotificationArguments myNotificationArguments)
        {
            return ((NObjectStored.Arguments) myNotificationArguments).ObjectLocation == ObjectLocation;
        }

        public override String Description
        {
            get { return "Notifies about any changes for a particular ObjectLocator"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
