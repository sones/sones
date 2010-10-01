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

/*
 * AFSObject
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Runtime.Serialization;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Exceptions;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Errors;
using sones.GraphFS.Events;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The abstract class for all GraphFS objects.
    /// </summary>

    public abstract class AFSObject : AFSObjectOntology, IFastSerialize, IEstimable
    {


        // A delegate type for hooking up change notifications.
        
        #region AFSObject Events

        #region OnLoad/OnLoadEvent(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified whenever a AFSObject is
        /// ready to be loaded.
        /// </summary>
        public event GraphFSEventHandlers.OnLoadEventHandler OnLoad;

        /// <summary>
        /// Invoke the OnLoad event, called whenever a AFSObject
        /// is ready to be loaded.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnLoadEvent(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnLoad != null)
                OnLoad(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #region OnLoaded/OnLoadedEvent(myObjectLocator, myAFSObject)

        /// <summary>
        /// An event to be notified whenever a AFSObject
        /// was successfully loaded.
        /// </summary>
        public event GraphFSEventHandlers.OnLoadedEventHandler OnLoaded;

        /// <summary>
        /// Invoke the OnLoaded event, called whenever a AFSObject
        /// was successfully loaded.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnLoadedEvent(ObjectLocator myObjectLocator, AFSObject myAFSObject)
        {
            if (OnLoaded != null)
                OnLoaded(myObjectLocator, myAFSObject);
        }

        #endregion

        #region OnSave/OnSaveEvent(myObjectLocation, myAFSObject)

        /// <summary>
        /// An event to be notified whenever a AFSObject
        /// is ready to be saved.
        /// </summary>
        public event GraphFSEventHandlers.OnSaveEventHandler OnSave;

        /// <summary>
        /// Invoke the OnSave event, called whenever a AFSObject
        /// is ready to be saved.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnSaveEvent(ObjectLocation myObjectLocation, AFSObject myAFSObject)
        {
            if (OnSave != null)
                OnSave(myObjectLocation, myAFSObject);
        }

        #endregion

        #region OnSaved/OnSavedEvent(myObjectLocation, myAFSObject, myOldObjectRevisionID)

        /// <summary>
        /// An event to be notified whenever a AFSObject
        /// was successfully saved on disc.
        /// </summary>
        public event GraphFSEventHandlers.OnSavedEventHandler OnSaved;

        /// <summary>
        /// Invoke the OnSaved event, called whenever a AFSObject
        /// was successfully saved on disc.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnSavedEvent(ObjectLocator myObjectLocator, AFSObject myAFSObject, ObjectRevisionID myOldObjectRevisionID)
        {
            if (OnSaved != null)
                OnSaved(myObjectLocator, myAFSObject, myOldObjectRevisionID);
        }

        #endregion

        #region OnRemove/OnRemoveEvent(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified whenever a AFSObject
        /// is ready to be removed.
        /// </summary>
        public event GraphFSEventHandlers.OnRemoveEventHandler OnRemove;

        /// <summary>
        /// Invoke the OnSave event, called whenever a AFSObject
        /// is ready to be removed.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnRemoveEvent(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnRemove != null)
                OnRemove(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #region OnRemoved/OnRemovedEvent(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID)

        /// <summary>
        /// An event to be notified whenever a AFSObject
        /// was successfully removed.
        /// </summary>
        public event GraphFSEventHandlers.OnRemovedEventHandler OnRemoved;

        /// <summary>
        /// Invoke the OnRemoved event, called whenever a AFSObject
        /// was successfully removed.
        /// </summary>
        /// <param name="e">EventArgs</param>
        public virtual void OnRemovedEvent(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myRevisionID)
        {
            if (OnRemoved != null)
                OnRemoved(myObjectLocation, myObjectStream, myObjectEdition, myRevisionID);
        }

        #endregion

        #endregion

        #region Constructors

        #region AFSObject()

        /// <summary>
        /// This will set all important variables within this AFSObject.
        /// This will especially create a new ObjectUUID and mark the
        /// AFSObject as "new" and "dirty".
        /// </summary>
        public AFSObject()
        {

            _ObjectStream       = null;
            _ObjectSize         = 0;
            _ObjectSizeOnDisc   = 0;

            // Generate a new ObjectUUID
            if (ObjectUUID.Length == 0)
                ObjectUUID = new ObjectUUID();

        }

        #endregion

        #region AFSObject(myObjectUUID)

        /// <summary>
        /// This will set all important variables within this AFSObject.
        /// Additionally it sets the ObjectUUID to the given value and marks
        /// the AFSObject as "new" and "dirty".
        /// </summary>
        public AFSObject(ObjectUUID myObjectUUID)
        {

            _ObjectStream       = null;

            _ObjectSize         = 0;
            _ObjectSizeOnDisc   = 0;

            // Members of AGraphStructure
            ObjectUUID          = myObjectUUID;

        }

        #endregion

        #endregion


        #region Load/Save/SaveAs/Remove/Erase

        //#region Load(myObjectLocation, myObjectStream, myObjectEditon, myObjectRevisionID, myObjectCopy)

        //public void Load(String myObjectLocation, String myObjectStream, String myObjectEditon, RevisionID myObjectRevisionID, Int32 myObjectCopy)
        //{
        //}

        //#endregion

        #region Save()

        public Exceptional Save()
        {

            if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive)
                return _IGraphFSSessionReference.Value.StoreFSObject(this, true);

            return new Exceptional(new GraphFSError("No file system given!"));

        }

        #endregion

        #region SaveAs(myObjectLocation)

        public void SaveAs(ObjectLocation myObjectLocation)
        {

            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("myObjectLocation must not be null or its length be zero!");

            if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive)
            {
                ObjectLocatorReference.ObjectLocationSetter = myObjectLocation;
                Save();
            }

        }

        #endregion

        #region SaveAs(myObjectLocation, myObjectStream, myObjectEditon, myObjectRevisionID)

        public void SaveAs(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEditon, ObjectRevisionID myObjectRevisionID)
        {

            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("myObjectLocation must not be null or its length be zero!");

            if (myObjectStream == null || myObjectStream.Length == 0)
                throw new ArgumentNullException("myObjectStream must not be null or its length be zero!");

            if (myObjectEditon == null || myObjectEditon.Length == 0)
                throw new ArgumentNullException("myObjectEditon must not be null or its length be zero!");

            if (myObjectRevisionID == null)
                throw new ArgumentNullException("myObjectRevisionID must not be null!");

            if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive)
            {

                ObjectLocatorReference.ObjectLocationSetter = myObjectLocation;
                _ObjectStream       = myObjectStream;
                _ObjectEdition      = myObjectEditon;
                _ObjectRevisionID   = myObjectRevisionID;

                Save();

            }

        }

        #endregion

        //#region SaveAs(myIGraphFSSession, myObjectLocation, myObjectStream, myObjectEditon, myObjectRevisionID)

        //public void SaveAs(IGraphFSSession myIGraphFSSession, ObjectLocation myObjectLocation, String myObjectStream, String myObjectEditon, RevisionID myObjectRevisionID)
        //{

        //    if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
        //        throw new ArgumentNullException("myObjectLocation must not be null or its length be zero!");

        //    if (myObjectStream == null || myObjectStream.Length == 0)
        //        throw new ArgumentNullException("myObjectStream must not be null or its length be zero!");

        //    if (myObjectEditon == null || myObjectEditon.Length == 0)
        //        throw new ArgumentNullException("myObjectEditon must not be null or its length be zero!");

        //    if (myObjectRevisionID == null)
        //        throw new ArgumentNullException("myObjectRevisionID must not be null!");

        //    if (_FSSessionReference != null)
        //    {

        //        _ObjectStream       = myObjectStream;
        //        _ObjectLocation     = myObjectLocation;
        //        _ObjectEdition      = myObjectEditon;
        //        _ObjectRevisionID   = myObjectRevisionID;

        //        Save();

        //    }

        //}

        //#endregion

        #region Rename(myNewObjectname)

        public Exceptional Rename(String myNewObjectName)
        {

            if (myNewObjectName == null || myNewObjectName.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("Invalid ObjectName!");

            if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive)
            {

                var _Exceptional = _IGraphFSSessionReference.Value.RenameFSObject(this.ObjectLocation, myNewObjectName);

                if (_Exceptional.Success())
                    this.ObjectLocatorReference.ObjectLocationSetter = new ObjectLocation(ObjectPath, ObjectName);

                return _Exceptional;
            
            }

            return new Exceptional();

        }

        #endregion

        #region Remove()

        public void Remove()
        {
            if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive)
                _IGraphFSSessionReference.Value.RemoveFSObject(ObjectLocation, _ObjectStream, _ObjectEdition, _ObjectRevisionID);
        }

        #endregion

        #region Erase()

        public void Erase()
        {
            if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive)
                _IGraphFSSessionReference.Value.EraseFSObject(ObjectLocation, _ObjectStream, _ObjectEdition, _ObjectRevisionID);
        }

        #endregion

        #endregion


        

        #region OP

        /// <summary>
        /// used for serializing
        /// </summary>
        [Obsolete("Move me to a better place!")]
        protected enum OP
        {
            ADD,
            REM
        }

        #endregion


        #region New (De-)Serialization and Cloning

        #region Serialize(myIntegrityCheckAlgorithm, myEncryptionAlgorithm, myCacheSerializeData)

        /// <summary>
        /// This will serialize the whole AFSObject including the common header of an
        /// AAFSObject and the actual AFSObject
        /// </summary>
        /// <param name="myIntegrityCheckAlgorithm"></param>
        /// <param name="myEncryptionAlgorithm"></param>
        /// <param name="myCacheSerializeData"></param>
        /// <returns></returns>
        public Byte[] Serialize(IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm, Boolean myCacheSerializeData)
        {

            #region Data

            Int32 IntegrityCheckValue_Length = 0;
            Byte[] IntegrityCheckValue = null;
            Int64 IntegrityCheckValue_Position = 0;

            Int32 EncryptionParameters_Length = 0;
            Byte DataPadding_Length = 0;
            Int32 AdditionalPadding_Length = 0;
            Byte[] _TmpSerializedAGraphStructure = null;

            #endregion

            try
            {

                #region Init SerializationWriter

                SerializationWriter writer = new SerializationWriter();

                #endregion

                #region Pad the length of the EncryptionParameters

                EncryptionParameters = Encoding.Default.GetBytes("-HIGHSECUREDATA-");
                EncryptionParameters_Length = sones.Lib.BufferHelper.AlignBufferLength(EncryptionParameters.Length, 8);

                #endregion

                #region Serialize AAFSObjectHeader

                Byte[] AAFSObjectHeader = new Byte[HeaderLength];

                AAFSObjectHeader[0] = HeaderVersion;

                if (myIntegrityCheckAlgorithm != null)
                    IntegrityCheckValue_Length = myIntegrityCheckAlgorithm.HashSize;

                if (IntegrityCheckValue_Length % 8 == 0)
                    AAFSObjectHeader[1] = (Byte)(IntegrityCheckValue_Length / 8);
                else AAFSObjectHeader[1] = (Byte)((IntegrityCheckValue_Length / 8) + 1);

                if (EncryptionParameters_Length % 8 == 0)
                    AAFSObjectHeader[2] = (Byte)(EncryptionParameters_Length / 8);
                else AAFSObjectHeader[2] = (Byte)((EncryptionParameters_Length / 8) + 1);

                AAFSObjectHeader[3] = (Byte)(DataPadding_Length);
                AAFSObjectHeader[4] = (Byte)(AdditionalPadding_Length / 256);
                AAFSObjectHeader[5] = (Byte)(AdditionalPadding_Length % 256);
                AAFSObjectHeader[6] = 0x00;
                AAFSObjectHeader[7] = 0x00;

                writer.WriteBytesDirect(AAFSObjectHeader);                      // 8 Bytes
                IntegrityCheckValue_Position = writer.BaseStream.Position;
                writer.WriteBytesDirect(new Byte[IntegrityCheckValue_Length]);      // n or at least 16 Bytes
                writer.WriteBytesDirect(EncryptionParameters);                      // m Bytes

                #endregion

                #region Serialize StructureVersion, ObjectUUID and the Inner Object

                writer.WriteBytesDirect(BitConverter.GetBytes(_StructureVersion));
                ObjectUUID.Serialize(ref writer);        // n or at least 16 Bytes                

                Serialize(ref writer);

                #endregion

                _TmpSerializedAGraphStructure = writer.ToArray();

                #region Encrypt
                #endregion

                #region Add IntegrityCheck

                if (myIntegrityCheckAlgorithm != null && IntegrityCheckValue_Length > 0)
                {

                    IntegrityCheckValue = myIntegrityCheckAlgorithm.GetHashValueAsByteArray(_TmpSerializedAGraphStructure);

                    // If the returned array is shorter than expected => pad with 0x00
                    // And if it is longer just copy the number of expected bytes
                    Array.Copy(IntegrityCheckValue, 0, _TmpSerializedAGraphStructure, IntegrityCheckValue_Position, IntegrityCheckValue.Length);

                }

                #endregion

                isDirty = false;

                if (myCacheSerializeData)
                    _SerializedAGraphStructure = _TmpSerializedAGraphStructure;


                return _TmpSerializedAGraphStructure;

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message, e);
            }

        }

        #endregion

        #region Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, myIgnoreIntegrityCheckFailures)

        public Exceptional Deserialize(Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm, Boolean myIgnoreIntegrityCheckFailures)
        {

            #region Data

            Byte[] IntegrityCheckValue;
            int IntegrityCheckValue_Length;
            Int64 IntegrityCheckValue_Position;
            Byte[] actualIntegrityCheckValue;

            Byte[] EncryptionParameters;
            int EncryptionParameters_Length;

            int DataPadding_Length;
            int AdditionalPadding_Length = 0;

            #endregion

            #region Check if data is larger than the minimum allowed size

            if (mySerializedData == null)
                throw new GraphFSException_InvalidInformationHeader("The information header is invalid!");

            if (mySerializedData.Length < 8)
                throw new GraphFSException_InvalidInformationHeader("The information header is invalid!");

            #endregion

            try
            {

                #region Init reader

                var _SerializationReader = new SerializationReader(mySerializedData);

                #endregion

                #region Read HeaderVersion

                var _NewHeaderVersion = _SerializationReader.ReadByte();

                #endregion

                #region Read Length of the Integrity Check Value

                // Multiply the value of the first byte with 8
                IntegrityCheckValue_Length = _SerializationReader.ReadByte() << 3;

                if (IntegrityCheckValue_Length > mySerializedData.Length - HeaderLength)
                    throw new GraphFSException_InvalidIntegrityCheckLengthField("The length of the integrity check value is invalid!");

                // HACK: Remeber that a IntegrityCheckValue of 0 will circumvent the whole integrity checking!
                if (myIntegrityCheckAlgorithm != null)
                    if ((IntegrityCheckValue_Length > 0) && (IntegrityCheckValue_Length != myIntegrityCheckAlgorithm.HashSize))
                        throw new GraphFSException_InvalidIntegrityCheckLengthField("The length of the integrity check value is " + IntegrityCheckValue_Length + ", but " + myIntegrityCheckAlgorithm.HashSize + " was expected!");

                #endregion

                #region Read Length of the Encryption Parameters

                // Multiply the value of the second byte with 8
                EncryptionParameters_Length = _SerializationReader.ReadByte() << 3;

                if (EncryptionParameters_Length > mySerializedData.Length - HeaderLength - IntegrityCheckValue_Length)
                    throw new GraphFSException_InvalidEncryptionParametersLengthField("The length of the encryption parameters is invalid!");

                #endregion

                #region Read Padding lengths

                DataPadding_Length = (Int32)_SerializationReader.ReadByte();
                AdditionalPadding_Length = (Int32)(256 * _SerializationReader.ReadByte() + _SerializationReader.ReadByte()) << 3;

                if ((HeaderLength + IntegrityCheckValue_Length + EncryptionParameters_Length + AdditionalPadding_Length) >= mySerializedData.Length)
                    throw new GraphFSException_InvalidAdditionalPaddingLengthField("The length of the additional padding is invalid!");

                _SerializationReader.ReadBytesDirect(2);  // Read reserved bytes

                #endregion

                #region Read Integrity Check Value and Encryption Parameters

                IntegrityCheckValue_Position = _SerializationReader.BaseStream.Position;

                if (IntegrityCheckValue_Length > 0)
                    IntegrityCheckValue = _SerializationReader.ReadBytesDirect(IntegrityCheckValue_Length);

                if (EncryptionParameters_Length > 0)
                    EncryptionParameters = _SerializationReader.ReadBytesDirect(EncryptionParameters_Length);

                #endregion


                #region Verify the integrity of the data

                if (myIntegrityCheckAlgorithm != null && IntegrityCheckValue_Length > 0)
                {

                    // Save the read IntegrityCheckValue
                    IntegrityCheckValue = new Byte[IntegrityCheckValue_Length];
                    Array.Copy(mySerializedData, IntegrityCheckValue_Position, IntegrityCheckValue, 0, IntegrityCheckValue_Length);

                    // Zero the IntegrityCheckValue within the serialized data
                    Byte[] AllZeros = new Byte[IntegrityCheckValue_Length];
                    Array.Copy(AllZeros, 0, mySerializedData, IntegrityCheckValue_Position, IntegrityCheckValue_Length);

                    // Calculate the actual IntegrityCheckValue
                    actualIntegrityCheckValue = myIntegrityCheckAlgorithm.GetHashValueAsByteArray(mySerializedData);

                    // Compare read and actual IntegrityCheckValue
                    if (IntegrityCheckValue.CompareByteArray(actualIntegrityCheckValue) != 0 && myIgnoreIntegrityCheckFailures == false)
                        throw new GraphFSException_IntegrityCheckFailed(String.Concat("The IntegrityCheck failed as ", actualIntegrityCheckValue.ToHexString(), " is not equal to the expected ", IntegrityCheckValue.ToHexString()));

                }

                #endregion

                #region Decrypt the remaining data

                //EncryptedData_Length      = (UInt64) (mySerializedData.Length - (HeaderLength + IntegrityCheckValue_Length + EncryptionParameters_Length + AdditionalPadding_Length));
                //EncryptedData             = new Byte[EncryptedData_Length];
                //Array.Copy(mySerializedData, HeaderLength + IntegrityCheckValue_Length + EncryptionParameters_Length, EncryptedData, 0, (Int64) EncryptedData_Length);

                //#endregion

                //#region Decrypt Data

                // Decrypt Data, sooon...!

                //if ( (UInt64) DataPadding_Length >= EncryptedData_Length)
                //    throw new GraphFSException_InvalidDataPaddingLengthField("The length of the data padding is invalid!");

                //DecryptedData = new Byte[EncryptedData_Length - (UInt64) DataPadding_Length];
                //Array.Copy(EncryptedData, 0, DecryptedData, 0, (Int64) (EncryptedData_Length - (UInt64) DataPadding_Length));

                #endregion


                #region Deserialize Inner Object

                _StructureVersion = BitConverter.ToUInt16(_SerializationReader.ReadBytesDirect(2), 0);
                ObjectUUID = new ObjectUUID();
                ObjectUUID.Deserialize(ref _SerializationReader); // n or at least 16 Bytes

                Deserialize(ref _SerializationReader);

                #endregion


            }

            catch (GraphFSException_IntegrityCheckFailed e)
            {
                throw new GraphFSException_IntegrityCheckFailed("The AGraphStructure could not be deserialized as its integrity is corrupted!\n\n" + e);
            }

            catch (Exception e)
            {
                throw new GraphFSException_AGraphStructureCouldNotBeDeserialized("The AGraphStructure could not be deserialized!\n\n" + e);
            }

            _SerializedAGraphStructure = mySerializedData;

            return new Exceptional();

        }

        #endregion

        #region (abstract) SerializeInnerObject(ref mySerializationWriter)

        /// <summary>
        /// This method will serialize this AGraphStructure
        /// </summary>
        /// <param name="mySerializationWriter">An SerializationWriter to write the serialized bytes to</param>
        public abstract void Serialize(ref SerializationWriter mySerializationWriter);

        #endregion

        #region (abstract) DeserializeInnerObject(ref mySerializationReader)

        /// <summary>
        /// This method will deserialize the content of the given array of bytes into this AGraphStructure
        /// </summary>
        /// <param name="mySerializationReader">An SerializationReader to read the serialized bytes from</param>
        public abstract void Deserialize(ref SerializationReader mySerializationReader);

        #endregion

        #region (abstract) Clone()

        /// <summary>
        /// This will create an exact deep-copy of this AAFSObject
        /// </summary>
        /// <returns>An exact deep-copy of this AAFSObject</returns>
        public abstract AFSObject Clone();

        #endregion

        #endregion


        #region Members of IFastSerialize

        #region Serialize()

        /// <summary>
        /// An abstract method to serialize this file system structure or object.
        /// </summary>
        /// <returns></returns>
        public Byte[] Serialize()
        {
            return Serialize(null, null, true);
        }

        #endregion

        #region Deserialize(mySerializedData)

        /// <summary>
        /// An abstract method to deserialize this file system structure or object.
        /// </summary>
        /// <returns></returns>
        public Exceptional Deserialize(Byte[] mySerializedData)
        {
            return Deserialize(mySerializedData, null, null, false);
        }

        #endregion

        #endregion

        #region Members of IEstimable

        public abstract ulong GetEstimatedSize();

        #endregion


        #region Deserialize(mySerializedData, myAAFSObject)

        /// <summary>
        /// This will call the normal Deserialize method and afterwards it will
        /// copy the content of all AGraphStructure and AAFSObject properties
        /// to the clone.
        /// </summary>
        /// <param name="mySerializedData">The fastserialized object as an array of bytes</param>
        /// <param name="myAAFSObject">The AAFSObject to copy the content from</param>
        public void Deserialize(Byte[] mySerializedData, AFSObject myAAFSObject)
        {
            Deserialize(mySerializedData);
            CloneObjectOntology(myAAFSObject);
        }

        #endregion

        #region Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, myAAFSObject)

        /// <summary>
        /// This will call the normal Deserialize method and afterwards it will
        /// copy the content of all AGraphStructure and AAFSObject properties
        /// to the clone.
        /// </summary>
        /// <param name="mySerializedData">The fastserialized object as an array of bytes</param>
        /// <param name="myAAFSObject">The AAFSObject to copy the content from</param>
        public void Deserialize(Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm, AFSObject myAAFSObject)
        {
            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            CloneObjectOntology(myAAFSObject);
        }

        #endregion
    }

}
