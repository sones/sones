/*
 * FileObject
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Runtime.Serialization;

using sones.GraphFS.Exceptions;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;
using sones.Lib;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// This represents a FileObject within the GraphFS which is
    /// responsible for storing normal files.
    /// </summary>

    public class FileObject : AFileObject
    {

        #region Properties

        #region Data - just a bunch of bytes

        private Byte[] _ObjectData;

        /// <summary>
        /// The interal array of bytes
        /// </summary>
        public override Byte[] ObjectData
        {

            get
            {
                return _ObjectData;
            }

            set
            {
                _ObjectData  = value;
                isDirty      = true;
            }

        }

        #endregion

        #region ContentType

        private String _ContentType;

        public override String ContentType
        {

            get
            {
                return _ContentType;
            }

            set
            {
                _ContentType = value;
                isDirty   = true;
            }

        }

        #endregion

        #endregion

        #region Constructors

        #region FileObject()

        public FileObject()
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream       = FSConstants.FILESTREAM;

            // Object specific data...
            _ObjectData         = null;
            _ContentType           = "application/unknown";

        }

        #endregion

        #region FileObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized FileObject</param>
        public FileObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (myObjectLocation == null || myObjectLocation.Length == 0)
                throw new ArgumentNullException("Invalid myObjectLocation!");

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new FileObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region Serialize(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {
                mySerializationWriter.WriteString(_ContentType);
                mySerializationWriter.Write(_ObjectData);
            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        #endregion

        #region Deserialize(ref mySerializationReader)

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {

                //var _count = mySerializationReader.BaseStream.Length - mySerializationReader.BaseStream.Position;
                //_ObjectData = new Byte[_count];

                //var b = mySerializationReader.BaseStream.Read(_ObjectData, 0, (int)_count);

                _ContentType = mySerializationReader.ReadString();
                _ObjectData  = mySerializationReader.ReadByteArray();

            }

            catch (Exception e)
            {
                throw new GraphFSException_FileObjectCouldNotBeDeserialized("The FileObject could not be deserialized!\n\n" + e);
            }

        }

        #endregion

        #region IEstimable Members

        public override ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.CalcByteArray(_ObjectData);
        }

        #endregion
    }

}
