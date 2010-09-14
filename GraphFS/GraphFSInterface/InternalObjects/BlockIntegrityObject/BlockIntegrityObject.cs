/* GraphFS - BlockIntegrityObject
 * (c) Achim Friedland, 2008 - 2009
 * 
 * This represents a structure for saving the block integrity check
 * values of an object stream within an array of fixed sized byte arrays.
 * This structure can handle multiple block integrity check arrays using
 * different block sizes.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// This represents a structure for saving the block integrity check
    /// values of an object stream within an array of fixed sized byte arrays.
    /// This structure can handle multiple block integrity check arrays using
    /// different block sizes.
    /// </summary>

    

    public class BlockIntegrityObject : ADictionaryObject<UInt64, Byte[]>
    {

        #region Data

        byte[] _SerializedObject;
        UInt64 _SerializedSize;

        #endregion


        #region Constructor

        #region BlockIntegrityObject()

        /// <summary>
        /// This will create an empty BlockIntegrityObject
        /// </summary>
        public BlockIntegrityObject()
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream   = FSConstants.BLOCKINTEGRITYSTREAM;

            // Object specific data...

        }

        #endregion

        #region BlockIntegrityObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of the BlockIntegrityObject (constisting of the ObjectPath and ObjectName) within the file system</param>
        /// <param name="mySerializedData">An array of bytes[] containing the serialized BlockIntegrityObject</param>
        public BlockIntegrityObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #endregion



        #region Objectspecific methods

        #region IntegrityCheckAlgorithm

        private IntegrityCheckTypes _IntegrityCheckAlgorithm;

        /// <summary>
        /// The algorithm used for block integrity checking
        /// </summary>
        public IntegrityCheckTypes IntegrityCheckAlgorithm
        {
            get
            {
                return _IntegrityCheckAlgorithm;
            }

            set
            {
                _IntegrityCheckAlgorithm  = value;
                isDirty                   = true;
//                _BlockIntegrityArray          = new Dictionary<UInt64, Byte[][]>();
            }

        }

        #endregion

        //#region BlockIntegrityArray

        ///// <summary>
        ///// A structure for saving the block integrity check values of an object
        ///// stream within an array of fixed sized byte arrays. This structure can
        ///// handle multiple block integrity check arrays using different block sizes
        ///// by using a dictionary <BlockSize, Byte[NumberOfBlocks][IntegrityCheckBlockSize]>
        ///// </summary>
        //public Dictionary<UInt64, Byte[][]> BlockIntegrityArray
        //{
        //    get
        //    {
        //        return _BlockIntegrityArray;
        //    }

        //    set
        //    {
        //        _BlockIntegrityArray = value;
        //        isDirty = true;
        //        _IndexHashTable = new Dictionary<UInt64, Byte[][]>();
        //    }

        //}

        //#endregion
        
        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new BlockIntegrityObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion

    }

}

