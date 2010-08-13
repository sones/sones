///* GraphFS - UserMetadataObject
// * (c) Achim Friedland, 2008 - 2009
// * 
// * Lead programmer:
// *      Achim Friedland
// * 
// * */

//#region Usings

//using System;
//using System.Text;

//using sones.Graph.Storage.GraphFS.Datastructures;
//using sones.Graph.Storage.GraphFS.Objects;

//using sones.Lib.Cryptography.IntegrityCheck;
//using sones.Lib.Cryptography.SymmetricEncryption;
//using sones.Graph.Storage.GraphFS.InternalObjects;
//using sones.Lib.DataStructures;

//#endregion

//namespace sones.GraphFS.Objects
//{

//    /// <summary>
//    ///  A MetadataObject to store user defined information.
//    /// </summary>
//    public class UserMetadataObject : MetadataObject<Object>, IDirectoryObject
//    {


//        #region Constructor

//        #region UserMetadataObject()

//        /// <summary>
//        /// This will create an empty MetadataObject
//        /// </summary>
//        public UserMetadataObject()
//        {

//            // Members of AGraphStructure
//            _StructureVersion   = 1;

//            // Members of AGraphObject
//            _ObjectStream       = FSConstants.USERMETADATASTREAM;

//            // Object specific data...
////            _IndexHashTable     = new Dictionary<String, List<T>>();

//        }

//        #endregion


//        #region UserMetadataObject(myObjectLocation)

//        /// <summary>
//        /// This will create an empty UserMetadataObject
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        public UserMetadataObject(ObjectLocation myObjectLocation)
//            : this()
//        {

//            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
//                throw new ArgumentNullException("Invalid ObjectLocation!");

//            // Set the property in order to automagically set the
//            // ObjectPath and ObjectName
//            ObjectLocation      = myObjectLocation;

//        }

//        #endregion

//        #region UserMetadataObject(myObjectLocation, myObjectStream)

//        /// <summary>
//        /// This will create a UserMetadataObject with the given ObjectLocation and ObjectStream.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        public UserMetadataObject(ObjectLocation myObjectLocation, String myObjectStream)
//            : this(myObjectLocation)
//        {

//            if (myObjectStream == null || myObjectStream.Length == 0)
//                throw new ArgumentNullException("Invalid ObjectStream!");

//            _ObjectStream = myObjectStream;

//        }

//        #endregion

//        #region UserMetadataObject(myObjectLocation, myObjectStream, myObjectEdition)

//        /// <summary>
//        /// This will create a UserMetadataObject with the given ObjectLocation, ObjectStream and ObjectEdition.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        public UserMetadataObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
//            : this(myObjectLocation, myObjectStream)
//        {

//            if (myObjectEdition == null || myObjectEdition.Length == 0)
//                _ObjectEdition = FSConstants.DefaultEdition;

//            else
//                _ObjectEdition = myObjectEdition;

//        }

//        #endregion

//        #region UserMetadataObject(myObjectLocation, myObjectStream, myObjectEdition, myObjectRevisionID)

//        /// <summary>
//        /// This will create a UserMetadataObject with the given ObjectLocation, ObjectStream, ObjectEdition and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectStream">the ObjectStream</param>
//        /// <param name="myObjectEdition">the ObjectEdition</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public UserMetadataObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, RevisionID myObjectRevisionID)
//            : this(myObjectLocation, myObjectStream, myObjectEdition)
//        {

//            if (myObjectRevisionID == null)
//                throw new ArgumentNullException("Invalid ObjectRevisionID!");

//            else
//            {

//                if (myObjectRevisionID.UUID == null)
//                    throw new ArgumentNullException("Invalid ObjectRevisionID UUID!");

//                else
//                    _ObjectRevisionID = myObjectRevisionID;

//            }

//        }

//        #endregion

//        #region UserMetadataObject(myObjectLocation, myObjectRevisionID)

//        /// <summary>
//        /// This will create a UserMetadataObject with the given ObjectLocation and ObjectRevisionID.
//        /// </summary>
//        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
//        /// <param name="myObjectRevision">the RevisionID of the AGraphObject</param>
//        public UserMetadataObject(ObjectLocation myObjectLocation, RevisionID myObjectRevisionID)
//            : this(myObjectLocation)
//        {

//            if (myObjectRevisionID == null)
//                throw new ArgumentNullException("Invalid ObjectRevisionID!");

//            else
//            {

//                if (myObjectRevisionID.UUID == null)
//                    throw new ArgumentNullException("Invalid ObjectRevisionID UUID!");

//                else
//                    _ObjectRevisionID = myObjectRevisionID;

//            }

//        }

//        #endregion


//        #region UserMetadataObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

//        /// <summary>
//        /// A constructor used for fast deserializing
//        /// </summary>
//        /// <param name="myObjectLocation">The ObjectLocation</param>
//        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized MetadataObject</param>
//        public UserMetadataObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
//            : this(myObjectLocation)
//        {

//            if (mySerializedData == null || mySerializedData.Length == 0)
//                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

//            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
//            _isNew = false;

//        }

//        #endregion

//        #endregion


//        #region Members of AGraphObject

//        #region Clone()

//        public override AGraphObject Clone()
//        {

//            var newT = new UserMetadataObject();
//            newT.Deserialize(Serialize(null, null, false), null, null, this);

//            return newT;

//        }

//        #endregion

//        #endregion

 
//    }

//}
